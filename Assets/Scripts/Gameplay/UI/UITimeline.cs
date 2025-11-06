using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITimeline : MonoBehaviour
{
    public List<UIAction> actions = new List<UIAction>();
    public Timeline linkedTimeline;
    public Color attackColor;
    public Color dodgeColor;
    public Color blockColor;

    void Start()
    {
        Init();
    }

    public void Init(){
        foreach(var action in actions){
            action.linkedTimeline = linkedTimeline;
            action.attackColor = attackColor;
            action.dodgeColor = dodgeColor;
            action.blockColor = blockColor;
        }
    }

    public void UpdateActions()
    {
        linkedTimeline.softInit();
        foreach(var action in actions){
            action.UpdateAction();
        }
    }
}
