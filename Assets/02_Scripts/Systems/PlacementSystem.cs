using System;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour // ����ν�� �� ���� �÷��̾� ��ġ�� ����� �� ����. �� �÷��̾��� ��ġ���� ����� �����Ѵ�.
{
    public GameObject MouseIndicator;
    public GameObject GridIndicator;
    public List<CardData> OriginDeck; // �ӽ�

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
        OriginDeck = DeckManager.Instance.GetPlayerDeck(); // �ӽ�
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
        if (!InputSystem.Instance.CanPlacemet()) // ��ü�� ���� �� ���� ������ ���콺�� �����ٸ�
        {
            StopPlacement();
            return;
        }

        GameObject go = Instantiate(selectedCardData.Prefab);
        go.transform.position = gridPosition;
        gridVertex.AstralOnGrid = go;
        Destroy(selectedCardPrefab); // ī�带 �´ٸ� ī�� ������ ����;
        StopPlacement();
    }
    public void StopPlacement()
    {
        Destroy(previewAstral); // ������ ������Ʈ ����
        selectedCardPrefab = null;
        InputSystem.Instance.OnPressedLeft -= GrapAstral;
        InputSystem.Instance.OnReleasedLeft -= PlacementAstral;
    }
}
