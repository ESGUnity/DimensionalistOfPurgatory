using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    List<CardData> originPlayerDeck;
    List<CardData> originOpponentDeck;

    List<CardData> playerDeck = new();
    List<CardData> opponentDeck = new();

    private static CardManager instance;
    public static CardManager Instance { get { return instance; } }
    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        originPlayerDeck = DeckManager.Instance.GetPlayerDeck();
        originOpponentDeck = DeckManager.Instance.GetOpponentDeck();
    }

    public void FillDeck(string playerTag) // 태그에 따라 덱을 채워주는 메서드
    {
        if (playerTag == "Player")
        {
            foreach (CardData cardData in originPlayerDeck)
            {
                CardData clone = (CardData)cardData.Clone(); // 안전성을 위해 Clone()으로 원본 덱의 완전한 복사본을 만들어, 그 복사본을 런타임 중 사용한다.
                playerDeck.Add(clone);
            }
            ShuffleDeck(playerDeck);
        }
        else if (playerTag == "Opponent")
        {
            foreach (CardData cardData in originOpponentDeck)
            {
                CardData clone = (CardData)cardData.Clone();
                opponentDeck.Add(clone);
            }
            ShuffleDeck(opponentDeck);
        }
    }
    public CardData DrawCard(string playerTag) // 태그에 따라 덱에서 카드를 뽑아주는 메서드
    {
        if (playerTag == "Player")
        {
            if (playerDeck[0] != null)
            {
                CardData drawCard = playerDeck[0];
                playerDeck.RemoveAt(0);
                return drawCard;
            }
        }
        else if (playerTag == "Opponent")
        {
            if (opponentDeck[0] != null)
            {
                CardData drawCard = opponentDeck[0];
                opponentDeck.RemoveAt(0);
                return drawCard;
            }
        }

        return null;
    }
    public int CountDeck(string playerTag) // 덱의 남은 카드 수를 반환하는 메서드
    {
        if (playerTag == "Player")
        {
            return playerDeck.Count;
        }
        else if (playerTag == "Opponent")
        {
            return opponentDeck.Count;
        }

        return default;
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
}
