using UnityEngine;

[System.Serializable]
public class Blank : Action
{

    public override void Play()
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
