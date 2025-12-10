using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Timeline linkedTimeline;
    public DeckManager deck;
    private Card linkedCard;

    void Start()
    {
        PrepHand();
    }

    public void PrepHand()
    {
        int num = 0;
        
        while (true) 
        {
            if (linkedTimeline.actions.Count == 12)
            {
                break;
            }

            linkedCard = deck.Draw();
            if (linkedCard == null)
            {
                Debug.LogWarning("EnemyAI: Deck is empty, cannot fill timeline.");
                break;
            }

            linkedTimeline.cards.Add(linkedCard);
            linkedTimeline.softInit();
            if (linkedTimeline.actions.Count > 12) 
            {
                linkedTimeline.cards.RemoveAt(linkedTimeline.cards.Count - 1);
                deck.DiscardFromHand(linkedCard);
            }
            else
            {
                deck.DiscardFromHand(linkedCard);
            }

            num += 1;
            if (num > 99) 
            {
                Debug.Log("Emergency Exit: Infinite Loop Protection");
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}