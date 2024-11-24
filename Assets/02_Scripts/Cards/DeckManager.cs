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
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 파괴되지 않는 돈디스트로이라는 공간에 게임 오브젝트를 저장하는 것!
        }
    }

    public List<CardData> GetPlayerDeck()
    {
        // json 복호화 등 코드가 들어갈 예정
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
        // f레벨 메니저에게 AI 적의 덱 정보 요청
        // deck에 채워넣기
        return deck;
    }
}
