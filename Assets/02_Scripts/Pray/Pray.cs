using UnityEngine;

public class Pray : MonoBehaviour
{
    [HideInInspector] public Vertex CastedGridVertex;
    [HideInInspector] public CardData cardData;
    [HideInInspector] public string thisPlayerTag;

    public void SetPrayInfo(Vertex castedGridVertex, CardData cardData, string tag)
    {
        CastedGridVertex = castedGridVertex;
        this.cardData = cardData;
        thisPlayerTag = tag;
    }
    public virtual void CastPray()
    {

    }
}
