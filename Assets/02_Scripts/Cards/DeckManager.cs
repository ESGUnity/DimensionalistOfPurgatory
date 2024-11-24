using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public CardDataBase DataBase;
    int[] imsiIndex = { 10001, 10002, 10003, 20001, 20002, 20003 };
    static DeckManager instance;
    public static DeckManager Instance { get { return instance; } }
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // ���� �ٲ� �ı����� �ʴ� ����Ʈ���̶�� ������ ���� ������Ʈ�� �����ϴ� ��!
        }
    }

    public List<CardData> GetPlayerDeck()
    {
        // json ��ȣȭ �� �ڵ尡 �� ����
        List<CardData> deck = new List<CardData>();
        foreach (int i in imsiIndex)
        {
            CardData origin = DataBase.CardDataList.Find(n => n.Id == i);
            CardData clone = (CardData)origin.Clone();
            deck.Add(clone);
        }
        return deck;
    }

    public List<CardData> GetOpponentAIDeck()
    {
        List<CardData> deck = new List<CardData>();
        // f���� �޴������� AI ���� �� ���� ��û
        // deck�� ä���ֱ�
        return deck;
    }
}
