using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIAction : MonoBehaviour
{
    public TextMeshPro valueLabel;
    public TextMeshPro argLabel;

    public Action linkedAction;
    public Timeline linkedTimeline;
    public Color defaultColor;
    public Color attackColor;
    public Color dodgeColor;
    public Color blockColor;

    public int linkedIndex;

    private void Awake()
    {
    }

    public void UpdateAction()
    {
        
        if (linkedTimeline.actions.Count > (linkedIndex)){
            
            linkedAction = linkedTimeline.actions[linkedIndex];

            valueLabel.text = (linkedAction.value).ToString();
            argLabel.text = (linkedAction.arg).ToString();
            string type = linkedAction.giveType();
            if (type == "Attack"){
                changeColor(attackColor);
            }
            else if (type == "Dodge"){
                changeColor(dodgeColor);
            }
            else if (type == "Block"){
                changeColor(blockColor);
            }
            else if (type == "Telegraph"){
               valueLabel.text = "T";
               argLabel.text = "";
               changeColor(defaultColor);
            }
            else if (type == "Blank"){
               valueLabel.text = "";
               argLabel.text = "";
            } else{
                changeColor(defaultColor);
            }

        } else {
            valueLabel.text = "";
            argLabel.text = "";
        }
    }

    private void changeColor(Color color){
        GetComponent<SpriteRenderer>().color = color;
        valueLabel.color = color;
        argLabel.color = color;
    }
}

