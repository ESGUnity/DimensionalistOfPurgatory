using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    List<CardData> originPlayerDeck;
    List<CardData> originOpponentAIDeck;
    List<CardData> originOpponentDeck;
    List<CardData> playerDeck = new();
    List<CardData> opponentAIDeck = new();
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
        originOpponentAIDeck = DeckManager.Instance.GetOpponentAIDeck();
    }

    public void FillDeck(string playerTag) // �±׿� ���� ���� ä���ִ� �޼���
    {
        if (playerTag == "Player")
        {
            foreach (CardData cardData in originPlayerDeck)
            {
                CardData clone = (CardData)cardData.Clone(); // �������� ���� Clone()���� ���� ���� ������ ���纻�� �����, �� ���纻�� ��Ÿ�� �� ����Ѵ�.
                playerDeck.Add(clone);
            }
            ShuffleDeck(playerDeck);
        }
        else if (playerTag == "OpponentAI")
        {
            foreach (CardData cardData in originOpponentAIDeck)
            {
                CardData clone = (CardData)cardData.Clone();
                opponentAIDeck.Add(clone);
            }
            ShuffleDeck(opponentAIDeck);
        }
    }
    public CardData DrawCard(string playerTag) // �±׿� ���� ������ ī�带 �̾��ִ� �޼���
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
        else if (playerTag == "OpponentAI")
        {
            if (opponentAIDeck[0] != null)
            {
                CardData drawCard = opponentAIDeck[0];
                opponentAIDeck.RemoveAt(0);
                return drawCard;
            }
        }

        return null;
    }
    public int CountDeck(string playerTag) // ���� ���� ī�� ���� ��ȯ�ϴ� �޼���
    {
        if (playerTag == "Player")
        {
            return playerDeck.Count;
        }
        else if (playerTag == "OpponentAI")
        {
            return opponentAIDeck.Count;
        }

        return default;
    }
    void ShuffleDeck(List<CardData> cards) // ���� �������� �����ִ� �޼���
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
