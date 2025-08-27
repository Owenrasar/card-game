using UnityEngine;

[System.Serializable]
public class Dodge : Action
{

    public override void Play()
    {
        owner.GetComponent<HealthManager>().activeDodge = this;
        parentTimeline.GetComponent<CombatManager>().AddDodge(this);
    }
    public override string giveType()
    {
        return "Dodge";
    }
}
 