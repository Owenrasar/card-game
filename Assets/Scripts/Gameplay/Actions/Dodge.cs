using UnityEngine;

[System.Serializable]
public class Dodge : Action
{

    public override void PlaySpecific()
    {
        owner.GetComponent<HealthManager>().activeDodge = this;
        parentTimeline.GetComponent<CombatManager>().AddDodge(this);
    }
    public override string giveType()
    {
        return "Dodge";
    }

    public override void render()
    {
        GameObject markerFab = markers[0];
        GameObject markerPlace = parentTimeline.transform.parent.Find("Markers").gameObject;
        int dir = parentTimeline.tileIndexs[0] - parentTimeline.tileIndexs[parentTimeline.tileIndexs.Count-1];
        bool first = true;
        foreach (var pos in parentTimeline.tileIndexs)
        {
            if (!first){
                GameObject marker = Instantiate(markerFab, new Vector3(pos, 1.5f, -1), Quaternion.identity);
                marker.transform.parent = markerPlace.transform;
                markers.Add(marker);
                marker.GetComponent<Marker>().FlashActive(0.0f,0.5f);
                if (dir < 0)
                {
                    marker.GetComponent<SpriteRenderer>().flipX = true;
                }
            } else {
                first = false;
            }
        }
    }
}
 