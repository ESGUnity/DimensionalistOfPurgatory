using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public CardDataBase DataBase;


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
    public void SetDeckNumber()
    {

    }
    public List<CardData> GetPlayerDeck()
    {
        string fileName = $"MyDeck{1}";
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

    public List<CardData> GetOpponentAIDeck()
    {
        List<CardData> deck = new List<CardData>();
        // f���� �޴������� AI ���� �� ���� ��û
        // deck�� ä���ֱ�
        return deck;
    }
}
