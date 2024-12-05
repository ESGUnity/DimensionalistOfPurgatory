using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour // ����ν�� �� ���� �÷��̾� ��ġ�� ����� �� ����. �� �÷��̾��� ��ġ���� ����� �����Ѵ�.
{
    [HideInInspector] public Vector3 gridPosition;

    List<CardData> OriginDeck; // �ӽ�
    Vector3 mousePosition;
    Vertex gridVertex;
    CardData selectedCardData;
    GameObject previewAstral;
    GameObject selectedCardPrefab;
    string thisPlayerTag;

    private void Start()
    {
        OriginDeck = DeckManager.Instance.GetPlayerDeck(); // �ӽ�
        thisPlayerTag = gameObject.tag;
    }

    void Update()
    {
        //mousePosition = GetComponent<InputSystem>().GetMousePositionOnField(); // �ӽ�
        gridVertex = GridManager.Instance.GetGridPosFromWorldPos(mousePosition);
        gridPosition = (GridManager.Instance.GetGridPosFromWorldPos(mousePosition)).Coordinate;

    }

    public void StartPlacement(int Id, GameObject cardPrefab)
    {
        if (PhaseManager.Instance.CurrentPhase == Phase.Preparation)
        {
            selectedCardData = (CardData)(OriginDeck.Find(card => card.Id == Id)).Clone();
            selectedCardPrefab = cardPrefab;

            if (selectedCardData.IsAstral)
            {
                previewAstral = Instantiate(selectedCardData.Prefab);
                GetComponent<GenerateGridVisual>().OnAstralGridVisual(true);

                GetComponent<InputSystem>().OnPressedLeft += GrapAstral;
                GetComponent<InputSystem>().OnReleasedLeft += PlacementAstral;
            }
            else
            {
                GetComponent<GenerateGridVisual>().OnPrayGridVisual(true);

                GetComponent<InputSystem>().OnPressedLeft += GrapPray;
                GetComponent<InputSystem>().OnReleasedLeft += PledgePray;
            }
        }
    }
    public void GrapAstral()
    {
        mousePosition = GetComponent<InputSystem>().GetMousePositionOnAstralField();
        GetComponent<GenerateGridVisual>().OnAstralIndicator(true);

        previewAstral.transform.position = gridPosition + new Vector3(0, 0.3f, 0);
    }
    public void PlacementAstral()
    {
        if (!GetComponent<InputSystem>().CanPlacementAstral()) // ��ü�� ���� �� ���� ������ OnReleasedLeft�� ����ȴٸ� ����
        {
            StopAstralPlacement();
            return;
        }
        if (gridVertex.AstralOnGrid != null) // �������� �׸��忡 �̹� ��ü�� �ִٸ� ����
        {
            StopAstralPlacement();
            return;
        }

        GameObject go = Instantiate(selectedCardData.Prefab);
        go.GetComponent<AstralBody>().SetAstralInfo(gridVertex, selectedCardData, thisPlayerTag); // ��ü ���� ���� // ���� �ݵ�� Ȱ��ȭ
        go.transform.position = gridPosition; // ��ü ��ġ ����

        selectedCardPrefab.GetComponent<InteractableCard>().DestroyCardPrefab(); // ī�� ������ ����
        GetComponent<HandSystem>().HandCount--; // HandSystem���� ī�尡 ���Ǿ� ���а� �ٵ��� �����
        StopAstralPlacement();
    }
    public void StopAstralPlacement()
    {
        Destroy(previewAstral); // ������ ������Ʈ ����
        GetComponent<GenerateGridVisual>().OnAstralGridVisual(false);
        GetComponent<GenerateGridVisual>().OnAstralIndicator(false);

        selectedCardPrefab = null;
        selectedCardData = null;

        GetComponent<InputSystem>().OnPressedLeft -= GrapAstral; // ���� ����
        GetComponent<InputSystem>().OnReleasedLeft -= PlacementAstral;
    }

    public void GrapPray()
    {
        mousePosition = GetComponent<InputSystem>().GetMousePositionOnField();
        GetComponent<GenerateGridVisual>().OnPrayRangeIndicator(true, selectedCardData.PrayRange);
    }
    public void PledgePray()
    {
        if (!GetComponent<InputSystem>().CanPlacement()) // �⵵�� ������ �� ���� Ŀ�� ��ġ���� ������ �Ϸ��� �ϸ� ����
        {
            StopPrayPledge();
            return;
        }

        GetComponent<HandSystem>().ImportPledgePrayToHand(selectedCardData);

        selectedCardPrefab.GetComponent<InteractableCard>().DestroyCardPrefab(); // ī�� ������ ����
        GetComponent<HandSystem>().HandCount--; // HandSystem���� ī�尡 ���Ǿ� ��ġ���� �˸�.
        StopPrayPledge();
    }
    public void StopPrayPledge()
    {
        GetComponent<GenerateGridVisual>().OnPrayGridVisual(false);
        GetComponent<GenerateGridVisual>().OnPrayRangeIndicator(false, 1);

        selectedCardPrefab = null;
        selectedCardData = null;

        GetComponent<InputSystem>().OnPressedLeft -= GrapPray; // ���� ����
        GetComponent<InputSystem>().OnReleasedLeft -= PledgePray;
    }
}
