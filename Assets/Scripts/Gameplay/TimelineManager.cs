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

    void Start(){
        StartCardRound();
    }

    public void StartCardRound() {
        foreach (var timeline in UItimelines)
        {
            timeline.UpdateActions();
        }
        StartCoroutine(DrawHand());
    }
    public void StartCombatRound()
    {
        
        foreach (var timeline in timelines)
        {
            timeline.Init();
        }

        timeSize = timelines[0].actions.Count+2;
        StartCoroutine(RunTimelineTicks());
    }

    IEnumerator RunTimelineTicks()
    {
        yield return new WaitForSeconds(tickInterval);//delay becasue why not
        for (int i = 0; i < timeSize; i++)
        {
            Debug.Log(i);
            MasterTick();
            yield return new WaitForSeconds(tickInterval);
        }
    }

    IEnumerator DrawHand()
    {
        yield return new WaitForSeconds(tickInterval);//delay becasue why not
        foreach (var card in UIcards)
        {
            card.Init();
            yield return new WaitForSeconds(tickInterval/4);
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