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
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 파괴되지 않는 돈디스트로이라는 공간에 게임 오브젝트를 저장하는 것!
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
        // f레벨 메니저에게 AI 적의 덱 정보 요청
        // deck에 채워넣기
        return deck;
    }
}
