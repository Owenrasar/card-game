using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
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

    public void Hit(int baseDamage)
    {
        if (activeBlock)
        {
            if (baseDamage <= activeBlock.value)
            {
                
                activeBlock.value -= baseDamage;
                Debug.Log("blocked to " + activeBlock.value);
                return;
            }
            else
            {

                //gaurdbreak
                HP -= (baseDamage - activeBlock.value);
                stagger += (baseDamage - activeBlock.value);
                activeBlock = null;
                Debug.Log("blocked but not but OW");
            }
        }
        else if (activeDodge)
        {
            if (baseDamage <= activeDodge.value)
            {
                Debug.Log("dodged");
                return;
            }
            else
            {

                //staggering blow
                HP -= baseDamage;
                stagger += baseDamage;
                activeBlock = null;
                Debug.Log("dodged but not but OW");
            }
        }
        else
        {
            //normal hit
            HP -= baseDamage;
            Debug.Log("ow");
        }
    }
}
