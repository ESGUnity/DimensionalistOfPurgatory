using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectedDeckNotifier : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    private void Start()
    {
        GetComponent<IDeck>().SetDeckInfo();
        transform.GetChild(1).gameObject.SetActive(false);
    }
    private void Update()
    {
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = GetComponent<IDeck>().DeckName;

        if (GetComponent<IDeck>().DeckCount < 25) // 25장으로 수정
        {
            transform.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            transform.GetChild(1).gameObject.SetActive(false);
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 쉐이더 효과
    }

    public void OnPointerExit(PointerEventData eventData)
    {

    }
    public void OnPointerDown(PointerEventData eventData)
    {
        MainMenuManager.Instance.ActivateStartGameButton(GetComponent<IDeck>());
    }

    public void OnPointerUp(PointerEventData eventData)
    {

    }
}
