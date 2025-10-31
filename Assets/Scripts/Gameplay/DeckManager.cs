using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    private static System.Random rng = new System.Random();
    [SerializeReference] public List<Card> drawPile = new List<Card>();
    [SerializeReference] public List<Card> discardPile = new List<Card>();
    void Start() {
        Shuffle(drawPile);
    }
    void Awake(){
        Shuffle(drawPile);
    }
    public Card Draw()
    {
        Card drawnCard = drawPile[0];
        discardPile.Insert(0,drawnCard);
        drawPile.RemoveAt(0);
        CheckEmpty();
        return drawnCard;
    }

    void CheckEmpty()
    {
       if (drawPile.Count > 0){
        return;
       }
       drawPile.AddRange(discardPile);
       discardPile = new List<Card>();
       Shuffle(drawPile);
    }

    public static void Shuffle<T>(IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (list[n], list[k]) = (list[k], list[n]);
        }
    }
}
