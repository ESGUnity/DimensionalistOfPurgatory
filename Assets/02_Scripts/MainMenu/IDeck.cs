using UnityEngine;

public interface IDeck
{
    int DeckNumber { get; set; }
    string DeckName { get; set; }
    int DeckCount { get; set; }

    public void SaveDeckInfo();

    public void SetDeckInfo();
}
