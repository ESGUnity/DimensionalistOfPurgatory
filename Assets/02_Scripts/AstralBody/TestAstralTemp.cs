using System.Collections.Generic;
using UnityEngine;

public class TestAstralTemp : MonoBehaviour
{
    [SerializeField] GameObject astralPrefab;

    List<CardData> originPlayerDeck;
    void Start()
    {
        originPlayerDeck = DeckManager.Instance.GetPlayerDeck();

        TempAstralSpawner();
    }


    void TempAstralSpawner()
    {
        GameObject go = Instantiate(astralPrefab);
        go.GetComponent<AstralBody>().SetAstralInfo(GridManager.Instance.GetGridPosFromWorldPos(new Vector3(-3.7f, 0, 3f)), originPlayerDeck[0], "Opponent");
        go.transform.position = GridManager.Instance.GetGridPosFromWorldPos(new Vector3(-3.7f, 0, 3f)).Coordinate;

        //GameObject go1 = Instantiate(astralPrefab);
        //go1.transform.position = GridManager.Instance.GetGridPosFromWorldPos(new Vector3(4.7f, 0, 2)).Coordinate;
        //go1.GetComponent<AstralBody>().SetAstralInfo(GridManager.Instance.GetGridPosFromWorldPos(new Vector3(4.7f, 0, 2)), originPlayerDeck[0], "Opponent");
    }
}
