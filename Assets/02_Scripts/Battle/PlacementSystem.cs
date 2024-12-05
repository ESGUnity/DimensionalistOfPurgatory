using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour // 현재로써는 두 명의 플레이어 배치를 담당할 수 없다. 한 플레이어의 배치에만 기능을 제공한다.
{
    [HideInInspector] public Vector3 gridPosition;

    List<CardData> OriginDeck; // 임시
    Vector3 mousePosition;
    Vertex gridVertex;
    CardData selectedCardData;
    GameObject previewAstral;
    GameObject selectedCardPrefab;
    string thisPlayerTag;

    private void Start()
    {
        OriginDeck = DeckManager.Instance.GetPlayerDeck(); // 임시
        thisPlayerTag = gameObject.tag;
    }

    void Update()
    {
        //mousePosition = GetComponent<InputSystem>().GetMousePositionOnField(); // 임시
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
        if (!GetComponent<InputSystem>().CanPlacementAstral()) // 영체를 놓을 수 없는 곳에서 OnReleasedLeft가 수행된다면 리턴
        {
            StopAstralPlacement();
            return;
        }
        if (gridVertex.AstralOnGrid != null) // 놓으려는 그리드에 이미 영체가 있다면 리턴
        {
            StopAstralPlacement();
            return;
        }

        GameObject go = Instantiate(selectedCardData.Prefab);
        go.GetComponent<AstralBody>().SetAstralInfo(gridVertex, selectedCardData, thisPlayerTag); // 영체 정보 설정 // 추후 반드시 활성화
        go.transform.position = gridPosition; // 영체 위치 설정

        selectedCardPrefab.GetComponent<InteractableCard>().DestroyCardPrefab(); // 카드 프리팹 제거
        GetComponent<HandSystem>().HandCount--; // HandSystem에도 카드가 사용되어 손패가 줄도록 만들기
        StopAstralPlacement();
    }
    public void StopAstralPlacement()
    {
        Destroy(previewAstral); // 프리뷰 오브젝트 제거
        GetComponent<GenerateGridVisual>().OnAstralGridVisual(false);
        GetComponent<GenerateGridVisual>().OnAstralIndicator(false);

        selectedCardPrefab = null;
        selectedCardData = null;

        GetComponent<InputSystem>().OnPressedLeft -= GrapAstral; // 구독 해제
        GetComponent<InputSystem>().OnReleasedLeft -= PlacementAstral;
    }

    public void GrapPray()
    {
        mousePosition = GetComponent<InputSystem>().GetMousePositionOnField();
        GetComponent<GenerateGridVisual>().OnPrayRangeIndicator(true, selectedCardData.PrayRange);
    }
    public void PledgePray()
    {
        if (!GetComponent<InputSystem>().CanPlacement()) // 기도를 서약할 수 없는 커서 위치에서 서약을 하려고 하면 리턴
        {
            StopPrayPledge();
            return;
        }

        GetComponent<HandSystem>().ImportPledgePrayToHand(selectedCardData);

        selectedCardPrefab.GetComponent<InteractableCard>().DestroyCardPrefab(); // 카드 프리팹 제거
        GetComponent<HandSystem>().HandCount--; // HandSystem에도 카드가 사용되어 배치됨을 알림.
        StopPrayPledge();
    }
    public void StopPrayPledge()
    {
        GetComponent<GenerateGridVisual>().OnPrayGridVisual(false);
        GetComponent<GenerateGridVisual>().OnPrayRangeIndicator(false, 1);

        selectedCardPrefab = null;
        selectedCardData = null;

        GetComponent<InputSystem>().OnPressedLeft -= GrapPray; // 구독 해제
        GetComponent<InputSystem>().OnReleasedLeft -= PledgePray;
    }
}
