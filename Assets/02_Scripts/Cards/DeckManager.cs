using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public CardDataBase DataBase;
    public int playerDeckNumber;
    public int opponentDeckNumber;

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
        InitializeAIDeck();
        opponentDeckNumber = 1;
    }
    public void SetPlayerDeckNumber(int deckNumber)
    {
        playerDeckNumber = deckNumber;
    }
    public void SetOpponentDeckNumber(int deckNumber)
    {
        opponentDeckNumber = deckNumber;
    }
    public List<CardData> GetPlayerDeck()
    {
        string fileName = $"MyDeck{playerDeckNumber}";

        string path = Path.Combine(Application.persistentDataPath, fileName);
        string json = File.ReadAllText(path);
        HashSet<int> cardsId = JsonConvert.DeserializeObject<HashSet<int>>(json);

        List<CardData> deck = new();

        foreach (int i in cardsId)
        {
            CardData origin = DataBase.CardDataList.Find(n => n.Id == i);
            CardData clone = (CardData)origin.Clone();
            deck.Add(clone);
        }
        return deck;
    }

    public List<CardData> GetOpponentDeck() // AI���̴�
    {
        string fileName = $"MyDeck{playerDeckNumber}";
        Debug.Log(opponentDeckNumber);
        //string fileName = $"OpponentDeck{opponentDeckNumber}";
        //string fileName = $"MyDeck{1}";

        string path = Path.Combine(Application.persistentDataPath, fileName);
        string json = File.ReadAllText(path);
        HashSet<int> cardsId = JsonConvert.DeserializeObject<HashSet<int>>(json);

        List<CardData> deck = new();

        foreach (int i in cardsId)
        {
            CardData origin = DataBase.CardDataList.Find(n => n.Id == i);
            CardData clone = (CardData)origin.Clone();
            deck.Add(clone);
        }
        return deck;
    }
    public void InitializeAIDeck()
    {
        for (int i = 1; i <= 4; i++)
        {
            string fileName = $"OpponentDeck{i}";
            string path = Path.Combine(Application.persistentDataPath, fileName);

            List<int> temList = new List<int>() { 10001, 10002, 10003, 20001, 20002, 30001, 30002, 30003, 40001, 40002, 40003, 50001, 50002, 70001 };

            HashSet<int> tempHashSet = new(temList);

            string json = JsonConvert.SerializeObject(tempHashSet);
            File.WriteAllText(path, json); // ù �����̾ ������ ���ٸ� ���� ����.
        }
    }
}
