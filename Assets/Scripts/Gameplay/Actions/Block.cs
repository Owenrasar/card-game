using UnityEngine;
using TMPro;

[System.Serializable]
public class Block : Action
{

    public override void PlaySpecific()
    {
        owner.GetComponent<HealthManager>().activeBlock = this;
        render();
    }

    public void expire(){
        GameObject.Destroy(markers[2].gameObject);
    }
    public override string giveType()
    {
        return "Block";
    }

    public override void render()
    {
        GameObject markerFab = markers[0];
        GameObject markerPlace = owner;
        int pos = parentTimeline.tileIndexs[0]; 
        GameObject marker = Instantiate(markerFab, new Vector3(pos, 1.5f, -0.5f), Quaternion.identity);
        marker.transform.parent = markerPlace.transform;
        markers.Add(marker);  
        marker.GetComponent<Marker>().FlashActive(0.0f,0.5f);
    }

    public bool blockHit(Attack atk,HealthManager healthMan){ //true if blocked, false if not
            int baseDamage = atk.value;
            if (baseDamage <= value)
            {
                value-=baseDamage;
                atk.owner.GetComponent<HealthManager>().takeStagger(value);
                parentTimeline.ownerLabel.text = (value).ToString();
                if (value == 0) this.ClashLose();
                return true;
            }
            else
            {
                //gaurdbreak
                healthMan.takeDamage(baseDamage - value);
                healthMan.takeStagger(baseDamage - value);
                this.ClashLose();
                atk.ClashWin();
                return false;
            }
    }
    
}
