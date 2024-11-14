using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CardSystem : MonoBehaviour
{
    public GameObject CardPrefab;
    public RectTransform CardPosA;
    public RectTransform CardPosB;
    public RectTransform CardPosC;
    public RectTransform CardPosD;
    public RectTransform CardPosE;
    public RectTransform SpawnPoint;
    public List<CardData> PlayerDeck;
    public List<CardData> OpponentrDeck;
    public Dictionary<RectTransform, bool> cardPosOnInventory = new();

    public int inventoryCount;
    int deckCount;
    string tagOfSide;
    List<CardData> consumeDeck = new();
    private static CardSystem instance;
    public static CardSystem Instance
    {
        get { return instance; }
    }
    private void Awake()
    {
        instance = this;
        tagOfSide = gameObject.tag;
        cardPosOnInventory[CardPosA] = false;
        cardPosOnInventory[CardPosB] = false;
        cardPosOnInventory[CardPosC] = false;
        cardPosOnInventory[CardPosD] = false;
        cardPosOnInventory[CardPosE] = false;
    }
    void Start()
    {
        PlayerDeck = DeckManager.Instance.GetPlayerDeck();
        FillDeck();
        DrawCard();
        DrawCard();
        DrawCard();
    }

    void Update()
    {
        
    }

    void FillDeck()
    {
        foreach (CardData cardData in PlayerDeck)
        {
            CardData clone = (CardData)cardData.Clone();
            consumeDeck.Add(clone);
        }
        ShuffleDeck(consumeDeck);
    }
    void ShuffleDeck(List<CardData> cards) // 덱을 무작위로 섞어주는 메서드
    {
        for (int i = 0; i < cards.Count; i++)
        {
            int randomIndex = Random.Range(0, cards.Count);
            CardData temp = cards[i];
            cards[i] = cards[randomIndex];
            cards[randomIndex] = temp;
        }
    }
    void DrawCard()
    {
        CardData drawCard = consumeDeck[0];
        consumeDeck.RemoveAt(0); // 카드 뽑기

        GameObject go = Instantiate(CardPrefab);
        go.GetComponent<AstralCard>().SetCardData(drawCard);
        foreach (RectTransform key in cardPosOnInventory.Keys)
        {
            if (!cardPosOnInventory[key])
            {
                cardPosOnInventory[key] = true;
                go.transform.position = SpawnPoint.position;
                go.transform.SetParent(key);
                go.GetComponent<AstralCard>().SetOriginPosition(key.position);
                go.GetComponent<AstralCard>().BackToOriginTransform();
                break;
            }
        }
        inventoryCount++;
    }
}
