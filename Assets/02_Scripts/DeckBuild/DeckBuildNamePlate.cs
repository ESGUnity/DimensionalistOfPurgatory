using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DeckBuildNamePlate : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] TextMeshProUGUI essenceCostText;
    [SerializeField] TextMeshProUGUI cardNameText;

    [HideInInspector] public CardData cardData;
    int essenceCost;
    string cardName;

    public void OnPointerEnter(PointerEventData eventData)
    {

    }

    public void OnPointerExit(PointerEventData eventData)
    {

    }


    public void SetCardInfo(CardData cardData)
    {
        this.cardData = cardData;
        essenceCost = cardData.Cost;
        cardName = cardData.Name;

        essenceCostText.text = essenceCost.ToString();
        cardNameText.text = cardName;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (DeckBuildManager.Instance.CheckDuplicate(cardData))
        {
            DeckBuildManager.Instance.StartRepositionCard(gameObject, cardData);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (DeckBuildManager.Instance.CheckDuplicate(cardData))
        {
            DeckBuildManager.Instance.OnRepositionCard();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (DeckBuildManager.Instance.CheckDuplicate(cardData))
        {
            DeckBuildManager.Instance.StopRepositionCard(gameObject);
        }
    }
}
