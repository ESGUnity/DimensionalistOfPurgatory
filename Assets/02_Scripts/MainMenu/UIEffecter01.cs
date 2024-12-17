using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIEffecter01 : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Color lightGray = new Color(0.8f, 0.8f, 0.8f, 1f); // R, G, B, A
    Color white = new Color(1f, 1f, 1f, 1f);
    void Start()
    {
        GetComponent<Image>().color = lightGray;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponent<Image>().color = white;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<Image>().color = lightGray;
    }
}
