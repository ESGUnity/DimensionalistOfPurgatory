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
        if (isHolding) // �巡�� ���̶��
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
    void RunStartPlacement() // ī�带 �巡����ä�� Field���� Ŀ���� �ö󰡸�, PlacenebtSystem�� ��ġ�� �����ϴ� �Լ��� �����ϴ� �޼���
    {
        if (InputSystem.Instance.CanPlacemet() && !isStartPlacement)
        {
            PlacementSystem.Instance.StartPlacement(cardData.Id, gameObject); // 10001 �ڸ��� �����տ� �ִ� Id��.
            isStartPlacement = true;
        }
    }
    void DragWithAdjustSize() // �巡�׿� ���� ī�� �������� ũ�� ���� ���
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
        transform.parent.SetAsLastSibling(); // ���̶�Ű ���� ������Ʈ ������ �ٲپ� �巡�� ���� UI�� �׻� ���� ���� ��ġ�ϵ��� ����.
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