using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineManager : MonoBehaviour
{
    public List<Timeline> timelines = new List<Timeline>();

    public float tickInterval = 1f;

    public float timeSize = 4;
    
    public CombatManager arena;

    void Start()
    {
        foreach (var timeline in timelines)
        {
            timeline.Init();
        }

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

    void MasterTick()
    {
        foreach (var timeline in timelines)
        {
            timeline.Tick();
        }
        arena.Tick();
    }
}