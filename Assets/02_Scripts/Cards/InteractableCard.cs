using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InteractableCard : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI Name, Cost, Health, Mana, Damage, Range, Ability;
    public Image Thumbnail;

    [HideInInspector] public CardData cardData;
    [HideInInspector] public bool isHolding = false;
    [HideInInspector] public bool isOver = false;
    [HideInInspector] public bool isStartPlacement = false;

    Vector3 OriginPosition;
    Vector3 OriginScale = new Vector3(0.8f, 0.8f, 0.8f);

    private void Update()
    {
        if (PhaseManager.Instance.CurrentPhase == Phase.AfterPreparation || PhaseManager.Instance.CurrentPhase == Phase.AfterBattle) // �غ� �� ���� �ܰ踦 ����� ī�� �巡�׸� ���߱�
        {
            isHolding = false;
            isStartPlacement = false;
            BackToOriginTransform();
        }

        else if (isHolding) // �巡�� ���̶��
        {
            transform.parent.SetAsLastSibling(); // ���̶�Ű ���� ������Ʈ ������ �ٲپ� �巡�� ���� UI�� �׻� ���� ���� ��ġ�ϵ��� ����
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
        isOver = true;
        transform.DOScale(OriginScale * 1.3f, 0.3f);
        transform.DOLocalMove(new Vector3(0, 20, 0), 0.3f);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isHolding) //  || PhaseManager.Instance.CurrentPhase == Phase.AfterPreparation || PhaseManager.Instance.CurrentPhase == Phase.AfterBattle
        {
            BackToOriginTransform();
        }
    }

    public void SetOriginPosition(Vector3 position)
    {
        OriginPosition = position;
    }
    public void BackToOriginTransform()
    {
        transform.DOLocalMove(Vector3.zero, 0.3f);
        transform.DOScale(OriginScale, 0.3f);
        MakeOpaque(gameObject);

    }
    public void DestroyCardPrefab()
    {
        transform.DOKill();
        DestroyImmediate(gameObject); // ��� �ı��ϱ� ������ ���� ���� �ذ���� ã��.
    }

    public virtual void SetupCardData(CardData cardData)
    {

    }
    void MakeOpaque(GameObject cardPrefab)
    {
        Color colorWhite = Color.white;
        colorWhite.a = 1;

        cardPrefab.GetComponent<Image>().color = colorWhite;

        Image[] images = cardPrefab.GetComponentsInChildren<Image>();
        foreach (Image image in images)
        {
            image.color = colorWhite;
        }

        TextMeshProUGUI[] texts = cardPrefab.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (TextMeshProUGUI text in texts)
        {
            text.enabled = true;
        }
    }
}