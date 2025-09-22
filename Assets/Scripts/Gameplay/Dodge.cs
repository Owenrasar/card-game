using UnityEngine;

[System.Serializable]
public class Dodge : Action
{

    public override void Play()
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
        GameObject markerFab = parentTimeline.dodgeMarker;
        GameObject markerPlace = parentTimeline.transform.parent.Find("Markers").gameObject;
        int dir = parentTimeline.tileIndexs[0] - parentTimeline.tileIndexs[parentTimeline.tileIndexs.Count-1];

        foreach (var pos in parentTimeline.tileIndexs)
        {
            GameObject marker = Instantiate(markerFab, new Vector3(pos, 1, -1), Quaternion.identity);
            marker.transform.parent = markerPlace.transform;
            if (dir < 0)
            {
                marker.GetComponent<SpriteRenderer>().flipX = true;
            }
        }
    }
}
 