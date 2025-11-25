using UnityEngine;

[System.Serializable]
public class Blank : Action
{

    public override void PlaySpecific()
    {
    }
    public override string giveType()
    {
        return "Blank";
    }

    public override void render()
    {
        
    }
}
