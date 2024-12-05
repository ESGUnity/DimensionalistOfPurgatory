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

        if (PhaseManager.Instance.CurrentPhase == Phase.AfterPreparation || PhaseManager.Instance.CurrentPhase == Phase.AfterBattle) // 준비 및 전투 단계를 벗어나면 카드 드래그를 멈추기
        {
            GetComponent<PlacementSystem>().StopAstralPlacement();
            GetComponent<PlacementSystem>().StopPrayPledge();
            // StopPrayPlacement도 꼭 넣자!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        }
        else
        {
            PutOutCardInPreparation();
            PutOutCardInBattle();
        }

        remainCardsInDeck.text = $"{CardManager.Instance.CountDeck(thisPlayerTag)} / 25";
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
            Debug.Log("기도를 등록하는 단계가 아닙니다.");
            // 대충 알려주는 UI 
        }
    }
    void PutOutCardInBattle() // 서약한 기도를 내는 걸 담당하는 메서드
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
    void ManageDragCard(GameObject cardPrefab)// Player 및 Opponent 전용 메서드. OpponentAI는 사용하지 않는다.(리스코프 치환 위배긴 한데..)
    {
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
