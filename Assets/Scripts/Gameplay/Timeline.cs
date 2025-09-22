using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timeline : MonoBehaviour
{
    public bool PlayerOwned = false;
    [SerializeReference] public List<Card> cards = new List<Card>();
    
    [SerializeReference] public List<Action> actions = new List<Action>();

    public Action currAction;
    
    public int timeIndex = 0;

    public List<int> tileIndexs = new List<int>();

    public GameObject owner;

    public Timeline targetTimeline;

    public GameObject attackMarker;

    public GameObject dodgeMarker;

    public void Tick()
    {
        currAction = actions[timeIndex];

        //check if we need to FLIP
        if (targetTimeline)
        {
            int ownerPos = tileIndexs[tileIndexs.Count - 1];
            int targetPos = targetTimeline.tileIndexs[targetTimeline.tileIndexs.Count - 1];
            if (targetPos < ownerPos)
            {

                string type = currAction.giveType();
                if (type == "Dodge" || type == "Attack")
                {
                    currAction.arg *= -1;
                }
            }
        }


        currAction.Play();
        timeIndex += 1;

        if (timeIndex >= actions.Count)
        {
            End();
        }
    }
    void End()
    {

    }

    public void Init()
    {
        List<Card> uniqueCards = new List<Card>();

        foreach (var originalCard in cards)
        {
            Card newCard = Instantiate(originalCard);

            newCard.actions = new List<Action>();
            foreach (var originalAction in originalCard.actions)
            {
                Action newAction = Instantiate(originalAction);
                newAction.parentCard = newCard;
                newAction.parentTimeline = this;
                newAction.owner = owner;
                newAction.value = newCard.cost+newAction.valueMod;
                newCard.actions.Add(newAction);
                actions.Add(newAction);
            }

            uniqueCards.Add(newCard);
        }
        cards = uniqueCards;
    }

}
