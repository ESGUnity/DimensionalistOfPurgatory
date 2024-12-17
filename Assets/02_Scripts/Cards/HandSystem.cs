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

    [SerializeField] TextMeshProUGUI remainCardsInDeckText;
    [SerializeField] TextMeshProUGUI deckFillNotifierText;
    [SerializeField] GameObject bringerDialogue;


    Dictionary<RectTransform, GameObject> cardPosOnHand = new();
    Dictionary<RectTransform, GameObject> prayCardPosOnHand = new();
    string thisPlayerTag;
    Vector3 sizeWhenPressed = new Vector3(0.5f, 0.5f, 0.5f);
    Coroutine deckFillNotifierCoroutine;

    private void Awake()
    {
        HandCount = 0; // 시작 시 손패 개수 초기화
        PledgeHandCount = 0;
        thisPlayerTag = gameObject.tag; // PlayerObject에 달리는 컴포넌트기에 gameObject의 tag를 바로 할당할 수 있다.

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
        PhaseManager.Instance.OnPreparation += SwapHandToPreparation;
    }
    private void OnDisable()
    {
        PhaseManager.Instance.OnAfterPreparation -= SwapHandToBattle;
        PhaseManager.Instance.OnPreparation -= SwapHandToPreparation;
    }
    void Start()
    {
        CardManager.Instance.FillDeck(thisPlayerTag);
    }

    void Update()
    {
        if (CardManager.Instance.CountDeck(thisPlayerTag) <= 0)  // && !(PhaseManager.Instance.CurrentPhase == Phase.Preparation)
        {
            CardManager.Instance.FillDeck(thisPlayerTag); 

            if (deckFillNotifierCoroutine == null) // 덱을 채울 때마다 알려준다.
            {
                deckFillNotifierCoroutine = StartCoroutine(NotifyDeckFilled());
            }
            else
            {
                StopCoroutine(deckFillNotifierCoroutine);
                deckFillNotifierCoroutine = StartCoroutine(NotifyDeckFilled());
            }
        }

        ImportDrawCardToHand();

        if (PhaseManager.Instance.CurrentPhase == Phase.AfterPreparation || PhaseManager.Instance.CurrentPhase == Phase.AfterBattle) // 준비 및 전투 단계를 벗어나면 카드 드래그를 멈추기
        {
            GetComponent<PlacementSystem>().StopAstralPlacement();
            GetComponent<PlacementSystem>().StopAstralReplacement();
            GetComponent<PlacementSystem>().StopPrayPledge();
            GetComponent<PlacementSystem>().StopPrayCast();
        }
        else
        {
            PutOutCardInPreparation();
            PutOutCardInBattle();
        }

        remainCardsInDeckText.text = $"{CardManager.Instance.CountDeck(thisPlayerTag)} / 25"; // 25로 바꾸자 제발
    }

    
    void ImportDrawCardToHand() // 손패가 5장 미만일 때, CardManager로 덱에서 카드를 뽑아 손으로 가져오는 메서드
    {
        if (HandCount < 5 && CardManager.Instance.CountDeck(thisPlayerTag) > 0 && PhaseManager.Instance.CurrentPhase == Phase.Preparation)
        {
            CardData drawCard = CardManager.Instance.DrawCard(thisPlayerTag); // 덱에서 카드 드로우

            if (drawCard != null) // null이 나올 리 없지만 안정성을 위한 장치.
            {
                GameObject go = drawCard.IsAstral ? Instantiate(astralPrefab) : Instantiate(prayPrefab); // 카드가 영체인지 기도인지 확인하고 각 타입에 맞는 프리팹을 생성.
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
    void PutOutCardInPreparation() // 손에 있는 카드를 내는 걸 담당하는 메서드
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
    public void ImportPledgePrayToHand(CardData pledgePray) // 준비 단계에서 기도 서약하는 메서드
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
            if (cannotPrayCoroutine == null)
            {
                cannotPrayCoroutine = StartCoroutine(BringerDialogueCannotPray());
            }
            else
            {
                StopCoroutine(cannotPrayCoroutine);
                cannotPrayCoroutine = StartCoroutine(BringerDialogueCannotPray());
            }
            return;
        }
    }
    void PutOutCardInBattle() // 서약한 기도를 내는 걸 담당하는 메서드
    {
        if (PhaseManager.Instance.CurrentPhase == Phase.Battle && !PhaseManager.Instance.SealPray()) // 기도 봉인이 false 여야 기도 시전을 할 수 있다.
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
    void ManageDragCard(GameObject cardPrefab)
    {
        if (PhaseManager.Instance.CurrentPhase == Phase.Battle)
        {
            if (PhaseManager.Instance.SealPray())
            {
                Debug.Log("흠르르");
                if (cannotCastPrayCoroutine == null)
                {
                    cannotCastPrayCoroutine = StartCoroutine(BringerDialogueCannotCast());
                }
                else
                {
                    StopCoroutine(cannotCastPrayCoroutine);
                    cannotCastPrayCoroutine = StartCoroutine(BringerDialogueCannotCast());
                }
                return;
            }

            cardPrefab.transform.position = GetComponent<InputSystem>().GetMousePositionOnScreen(); // Holding 중이라면 카드 프리팹이 마우스의 위치를 따라다니는 코드

            if (GetComponent<InputSystem>().CanPlacement())
            {
                MakeTransParent(cardPrefab);

                if (!cardPrefab.GetComponent<InteractableCard>().isStartPlacement)
                {
                    GetComponent<PlacementSystem>().StartPrayCast(cardPrefab.GetComponent<InteractableCard>().cardData.Id, cardPrefab);
                    cardPrefab.GetComponent<InteractableCard>().isStartPlacement = true;
                }
            }
            else
            {
                MakeOpaque(cardPrefab);
                cardPrefab.transform.localScale = sizeWhenPressed;
            }
        }
        else if (PhaseManager.Instance.CurrentPhase == Phase.Preparation) // 준비 단계라면 카드의 에센스에 따라서 카드를 낼 수 없게 만든다.
        {
            if (cardPrefab.GetComponent<InteractableCard>().cardData.Cost > GetComponent<BringerSystem>().CurrentEssence)
            {
                if (lackCoroutine == null)
                {
                    lackCoroutine = StartCoroutine(BringerDialogueEssenceLack());
                }
                else
                {
                    StopCoroutine(lackCoroutine);
                    lackCoroutine = StartCoroutine(BringerDialogueEssenceLack());
                }
                return;
            }

            if (cardPrefab.GetComponent<InteractableCard>().cardData.Cost > GetComponent<BringerSystem>().LimitEssence)
            {
                if (limitCoroutine == null)
                {
                    limitCoroutine = StartCoroutine(BringerDialogueEssenceLimit());
                }
                else
                {
                    StopCoroutine(limitCoroutine);
                    limitCoroutine = StartCoroutine(BringerDialogueEssenceLimit());
                }
                return;
            }
            if (!cardPrefab.GetComponent<InteractableCard>().cardData.IsAstral && PledgeHandCount >= 3)
            {
                if (cannotPrayCoroutine == null)
                {
                    cannotPrayCoroutine = StartCoroutine(BringerDialogueCannotPray());
                }
                else
                {
                    StopCoroutine(cannotPrayCoroutine);
                    cannotPrayCoroutine = StartCoroutine(BringerDialogueCannotPray());
                }
                return;
            }

            cardPrefab.transform.position = GetComponent<InputSystem>().GetMousePositionOnScreen(); // Holding 중이라면 카드 프리팹이 마우스의 위치를 따라다니는 코드

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
    IEnumerator NotifyDeckFilled()
    {
        deckFillNotifierText.gameObject.SetActive(true);

        yield return new WaitForSeconds(3f);

        deckFillNotifierText.gameObject.SetActive(false);
    }

    Coroutine limitCoroutine;
    IEnumerator BringerDialogueEssenceLimit()
    {
        bringerDialogue.SetActive(true);
        bringerDialogue.GetComponentInChildren<TextMeshProUGUI>().text = "에센스 한계치야";
        yield return new WaitForSeconds(3f);
        bringerDialogue.SetActive(false);
    }
    Coroutine lackCoroutine;
    IEnumerator BringerDialogueEssenceLack()
    {
        bringerDialogue.SetActive(true);
        bringerDialogue.GetComponentInChildren<TextMeshProUGUI>().text = "에센스가 부족해";
        yield return new WaitForSeconds(3f);
        bringerDialogue.SetActive(false);
    }
    Coroutine cannotPrayCoroutine;
    IEnumerator BringerDialogueCannotPray()
    {
        bringerDialogue.SetActive(true);
        bringerDialogue.GetComponentInChildren<TextMeshProUGUI>().text = "이미 3장의 기도를 서약했어";
        yield return new WaitForSeconds(3f);
        bringerDialogue.SetActive(false);
    }
    Coroutine cannotCastPrayCoroutine;
    IEnumerator BringerDialogueCannotCast()
    {
        bringerDialogue.SetActive(true);
        bringerDialogue.GetComponentInChildren<TextMeshProUGUI>().text = "기도를 시전할 수 없어";
        yield return new WaitForSeconds(3f);
        bringerDialogue.SetActive(false);
    }
}
