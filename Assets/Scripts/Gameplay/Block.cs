using UnityEngine;

[System.Serializable]
public class Block : Action
{

    public override void Play()
    {
        owner.GetComponent<HealthManager>().activeBlock = this;
    }
}
