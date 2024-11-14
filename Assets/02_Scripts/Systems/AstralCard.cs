using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AstralCard : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Vector3 OriginPosition;
    public Vector3 OriginScale;
    public TextMeshProUGUI Name, Cost, Health, Mana, Damage, Range, Ability;
    public Image Thumbnail;

    CardData cardData;
    bool isHolding = false;
    bool isStartPlacement = false;

    private void Awake()
    {
        OriginScale = new Vector3(0.8f, 0.8f, 0.8f);
    }
    private void Update()
    {
        if (isHolding) // 드래그 중이라면
        {
            RunStartPlacement(); 
            DragWithAdjustSize();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isHolding = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isHolding = false;
        isStartPlacement = false;
        BackToOriginTransform();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = OriginScale * 1.2f;
        transform.position += new Vector3(0, 10, 0);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        BackToOriginTransform();
    }
    public void SetOriginPosition(Vector3 position)
    {
        OriginPosition = position;
    }
    void RunStartPlacement() // 카드를 드래그한채로 Field위에 커서가 올라가면, PlacenebtSystem의 배치를 시작하는 함수를 실행하는 메서드
    {
        if (InputSystem.Instance.CanPlacemet() && !isStartPlacement)
        {
            PlacementSystem.Instance.StartPlacement(cardData.Id, gameObject); // 10001 자리엔 프리팹에 있는 Id로.
            isStartPlacement = true;
        }
    }
    void DragWithAdjustSize() // 드래그에 따른 카드 프리팹의 크기 조절 담당
    {
        transform.position = InputSystem.Instance.GetMousePositionOnScreen();
        if (InputSystem.Instance.CanPlacemet())
        {
            transform.localScale = OriginScale * 0f;
        }
        else
        {
            transform.localScale = OriginScale * 0.7f;
        }
        transform.parent.SetAsLastSibling(); // 하이라키 상의 오브젝트 순서를 바꾸어 드래그 중인 UI가 항상 제일 위에 위치하도록 설정.
    }
    public void BackToOriginTransform()
    {
        transform.DOMove(OriginPosition, 0.5f);
        transform.DOScale(OriginScale, 0.5f);
    }
    void DestroyThisIfCompletePlacement()
    {
        CardSystem.Instance.inventoryCount--;
        DOTween.Kill(transform);
        Destroy(gameObject);
    }
    public void SetCardData(CardData cardData)
    {
        this.cardData = cardData;
        Name.text = cardData.Name;
        Cost.text = cardData.Cost.ToString();
        Thumbnail.sprite = cardData.Thumbnail;
        Health.text = cardData.Health.ToString();
        if (cardData.Mana != 0)
        {
            Mana.text = cardData.Mana.ToString();
        }
        else
        {
            Mana.text = "X";
        }
        Damage.text = cardData.Damage.ToString();
        Range.text = cardData.Range.ToString();
        Ability.text = cardData.Ability;
    }
}