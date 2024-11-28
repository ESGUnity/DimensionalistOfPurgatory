using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DeckBuildInteractableCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    // Pointer로 Holding 시 input.mousePosition으로 따라다니게 만들기
    // Ray 쏴서 Deck Layer를 만났을때 PointerUp하면 덱에 포함되게 만들기
    // 덱 전용 프리팹 만들어서 카드 정보 받아서 덱 전용 프리팹으로 변환하기(StartPlacement처럼 레이가 덱이면 덱전용, 레이가 덱 아니면 카드로
    public TextMeshProUGUI Name, Cost, Health, Mana, Damage, Range, Ability;
    public Image Thumbnail;
    public Image ExistImage;
    public bool IsViewOnly = false;

    [HideInInspector] public CardData cardData;
    [HideInInspector] public bool isHolding = false;


    private void Update()
    {
        if (DeckBuildManager.Instance.CheckDuplicate(cardData))
        {
            ExistImage.gameObject.SetActive(true);
        }
        else
        {
            ExistImage.gameObject.SetActive(false);
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {

    }
    public void OnPointerExit(PointerEventData eventData)
    {

    }

    public virtual void SetupCardData(CardData cardData)
    {

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!DeckBuildManager.Instance.CheckDuplicate(cardData) && !IsViewOnly)
        {
            DeckBuildManager.Instance.StartRepositionCard(gameObject, cardData);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!DeckBuildManager.Instance.CheckDuplicate(cardData) && !IsViewOnly)
        {
            DeckBuildManager.Instance.OnRepositionCard();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!DeckBuildManager.Instance.CheckDuplicate(cardData) && !IsViewOnly)
        {
            DeckBuildManager.Instance.StopRepositionCard(gameObject);
        }
    }
}
