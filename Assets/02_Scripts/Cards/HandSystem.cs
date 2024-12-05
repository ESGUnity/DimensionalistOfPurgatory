using System;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HandSystem : MonoBehaviour
{
    public int HandCount;
    public int PledgeHandCount;

    [SerializeField] GameObject astralPrefab;
    [SerializeField] GameObject prayPrefab;

    [SerializeField] RectTransform cardSpawnPos;
    [SerializeField] RectTransform handPos;
    [SerializeField] RectTransform swapPos;
    [SerializeField] RectTransform cardPosA;
    [SerializeField] RectTransform cardPosB;
    [SerializeField] RectTransform cardPosC;
    [SerializeField] RectTransform cardPosD;
    [SerializeField] RectTransform cardPosE;
    [SerializeField] GameObject preparationHand;

    [SerializeField] RectTransform prayCardPosA;
    [SerializeField] RectTransform prayCardPosB;
    [SerializeField] RectTransform prayCardPosC;
    [SerializeField] GameObject BattleHand;

    [SerializeField] TextMeshProUGUI remainCardsInDeck;

    Dictionary<RectTransform, GameObject> cardPosOnHand = new();
    Dictionary<RectTransform, GameObject> prayCardPosOnHand = new();
    string thisPlayerTag;
    Vector3 sizeWhenPressed = new Vector3(0.5f, 0.5f, 0.5f);

    private void Awake()
    {
        HandCount = 0; // ���� �� ���� ���� �ʱ�ȭ
        PledgeHandCount = 0;
        thisPlayerTag = gameObject.tag; // PlayerObject�� �޸��� ������Ʈ�⿡ gameObject�� tag�� �ٷ� �Ҵ��� �� �ִ�.

        cardPosOnHand[cardPosA] = null;
        cardPosOnHand[cardPosB] = null;
        cardPosOnHand[cardPosC] = null;
        cardPosOnHand[cardPosD] = null;
        cardPosOnHand[cardPosE] = null;

        prayCardPosOnHand[prayCardPosA] = null;
        prayCardPosOnHand[prayCardPosB] = null;
        prayCardPosOnHand[prayCardPosC] = null;
    }
    private void OnEnable()
    {
        PhaseManager.Instance.OnAfterPreparation += SwapHandToBattle;
        PhaseManager.Instance.OnAfterBattle += SwapHandToPreparation;
    }
    private void OnDisable()
    {
        PhaseManager.Instance.OnAfterPreparation -= SwapHandToBattle;
        PhaseManager.Instance.OnAfterBattle -= SwapHandToPreparation;
    }
    void Start()
    {
        CardManager.Instance.FillDeck(thisPlayerTag);
    }

    void Update()
    {
        if (CardManager.Instance.CountDeck(thisPlayerTag) <= 0 && !(PhaseManager.Instance.CurrentPhase == Phase.Preparation))  
        {
            CardManager.Instance.FillDeck(thisPlayerTag);
        }

        ImportDrawCardToHand();

        if (PhaseManager.Instance.CurrentPhase == Phase.AfterPreparation || PhaseManager.Instance.CurrentPhase == Phase.AfterBattle) // �غ� �� ���� �ܰ踦 ����� ī�� �巡�׸� ���߱�
        {
            GetComponent<PlacementSystem>().StopAstralPlacement();
            GetComponent<PlacementSystem>().StopPrayPledge();
            // StopPrayPlacement�� �� ����!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        }
        else
        {
            PutOutCardInPreparation();
            PutOutCardInBattle();
        }

        remainCardsInDeck.text = $"{CardManager.Instance.CountDeck(thisPlayerTag)} / 25";
    }

    
    void ImportDrawCardToHand() // ���а� 5�� �̸��� ��, CardManager�� ������ ī�带 �̾� ������ �������� �޼���
    {
        if (HandCount < 5 && CardManager.Instance.CountDeck(thisPlayerTag) > 0 && PhaseManager.Instance.CurrentPhase == Phase.Preparation)
        {
            CardData drawCard = CardManager.Instance.DrawCard(thisPlayerTag); // ������ ī�� ��ο�

            if (drawCard != null) // null�� ���� �� ������ �������� ���� ��ġ.
            {
                GameObject go = drawCard.IsAstral ? Instantiate(astralPrefab) : Instantiate(prayPrefab); // ī�尡 ��ü���� �⵵���� Ȯ���ϰ� �� Ÿ�Կ� �´� �������� ����.
                go.GetComponent<InteractableCard>().SetupCardData(drawCard);

                foreach (RectTransform key in cardPosOnHand.Keys)
                {
                    if (cardPosOnHand[key] == null)
                    {
                        cardPosOnHand[key] = go;
                        go.transform.position = cardSpawnPos.position;
                        go.transform.SetParent(key);
                        go.GetComponent<InteractableCard>().SetOriginPosition(key.position);
                        go.GetComponent<InteractableCard>().BackToOriginTransform();
                        break;
                    }
                }

                HandCount++;
            }
        }
    }
    void PutOutCardInPreparation() // �տ� �ִ� ī�带 ���� �� ����ϴ� �޼���
    {
        if (PhaseManager.Instance.CurrentPhase == Phase.Preparation)
        {
            if (cardPosOnHand[cardPosA] != null && cardPosOnHand[cardPosA].GetComponent<InteractableCard>().isHolding)
            {
                ManageDragCard(cardPosOnHand[cardPosA]);
            }
            if (cardPosOnHand[cardPosB] != null && cardPosOnHand[cardPosB].GetComponent<InteractableCard>().isHolding)
            {
                ManageDragCard(cardPosOnHand[cardPosB]);
            }
            if (cardPosOnHand[cardPosC] != null && cardPosOnHand[cardPosC].GetComponent<InteractableCard>().isHolding)
            {
                ManageDragCard(cardPosOnHand[cardPosC]);
            }
            if (cardPosOnHand[cardPosD] != null && cardPosOnHand[cardPosD].GetComponent<InteractableCard>().isHolding)
            {
                ManageDragCard(cardPosOnHand[cardPosD]);
            }
            if (cardPosOnHand[cardPosE] != null && cardPosOnHand[cardPosE].GetComponent<InteractableCard>().isHolding)
            {
                ManageDragCard(cardPosOnHand[cardPosE]);
            }
        }
    }
    public void ImportPledgePrayToHand(CardData pledgePray) // �غ� �ܰ迡�� �⵵ �����ϴ� �޼���
    {
        if (PledgeHandCount < 3 && PhaseManager.Instance.CurrentPhase == Phase.Preparation)
        {
            GameObject go = Instantiate(prayPrefab); 
            go.GetComponent<InteractableCard>().SetupCardData(pledgePray);

            foreach (RectTransform key in prayCardPosOnHand.Keys)
            {
                if (prayCardPosOnHand[key] == null)
                {
                    prayCardPosOnHand[key] = go;
                    go.transform.position = swapPos.position;
                    go.transform.SetParent(key);
                    go.GetComponent<InteractableCard>().SetOriginPosition(key.position);
                    go.GetComponent<InteractableCard>().BackToOriginTransform();
                    break;
                }
            }

            PledgeHandCount++;
        }
        else
        {
            Debug.Log("�⵵�� ����ϴ� �ܰ谡 �ƴմϴ�.");
            // ���� �˷��ִ� UI 
        }
    }
    void PutOutCardInBattle() // ������ �⵵�� ���� �� ����ϴ� �޼���
    {
        if (PhaseManager.Instance.CurrentPhase == Phase.Battle)
        {
            if (prayCardPosOnHand[prayCardPosA] != null && prayCardPosOnHand[prayCardPosA].GetComponent<InteractableCard>().isHolding)
            {
                ManageDragCard(prayCardPosOnHand[prayCardPosA]);
            }
            if (prayCardPosOnHand[prayCardPosB] != null && prayCardPosOnHand[prayCardPosB].GetComponent<InteractableCard>().isHolding)
            {
                ManageDragCard(prayCardPosOnHand[prayCardPosB]);
            }
            if (prayCardPosOnHand[prayCardPosC] != null && prayCardPosOnHand[prayCardPosC].GetComponent<InteractableCard>().isHolding)
            {
                ManageDragCard(prayCardPosOnHand[prayCardPosC]);
            }
        }
    }
    void ManageDragCard(GameObject cardPrefab)// Player �� Opponent ���� �޼���. OpponentAI�� ������� �ʴ´�.(�������� ġȯ ����� �ѵ�..)
    {
        cardPrefab.transform.position = GetComponent<InputSystem>().GetMousePositionOnScreen(); // Holding ���̶�� ī�� �������� ���콺�� ��ġ�� ����ٴϴ� �ڵ�

        if (GetComponent<InputSystem>().CanPlacement())
        {
            MakeTransParent(cardPrefab);

            if (!cardPrefab.GetComponent<InteractableCard>().isStartPlacement)
            {
                GetComponent<PlacementSystem>().StartPlacement(cardPrefab.GetComponent<InteractableCard>().cardData.Id, cardPrefab);
                cardPrefab.GetComponent<InteractableCard>().isStartPlacement = true;
            }
        }
        else
        {
            MakeOpaque(cardPrefab);
            cardPrefab.transform.localScale = sizeWhenPressed;
        }
    }
    void MakeTransParent(GameObject cardPrefab)
    {
        Color colorWhite = Color.white;
        colorWhite.a = 0;

        cardPrefab.GetComponent<Image>().color = colorWhite;

        Image[] images = cardPrefab.GetComponentsInChildren<Image>();
        foreach (Image image in images)
        {
            image.color = colorWhite;
        }

        TMP_Text[] texts = cardPrefab.GetComponentsInChildren<TMP_Text>();
        foreach (TMP_Text text in texts)
        {
            text.enabled = false;
        }
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

    public void SwapHandToPreparation()
    {
        StartCoroutine(SwapPreparation());
    }
    public void SwapHandToBattle()
    {
        StartCoroutine(SwapBattle());
    }

    IEnumerator SwapPreparation()
    {
        BattleHand.transform.DOMove(swapPos.position, 0.5f);
        yield return new WaitForSeconds(0.5f);
        preparationHand.transform.DOMove(handPos.position, 0.5f);

        preparationHand.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        BattleHand.gameObject.SetActive(false);

    }
    IEnumerator SwapBattle()
    {
        preparationHand.transform.DOMove(swapPos.position, 0.5f);
        yield return new WaitForSeconds(0.5f);
        BattleHand.transform.DOMove(handPos.position, 0.5f);

        BattleHand.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        preparationHand.gameObject.SetActive(false);
    }
}
