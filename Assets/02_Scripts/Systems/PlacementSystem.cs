using System;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour // 현재로써는 두 명의 플레이어 배치를 담당할 수 없다. 한 플레이어의 배치에만 기능을 제공한다.
{
    public GameObject MouseIndicator;
    public GameObject GridIndicator;
    public List<CardData> OriginDeck; // 임시

    Vector3 mousePosition;
    Vector3 gridPosition;
    Vertex gridVertex;
    CardData selectedCardData;
    GameObject previewAstral;
    GameObject selectedCardPrefab;

    private static PlacementSystem instance;
    public static PlacementSystem Instance {  get { return instance; } }
    void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        OriginDeck = DeckManager.Instance.GetPlayerDeck(); // 임시
    }

    void Update()
    {
        mousePosition = InputSystem.Instance.GetMousePositionOnField();
        MouseIndicator.transform.position = mousePosition;
        gridPosition = (GridSystem.Instance.GetGridPosFromWorldPos(mousePosition)).Coordinate;
        gridVertex = GridSystem.Instance.GetGridPosFromWorldPos(mousePosition);
        GridIndicator.transform.position = gridPosition;
    }

    public void StartPlacement(int Id, GameObject cardPrefab)
    {
        selectedCardData = (CardData)(OriginDeck.Find(card => card.Id == Id)).Clone();
        selectedCardPrefab = cardPrefab;
        previewAstral = Instantiate(selectedCardData.Prefab);

        InputSystem.Instance.OnPressedLeft += GrapAstral;
        InputSystem.Instance.OnReleasedLeft += PlacementAstral;
    }
    public void GrapAstral()
    {
        previewAstral.transform.position = mousePosition + new Vector3(0, 0.3f, 0);
    }
    public void PlacementAstral()
    {
        if (!InputSystem.Instance.CanPlacemet()) // 영체를 놓을 수 없는 곳에서 마우스를 떼었다면
        {
            StopPlacement();
            return;
        }

        GameObject go = Instantiate(selectedCardData.Prefab);
        go.transform.position = gridPosition;
        gridVertex.AstralOnGrid = go;
        Destroy(selectedCardPrefab); // 카드를 냈다면 카드 프리팹 제거;
        StopPlacement();
    }
    public void StopPlacement()
    {
        Destroy(previewAstral); // 프리뷰 오브젝트 제거
        selectedCardPrefab = null;
        InputSystem.Instance.OnPressedLeft -= GrapAstral;
        InputSystem.Instance.OnReleasedLeft -= PlacementAstral;
    }
}
