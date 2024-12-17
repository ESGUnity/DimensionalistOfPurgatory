using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class Deck2 : MonoBehaviour, IDeck
{
    public int DeckNumber { get; set; } = 2;
    public string DeckName { get; set; } = "2�� ��";
    public int DeckCount { get; set; }
    public void SaveDeckInfo()
    {
        string fileName = $"MyDeck{DeckNumber}Info";
        string path = Path.Combine(Application.persistentDataPath, fileName);

        Dictionary<string, int> DeckInfo = new Dictionary<string, int>();
        DeckInfo[DeckName] = DeckCount;

        if (File.Exists(path))
        {
            string json = JsonConvert.SerializeObject(DeckInfo);
            File.WriteAllText(path, json);
        }
    }
    public void SetDeckInfo()
    {
        string fileName = $"MyDeck{DeckNumber}Info";
        string path = Path.Combine(Application.persistentDataPath, fileName);

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            Dictionary<string, int> DeckInfo = JsonConvert.DeserializeObject<Dictionary<string, int>>(json);
            DeckName = DeckInfo.Keys.First(); // ��ť
            DeckCount = DeckInfo[DeckName];
        }
        else
        {
            Dictionary<string, int> DeckInfo = new Dictionary<string, int>();
            DeckInfo[DeckName] = DeckCount;

            string json = JsonConvert.SerializeObject(DeckInfo);
            File.WriteAllText(path, json); // ù �����̾ ������ ���ٸ� ���� ����.
        }
    }
}
