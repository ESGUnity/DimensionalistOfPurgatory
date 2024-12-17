using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class OpenDeckBuildScreen : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    private void Start()
    {
        GetComponent<IDeck>().SetDeckInfo();
    }
    private void Update()
    {
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = GetComponent<IDeck>().DeckName;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        // ½¦ÀÌ´õ È¿°ú
    }

    public void OnPointerExit(PointerEventData eventData)
    {
      
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        DeckBuildManager.Instance.StartDeckBuild(GetComponent<IDeck>());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
       
    }

}
