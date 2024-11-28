using UnityEngine;
using UnityEngine.EventSystems;

public class OpenDeckBuildScreen : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] int deckNumber;

    public void OnPointerEnter(PointerEventData eventData)
    {
        // ½¦ÀÌ´õ È¿°ú
    }

    public void OnPointerExit(PointerEventData eventData)
    {
      
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        DeckBuildManager.Instance.GetComponent<DeckBuildManager>().StartDeckBuild(deckNumber);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
       
    }

}
