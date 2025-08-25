using UnityEngine;

public abstract class Action : ScriptableObject
{
    public int valueMod;

    public int value = 0;
    public float arg;//other value besides, well, value. used for area and duration

    public Card parentCard;

    public Timeline parentTimeline;

    public GameObject owner; //gameobject with the HP controller (and other stuff) involved in comabt

    public bool isEX = false;

    public abstract void Play();

}
