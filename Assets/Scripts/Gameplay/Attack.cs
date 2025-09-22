using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Attack : Action
{
    public List<int> area = new List<int>();
    public override void Play()
    {

        CombatManager arenaManager = parentTimeline.gameObject.GetComponent<CombatManager>();

        int mult = 0;
        if (arg >= 0)
        {
            mult = 1;
        }
        else
        {
            mult = -1;
        }

        for (int i = 1; i <= Mathf.Abs(arg); i++)
        {
            area.Add(parentTimeline.tileIndexs[0] + i * mult);//BAND AID FIX, make some sort of function to get correct tielindex. ALSO IT NEEDS TO FLIP
            //if you are striking something behind you.
        }
        arenaManager.AddAttack(this);
    }
    public string AttackClash(Attack otherAttack)//returns string represening loser
    {
        if (value > otherAttack.value)
        {
            otherAttack.Destroy();
            return "other";
        }
        else if (value < otherAttack.value)
        {
            Destroy();
            return "self";
        }
        else
        {
            otherAttack.Destroy();
            Destroy();
            return "both";
        }
    }

    public void Destroy()
    {
        value = 0;
    }

    public override string giveType()
    {
        return "Attack";
    }

    public override void render()
    {
        GameObject markerFab = parentTimeline.attackMarker;
        GameObject markerPlace = parentTimeline.transform.parent.Find("Markers").gameObject;

        foreach (var pos in area)
        {
            GameObject marker = Instantiate(markerFab, new Vector3(pos, 1, -2), Quaternion.identity);
            marker.transform.parent = markerPlace.transform;
        }

    }
}   