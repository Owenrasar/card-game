using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CombatManager : MonoBehaviour
{
    public TimelineManager timeManager;

    public List<Action> attacks = new List<Action>();


    public List<Action> dodges = new List<Action>();

    public List<Action> telegraphs = new List<Action>();

    public void Tick()
    {
        //determine hitboxes, then strike them, then update hitboxes, then remove blocks
        ResolvePrep();
        ResolveDodges();
        ResolveAttacks();
        ResetDodges();
        ResolveBlocks();
        RemoveDodges();
        attacks = new List<Action>();
        dodges = new List<Action>();
        telegraphs = new List<Action>();
    }

    public void AddAttack(Action attack)
    {
        attacks.Add(attack);
    }

    public void AddDodge(Action dodge)
    {
        dodges.Add(dodge);
    }

    public void AddTelegraph(Action telegraph)
    {
        
        telegraphs.Add(telegraph);
        Debug.Log("aded tele: " + telegraph);
    }

    public void ResolvePrep()
    {
        foreach(Transform child in transform.parent.Find("TeleMarkers").transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Telegraph telegraph in telegraphs)
        {
            telegraph.render();
        }
    }

    public void ResolveDodges()
    {
        foreach (Dodge dodge in dodges)
        {

            //extend hitbox for the dash
            List<int> newTiles = new List<int>();
            int startTile = dodge.parentTimeline.tileIndexs[0];
            int endTile = startTile + (int)dodge.arg;

            //End point becomes index 0
            if (startTile < endTile)
            {
                for (int i = endTile; i >= startTile; i--)
                    newTiles.Add(i);
            }
            else
            {
                for (int i = endTile; i <= startTile; i++)
                    newTiles.Add(i);
            }
            dodge.parentTimeline.tileIndexs = newTiles;
        }
        List<Timeline> dodgers = new List<Timeline>();
        List<Timeline> standers = new List<Timeline>();
        List<Timeline> blockers = new List<Timeline>();
        foreach (Timeline timeline in timeManager.timelines)
        {
            if (timeline.owner.GetComponent<HealthManager>().activeDodge != null)
            {
                dodgers.Add(timeline);
            }
            else if (timeline.owner.GetComponent<HealthManager>().activeBlock != null)
            {
                blockers.Add(timeline);
            }
            else
            {
                standers.Add(timeline);
            }
        }

        foreach (Timeline dTimeline in dodgers)
        {
            bool change = true;
            while (change)
            {
                change = false;
                foreach (Timeline sTimeline in standers)
                {
                    if (dTimeline.tileIndexs[0] == sTimeline.tileIndexs[0])
                    {
                        dTimeline.tileIndexs.RemoveAt(0);//just remove the exact endpoint
                        change = true;
                    }
                }
                foreach (Timeline bTimeline in blockers)
                {
                    if (dTimeline.tileIndexs[0] == bTimeline.tileIndexs[0])
                    {
                        dTimeline.tileIndexs.RemoveAt(0);//just remove the exact endpoint
                        change = true;
                    }
                }
                foreach (Timeline d2Timeline in dodgers)
                {
                    if ((d2Timeline != dTimeline) && (dTimeline.tileIndexs[0] == d2Timeline.tileIndexs[0]))
                    {
                        if (d2Timeline.owner.GetComponent<HealthManager>().activeDodge.value > dTimeline.owner.GetComponent<HealthManager>().activeDodge.value)
                        {
                            dTimeline.tileIndexs.RemoveAt(0);
                            change = true;
                        }
                        else if (dTimeline.owner.GetComponent<HealthManager>().activeDodge.value > d2Timeline.owner.GetComponent<HealthManager>().activeDodge.value)
                        {
                            d2Timeline.tileIndexs.RemoveAt(0);
                        }
                        else
                        {
                            dTimeline.tileIndexs.RemoveAt(0);
                            d2Timeline.tileIndexs.RemoveAt(0);
                            change = true;
                        }
                    }
                }
                foreach (Timeline bTimeline in blockers)
                {
                    List<int> overlap = bTimeline.tileIndexs.Intersect(dTimeline.tileIndexs).ToList();
                    if (dTimeline.owner.GetComponent<HealthManager>().activeDodge.value <= bTimeline.owner.GetComponent<HealthManager>().activeBlock.value)
                    { //dodge looses clash, gets blocked


                        if (overlap != null)
                        {
                            int blockerTile = bTimeline.tileIndexs[0];
                            if (bTimeline.tileIndexs.Count != 1)//both dodging and blocking
                            {
                                Debug.Log("YOU HAVENT CODED THIS INTERACTION YET BOZO");

                            }
                            else
                            {
                                int cutoff = bTimeline.tileIndexs[0];
                                int cutoffIndex = dTimeline.tileIndexs.IndexOf(cutoff);
                                dTimeline.tileIndexs = dTimeline.tileIndexs.Skip(cutoffIndex + 1).ToList();
                                
                            }

                        }
                    }
                    else
                    {
                        int blockerTile = bTimeline.tileIndexs[0];
                        int cutoffIndex = dTimeline.tileIndexs.IndexOf(blockerTile);
                        if (cutoffIndex > 0)
                        {
                            dTimeline.tileIndexs = dTimeline.tileIndexs.Take(cutoffIndex).ToList();
                            change = true; 
                        }
                    }
                }
            }
        }
        foreach (Dodge dodge in dodges) //now actually do the movment
        {
            Vector3 currentPos = dodge.owner.transform.position;
            Vector3 targetPos = currentPos;
            targetPos.x = dodge.parentTimeline.tileIndexs[0];

            StartCoroutine(LerpDodge(dodge.owner.transform, targetPos, 0.05f));
            
            dodge.render(); 
        }
    }

    public void ResetDodges()
    {
        foreach (Dodge dodge in dodges)
        {
            dodge.parentTimeline.tileIndexs.RemoveRange(1, dodge.parentTimeline.tileIndexs.Count - 1);//only keep index 0

        }
    }

    public void ResolveAttacks()
    {
        HashSet<Attack> toDestroy = new HashSet<Attack>();
        HashSet<Attack> toWin = new HashSet<Attack>();

        for (int i = 0; i < attacks.Count; i++) //attacks clash attacks
        {
            if (attacks[i] is not Attack atkA) continue;
            if (toDestroy.Contains(atkA)) continue;

            var areaA = new HashSet<int>(atkA.area);

            for (int j = i + 1; j < attacks.Count; j++)
            {
                if (attacks[j] is not Attack atkB) continue;
                if (toDestroy.Contains(atkB)) continue;

                if (atkA.owner == atkB.owner) continue;

                foreach (int tile in atkB.area)
                {
                    if (areaA.Contains(tile))
                    {
                        Debug.Log($"Collision on tile {tile} between Attack A: {atkA} and Attack B: {atkB}");
                        string loser = atkA.AttackClash(atkB);

                        if (loser == "other")
                        {
                            toDestroy.Add(atkB);
                            toWin.Add(atkA);
                        }
                        else if (loser == "self")
                        {
                            toDestroy.Add(atkA);
                            toWin.Add(atkB);
                        }
                        else if (loser == "both")
                        {
                            toDestroy.Add(atkA);
                            toDestroy.Add(atkB);
                        }

                        break;
                    }
                }
            }
        }

        foreach (var attack in attacks)//finish up clashing between attacks
        {
            if (toDestroy.Contains(attack)) {
                ((Attack)attack).lost = true;
                attack.ClashLose();
            }
            if (toWin.Contains(attack))
            {
                attack.ClashWin();
            }
            attack.render();
        }

        foreach (var action in attacks) // attacks damage (or get blocked or dodged)
        {
            if (action is not Attack atk) continue;

            // Keep track of which owners we've already hit this iteration
            HashSet<GameObject> alreadyHit = new HashSet<GameObject>();

            foreach (var timeline in FindObjectsOfType<Timeline>())
            {
                if (atk.owner == timeline.owner) continue; // Don't hit self
                if (alreadyHit.Contains(timeline.owner)) continue; // Already hit this one

                // Check if ANY of the creature's tile indices overlap the attack area
                bool overlaps = timeline.tileIndexs.Any(index => atk.area.Contains(index));
                if (overlaps) //DO THE DAMAGE
                {
                    timeline.owner.GetComponent<HealthManager>().Hit(atk);
                    if (action.value > 0) action.owner.GetComponent<ParticleManager>().playHit(timeline.owner);
                    alreadyHit.Add(timeline.owner);
                }
            }
        }
        //projectiles happen SOMEWHER
    }

    public void ResolveBlocks()
    {
        //blocks loose duration
        foreach (var timeline in FindObjectsOfType<Timeline>())
        {
            Block checker = timeline.owner.GetComponent<HealthManager>().activeBlock;
            if (checker)
            {
                checker.arg -= 1;
                if (checker.arg < 0)
                {
                    timeline.owner.GetComponent<HealthManager>().activeBlock = null;
                    checker.expire();
                }
            }
        }
    }

    public void RemoveDodges()
    {
        foreach (var timeline in FindObjectsOfType<Timeline>())
        {
            Dodge checker = timeline.owner.GetComponent<HealthManager>().activeDodge;
            if (checker)
            {
                timeline.owner.GetComponent<HealthManager>().activeDodge = null;
            }
        }
    }
    

    private IEnumerator LerpDodge(Transform targetTransform, Vector3 targetPosition, float duration)
    {
    float time = 0;
    Vector3 startPosition = targetTransform.position;

    while (time < duration)
    {
        targetTransform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
        time += Time.deltaTime;
        yield return null;
    }

    targetTransform.position = targetPosition;
    }
}



