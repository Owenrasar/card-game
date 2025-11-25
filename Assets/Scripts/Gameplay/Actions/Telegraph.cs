using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Telegraph : Action
{
    public Action linkedAction;
    public override void PlaySpecific()
    {
        CombatManager arenaManager = parentTimeline.gameObject.GetComponent<CombatManager>();
        arenaManager.AddTelegraph(this);
    }
    public override string giveType()
    {
        return "Telegraph";
    }

    public override void render()
    {
        Timeline timeline = parentTimeline;
        Timeline targetTimeline = timeline.targetTimeline;
        int ownerPos = timeline.tileIndexs[timeline.tileIndexs.Count - 1];
        int targetPos = timeline.targetTimeline.tileIndexs[targetTimeline.tileIndexs.Count - 1];
        bool swap = targetTimeline && (targetPos < ownerPos);
        

        string type = linkedAction.giveType();

        if (type == "Attack"){//attacks clear themselves so we can just render
            
            GameObject markerFab = linkedAction.markers[0];
            GameObject markerPlace = linkedAction.parentTimeline.transform.parent.Find("TeleMarkers").gameObject;
            
            
            
                int mult = 0;

                if (linkedAction.arg >= 0)//why do these numbers need to be flipped? GOOD QUESTION
                {
                    mult = 1;
                }
                else
                {
                    mult = -1;
                }

                for (int i = 1; i <= Mathf.Abs(linkedAction.arg); i++)
                {
                    ((Attack)linkedAction).area.Add(linkedAction.parentTimeline.tileIndexs[0] + i * mult);//BAND AID FIX, make some sort of function to get correct tielindex. ALSO IT NEEDS TO FLIP
                }
            GameObject marker = null;
            foreach (var pos in ((Attack)linkedAction).area)
            {
                marker = Instantiate(markerFab, new Vector3(pos, 1.5f, -1), Quaternion.identity);
                marker.transform.parent = markerPlace.transform;
                markers.Add(marker);
                marker.GetComponent<Marker>().PreRender();
                if (swap)
                {
                    marker.GetComponent<SpriteRenderer>().flipX = true;
                }
            }
            //marker.GetComponent<SpriteRenderer>().sprite = parentTimeline.attackDirectionalMarkerSprite;
            marker.transform.localScale = new Vector3(1, 1, 1);
            if (swap)
            {
                linkedAction.arg *= -1;
                //marker.GetComponent<SpriteRenderer>().flipX = true;
            }
            ((Attack)linkedAction).area = new List<int>();

        }   else if (type == "Block"){//blocks do not clear themselves, so we slap it in TeleMarkers to get cleared later
            GameObject markerFab = linkedAction.markers[0];
            GameObject markerPlace = linkedAction.parentTimeline.transform.parent.Find("TeleMarkers").gameObject;
            int pos = linkedAction.parentTimeline.tileIndexs[0]; 
            GameObject marker = Instantiate(markerFab, new Vector3(pos, 1.5f, -0.5f), Quaternion.identity);
            marker.transform.parent = markerPlace.transform;
            marker.GetComponent<Marker>().PreRender();

        } else if (type == "Dodge"){
            GameObject markerFab = linkedAction.markers[0];
            GameObject markerPlace = linkedAction.parentTimeline.transform.parent.Find("TeleMarkers").gameObject;
            int startTile = linkedAction.parentTimeline.tileIndexs[0];
            int dir = 0;
            if (linkedAction.arg > 0){
                dir = 1;
            } else {
                dir = -1;
            }
            float endTile = startTile+linkedAction.arg+dir;
            while (startTile != endTile){
                GameObject marker = Instantiate(markerFab, new Vector3(startTile, 1.5f, -1), Quaternion.identity);
                marker.transform.parent = markerPlace.transform;
                markers.Add(marker);
                marker.GetComponent<Marker>().PreRender();
                if (dir < 0)
                {
                    marker.GetComponent<SpriteRenderer>().flipX = true;
                }
                startTile+= dir;
            } 
            if (swap) linkedAction.arg *= -1;
        }
    }
}
