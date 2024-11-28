using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DeckBuildInteractableCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    // Pointer�� Holding �� input.mousePosition���� ����ٴϰ� �����
    // Ray ���� Deck Layer�� �������� PointerUp�ϸ� ���� ���Եǰ� �����
    // �� ���� ������ ���� ī�� ���� �޾Ƽ� �� ���� ���������� ��ȯ�ϱ�(StartPlacementó�� ���̰� ���̸� ������, ���̰� �� �ƴϸ� ī���
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
