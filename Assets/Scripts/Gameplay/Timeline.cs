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

    public Sprite attackDirectionalMarkerSprite;

    public GameObject dodgeMarker;

    public GameObject dodgeLostMarker;

    public GameObject blockMarker;

    public GameObject blockLostMarker;

    public Telegraph telegraph;

    public Blank blank;

    public Animator anim;

    public int side; //1 is right, -1 is left

    public int maxActions;

    private bool dead = false;
    public void Tick()
    {
        if (dead) return;
        if (timeIndex < actions.Count && timeIndex < maxActions*2)
        {
            currAction = actions[timeIndex];
        } else {
            currAction = blank;
        }
        //check if we need to FLIP
        if (targetTimeline)
        {
            CheckFlip();
            string type = currAction.giveType();
            int ownerPos = tileIndexs[tileIndexs.Count - 1];
            int targetPos = targetTimeline.tileIndexs[targetTimeline.tileIndexs.Count - 1];
            if (targetPos < ownerPos)
            {
                if ((type == "Dodge" || type == "Attack"))
                {
                    currAction.arg *= -1;
                }

                if (type == "Telegraph" && ((Telegraph)currAction).linkedAction.giveType() != "Block")
                {
                    ((Telegraph)currAction).linkedAction.arg *= -1;
                }
            }

            //display stuff
            if (type == "Telegraph")
            {
                anim.SetTrigger("prep " + ((Telegraph)currAction).linkedAction.giveType());
            }
            else
            {
                if (type != "Dodge")
                {
                    anim.SetTrigger(type);
                }
                else
                {
                    if (currAction.arg * side < 0)
                    {
                        anim.SetTrigger("back " + type);
                    }
                    else
                    {
                        anim.SetTrigger("forward " + type);
                    }
                }
            }






            currAction.Play();
            if (ownerLabel)
            {
                if (type == "Telegraph")
                {
                    ownerLabel.text = (((Telegraph)currAction).linkedAction.value).ToString();
                }
                else
                {
                    ownerLabel.text = (currAction.value).ToString();
                }
            }
            CheckFlip();
        }



        timeIndex += 1;

    }
    public void End()
    {
        actions = new List<Action>();
        cards = new List<Card>();
        timeIndex = 0;
    }
    
    void CheckFlip()
    {
        int ownerPos = tileIndexs[tileIndexs.Count - 1];
        int targetPos = targetTimeline.tileIndexs[targetTimeline.tileIndexs.Count - 1];
            
            
        if (targetPos < ownerPos && side == 1)
        {
            side = -1;
            StartCoroutine(LerpTurn(-1));
        }
        else if (targetPos > ownerPos && side == -1)
        {
            side = 1;
            StartCoroutine(LerpTurn(1));
        }
    }

    public void Init()
    {

        List<Card> uniqueCards = new List<Card>();
        actions = new List<Action>();
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
                newAction.value = newCard.cost + newAction.valueMod;
                string type = newAction.giveType();
                if (type == "Attack")
                {
                    newAction.markers.Add(attackMarker);
                    newAction.markers.Add(attackLostMarker);
                }
                if (type == "Dodge")
                {
                    newAction.markers.Add(dodgeMarker);
                    newAction.markers.Add(dodgeLostMarker);
                }
                if (type == "Block")
                {
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

    public void softInit()
    {

        List<Card> uniqueCards = new List<Card>();
        actions = new List<Action>();
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
                newAction.value = newCard.cost + newAction.valueMod;
                string type = newAction.giveType();
                if (type == "Attack")
                {
                    newAction.markers.Add(attackMarker);
                    newAction.markers.Add(attackLostMarker);
                }
                if (type == "Dodge")
                {
                    newAction.markers.Add(dodgeMarker);
                    newAction.markers.Add(dodgeLostMarker);
                }
                if (type == "Block")
                {
                    newAction.markers.Add(blockMarker);
                    newAction.markers.Add(blockLostMarker);
                }

                Telegraph newTelegraph = Instantiate(telegraph);
                newTelegraph.linkedAction = newAction;
                newTelegraph.parentCard = newCard;
                newTelegraph.parentTimeline = this;
                newTelegraph.owner = owner;

                actions.Add(newTelegraph);
                actions.Add(newAction);
            }
        }
    }

    private IEnumerator LerpTurn(int zScale)
    {
        Transform model = owner.transform.Find("Model");
        float time = 0;
        float duration = 0.1f;
        Vector3 startScale = model.transform.localScale;
        Vector3 endScale = new Vector3(1, 1, zScale);

        while (time < duration)
        {
            model.transform.localScale = Vector3.Lerp(startScale, endScale, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        model.transform.localScale = endScale;
    }

    public void Stagger(){
        for (int i = 0; i< actions.Count; i++){
            if (i >= timeIndex){
                actions[i] = blank;
            }
        }
    }

    public void Kill(){
        for (int i = 0; i< actions.Count; i++){
            if (i >= timeIndex){
                actions[i] = blank;
                dead = true;
            }
        }
    }
}
