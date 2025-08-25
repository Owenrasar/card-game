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
        for (int i = 1; i <= Mathf.Abs(arg); i++)
        {
            area.Add(parentTimeline.tileIndexs[0] + i);//BAND AID FIX, make some sort of function to get correct tielindex. ALSO IT NEEDS TO FLIP
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
}