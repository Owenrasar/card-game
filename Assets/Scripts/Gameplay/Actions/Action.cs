using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public abstract class Action : ScriptableObject
{
    public List<GameObject> markers = new List<GameObject>(); //0t is used to make rest of markers, 1 is displayed in a lost clash, rest are actual markers in the game
    public int valueMod;

    public int value = 0;
    public float arg;//other value besides, well, value. used for area and duration

    public Card parentCard;

    public Timeline parentTimeline;

    public GameObject owner; //gameobject with the HP controller (and other stuff) involved in comabt

    public bool isEX = false;

    public abstract void Play();

    public void ClashLose() {
        Debug.Log("we LOST");
        int i = 0;
        Sprite lostSprite = markers[1].GetComponent<SpriteRenderer>().sprite;
        foreach (var marker in markers) {
            if (i > 1){
                marker.GetComponent<Marker>().Break();
                
            } else {
                i+=1;
            }
        }
        parentTimeline.ownerLabel.text = "x";
    }

    public void ClashWin() {

    }

    public void Contact(){
        
    }

    public abstract string giveType();

    public abstract void render();

    

}
