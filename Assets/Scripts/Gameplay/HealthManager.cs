using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HealthManager : MonoBehaviour
{
    public GameObject notifyPrefab;
    public float HP;
    public float stagger = 0;

    public Block activeBlock;

    public Dodge activeDodge;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Hit(Attack atk)
    {
        int baseDamage = atk.value;
        if (activeBlock) //block-attack clash
        {
            ((Block)activeBlock).blockHit(atk,this);
        }
        else if (activeDodge)//dodge-attack clash
        {
            if (baseDamage <= activeDodge.value)
            {
                Debug.Log("dodged");
                return;
            }
            else
            {

                //staggering blow
                this.takeDamage(baseDamage);
                this.takeStagger(baseDamage);
                activeDodge.ClashLose();
                atk.ClashWin();

                activeDodge = null;
                
                Debug.Log("dodged but not but OW");
            }
        }
        else
        {
            //normal hit
            this.takeDamage(baseDamage);
            Debug.Log("ow");
        }
    }

    public void takeDamage(int damage){
        HP -= damage;
        GameObject n = Instantiate(notifyPrefab);
        n.GetComponent<Notify>().DisplayNotify((damage).ToString(), this.gameObject.transform.position, Color.red, 1f);
    }

    public void takeStagger(int damage){
        stagger -= damage;

        GameObject n = Instantiate(notifyPrefab);
        n.GetComponent<Notify>().DisplayNotify((damage).ToString(), this.gameObject.transform.position, Color.yellow, 0.75f);
    
    }
}
