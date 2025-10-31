using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timeline : MonoBehaviour
{
    public bool PlayerOwned = false;
    [SerializeReference] public List<Card> cards = new List<Card>();
    
    [SerializeReference] public List<Action> actions = new List<Action>();

    public Action currAction;
    
    public int timeIndex = 0;

    public List<int> tileIndexs = new List<int>();

    public GameObject owner;

    public TMP_Text ownerLabel;

    public Timeline targetTimeline;

    public GameObject attackMarker;

    public GameObject attackLostMarker;

    public GameObject dodgeMarker;

    public GameObject dodgeLostMarker;

    public GameObject blockMarker;

    public GameObject blockLostMarker;

    public Telegraph telegraph;

    public void Tick()
    {
        currAction = actions[timeIndex];

        //check if we need to FLIP
        if (targetTimeline)
        {
            string type = currAction.giveType();
            int ownerPos = tileIndexs[tileIndexs.Count - 1];
            int targetPos = targetTimeline.tileIndexs[targetTimeline.tileIndexs.Count - 1];
            if (targetPos < ownerPos)
            {
                if ((type == "Dodge" || type == "Attack"))
                {
                    currAction.arg *= -1;
                }

                if (type =="Telegraph" && ((Telegraph)currAction).linkedAction.giveType() != "Block") {
                    ((Telegraph)currAction).linkedAction.arg *= -1;
                }
            } 
            currAction.Play();
            if (ownerLabel) {
                if (type == "Telegraph") {
                    ownerLabel.text = (((Telegraph)currAction).linkedAction.value).ToString();
                } else {
                    ownerLabel.text = (currAction.value).ToString();
                }
            }
        
        } 

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
        Telegraph teleAction = null;

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
                string type = newAction.giveType();
                if (type == "Attack"){
                    newAction.markers.Add(attackMarker);
                    newAction.markers.Add(attackLostMarker);
                }
                if (type == "Dodge"){
                    newAction.markers.Add(dodgeMarker);
                    newAction.markers.Add(dodgeLostMarker);
                }
                if (type == "Block"){
                    newAction.markers.Add(blockMarker);
                    newAction.markers.Add(blockLostMarker);
                }

                Telegraph newTelegraph = Instantiate(telegraph);
                newTelegraph.linkedAction = newAction;
                newTelegraph.parentCard = newCard;
                newTelegraph.parentTimeline = this;
                newTelegraph.owner = owner;

                newCard.actions.Add(newTelegraph);
                newCard.actions.Add(newAction);

                actions.Add(newTelegraph);
                actions.Add(newAction);
            }

            uniqueCards.Add(newCard);
        }
        cards = uniqueCards;
    }

}
