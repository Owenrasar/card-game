using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineManager : MonoBehaviour
{
    public List<Timeline> timelines = new List<Timeline>();

    public float tickInterval = 1f;

    public float timeSize = 4;

    public CombatManager arena;


    public List<UICard> UIcards = new List<UICard>();

    public List<UITimeline> UItimelines = new List<UITimeline>();

    public EnemyAI enemyAi;

    public GameObject canvas;

    public GameObject winUI;

    public GameObject lossUI;

    void Start(){
        StartCardRound();
    }

    public void StartCardRound()
    {
        foreach (var timeline in UItimelines)
        {
            timeline.Init();
            timeline.UpdateActions();
        }
        StartCoroutine(DrawHand());
    }
    
    public void EndCardRound()
    {
        foreach (var card in UIcards)
        {
            card.MaxLower();
        }
    }
    public void StartCombatRound()
    {
        EndCardRound();
        foreach (var timeline in timelines)
        {
            timeline.Init();
        }

        timeSize = timelines[0].actions.Count+1;
        StartCoroutine(RunTimelineTicks());
    }

    IEnumerator RunTimelineTicks()
    {
        yield return new WaitForSeconds(tickInterval);//delay becasue why not
        for (int i = 0; i < timeSize; i++)
        {
            MasterTick();
            yield return new WaitForSeconds(tickInterval);
        }

        

        foreach (var timeline in timelines) //start setting up for next turn
        {
            timeline.End();
            if (timeline.owner.GetComponent<HealthManager>().HP <= 0){
                if (!(timeline.PlayerOwned)){//player WIN
                    GameObject newUI = Instantiate(winUI);
                    newUI.transform.SetParent(canvas.transform, false);
                    yield break;
                } else {//player LOOSE
                    GameObject newUI = Instantiate(lossUI);
                    newUI.transform.SetParent(canvas.transform, false);
                    yield break;
                }
            }
        }
        StartCoroutine(RaiseHand());
        enemyAi.PrepHand();
        foreach (var timeline in timelines){
            timeline.owner.GetComponent<HealthManager>().EndTurn();
        }
    }

    IEnumerator DrawHand()
    {
        yield return new WaitForSeconds(tickInterval);//delay becasue why not
        foreach (var card in UIcards)
        {
            card.Init();
            yield return new WaitForSeconds(tickInterval / 4);
        }
    }
    
    IEnumerator RaiseHand()
    {
        foreach (var card in UIcards)
        {
            card.Lower();
            yield return new WaitForSeconds(tickInterval / 8);
        }

        foreach (var timeline in UItimelines)
        {
            timeline.ResetActions();
        }
    }

    void MasterTick()
    {
        cleanseMarkers();
        foreach (var timeline in timelines)
        {
            timeline.Tick();
        }
        arena.Tick();
    }

    void cleanseMarkers()
    {
        GameObject markerPlace = transform.parent.Find("Markers").gameObject;

        foreach (Transform marker in markerPlace.transform)
        {
            GameObject.Destroy(marker.gameObject);
        }
    }
}