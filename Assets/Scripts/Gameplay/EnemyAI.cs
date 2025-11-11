using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Timeline linkedTimeline;
    public DeckManager deck;
    private Card linkedCard;
    void Start(){
        PrepHand();
    }
    void PrepHand()
    {
        int num = 0;
        while (true) {
            if (linkedTimeline.actions.Count == 12){
                break;
            }

            linkedCard = deck.Draw();
            linkedTimeline.cards.Add(linkedCard);
            linkedTimeline.softInit();
            
            if (linkedTimeline.actions.Count > 12) {
                Debug.Log("removing: " + linkedTimeline.cards[linkedTimeline.cards.Count -1]);
                linkedTimeline.cards.RemoveAt(linkedTimeline.cards.Count -1);
            }
            num += 1;
            if (num > 99) {
                Debug.Log("Emergancy Exit");
                break;
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
