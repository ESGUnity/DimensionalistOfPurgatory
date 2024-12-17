using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour // 현재로써는 두 명의 플레이어 배치를 담당할 수 없다. 한 플레이어의 배치에만 기능을 제공한다.
{
    [HideInInspector] public Vector3 gridPosition;

    [SerializeField] Material previewMaterial;
    [SerializeField] Material astralMaterial;

    List<CardData> OriginDeck;
    Vector3 mousePosition;
    Vertex gridVertex;
    CardData selectedCardData;
    GameObject previewAstral;
    GameObject replacementAstral;
    GameObject selectedCardPrefab;
    string thisPlayerTag;

    private void Start()
    {
        thisPlayerTag = gameObject.tag;

        if (thisPlayerTag == "Player")
        {
            OriginDeck = DeckManager.Instance.GetPlayerDeck();
        }
        else if (thisPlayerTag == "Opponent")
        {
            OriginDeck = DeckManager.Instance.GetOpponentDeck();
        }
    }

    void Update()
    {
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
                previewAstral = Instantiate(selectedCardData.Prefab); // 프리뷰 영체에 관한 코드
                previewAstral.GetComponent<AstralBody>().enabled = false;
                previewAstral.GetComponent<AstralStatusEffect>().enabled = false;
                foreach (Renderer renderer in previewAstral.GetComponentsInChildren<Renderer>())
                {
                    renderer.material = previewMaterial;
                }

                GetComponent<GenerateGridVisual>().OnAstralGridVisual(true);

                GetComponent<InputSystem>().OnPressedLeft += GrapAstral;
                GetComponent<InputSystem>().OnReleasedLeft += PlacementAstral;
                GetComponent<InputSystem>().OnClickedRight += StopAstralPlacement;
            }
            else
            {
                GetComponent<GenerateGridVisual>().OnPrayGridVisual(true);

                GetComponent<InputSystem>().OnPressedLeft += GrapPray;
                GetComponent<InputSystem>().OnReleasedLeft += PledgePray;
                GetComponent<InputSystem>().OnClickedRight += StopPrayPledge;
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
        if (PhaseManager.Instance.CurrentPhase == Phase.Preparation)
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
            foreach (Renderer renderer in go.GetComponentsInChildren<Renderer>()) // 머티리얼 설정
            {
                renderer.material = astralMaterial;
            }

            GetComponent<BringerSystem>().CurrentEssence -= selectedCardData.Cost;
            GetComponent<BringerSystem>().HandleAstralAnimation();
            go.transform.eulerAngles = new Vector3(0, 0, 0); // 회전도 직접 할당. 항상 적방향을 보도록

            selectedCardPrefab.GetComponent<InteractableCard>().DestroyCardPrefab(); // 카드 프리팹 제거
            GetComponent<HandSystem>().HandCount--; // HandSystem에도 카드가 사용되어 손패가 줄도록 만들기
            StopAstralPlacement();
        }
        else
        {
            StopAstralPlacement();
        }
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
        GetComponent<InputSystem>().OnClickedRight -= StopAstralPlacement;
    }
    public void StartAstralReplacement(GameObject astral)
    {
        if (PhaseManager.Instance.CurrentPhase == Phase.Preparation)
        {
            replacementAstral = astral;
            GetComponent<GenerateGridVisual>().OnAstralGridVisual(true);

            GetComponent<InputSystem>().OnPressedLeft += ReGrapAstral;
            GetComponent<InputSystem>().OnReleasedLeft += ReplacementAstral;
            GetComponent<InputSystem>().OnClickedRight += StopAstralReplacement;
        }
    }
    public void ReGrapAstral()
    {
        if (PhaseManager.Instance.CurrentPhase == Phase.Preparation)
        {
            mousePosition = GetComponent<InputSystem>().GetMousePositionOnAstralField();
            GetComponent<GenerateGridVisual>().OnAstralIndicator(true);

            replacementAstral.transform.position = mousePosition + new Vector3(0, 0.3f, 0);
        }
        else
        {
            replacementAstral.transform.position = replacementAstral.GetComponent<AstralBody>().thisGridVertex.Coordinate;
            StopAstralReplacement();
            return;
        }
    }
    public void ReplacementAstral()
    {
        if (PhaseManager.Instance.CurrentPhase == Phase.Preparation)
        {
            if (!GetComponent<InputSystem>().CanPlacementAstral()) // 영체를 놓을 수 없는 곳에서 OnReleasedLeft가 수행된다면 리턴
            {
                replacementAstral.transform.position = replacementAstral.GetComponent<AstralBody>().thisGridVertex.Coordinate;
                StopAstralReplacement();
                return;
            }
            if (gridVertex.AstralOnGrid != null) // 놓으려는 그리드에 이미 영체가 있다면 리턴
            {
                replacementAstral.transform.position = replacementAstral.GetComponent<AstralBody>().thisGridVertex.Coordinate;
                StopAstralReplacement();
                return;
            }

            replacementAstral.transform.position = gridPosition; // 영체 위치 설정
            gridVertex.AstralOnGrid = replacementAstral;
            replacementAstral.GetComponent<AstralBody>().thisGridVertex.AstralOnGrid = null;
            replacementAstral.GetComponent<AstralBody>().thisGridVertex = gridVertex;

            GetComponent<BringerSystem>().HandleAstralAnimation();

            if (thisPlayerTag == "Player")
            {
                PhaseManager.Instance.phaseStorageBattleInfo.PlayerAstralOriginPos[replacementAstral] = gridVertex;
            }
            else if (thisPlayerTag == "Opponent")
            {
                PhaseManager.Instance.phaseStorageBattleInfo.OpponentAstralOriginPos[replacementAstral] = gridVertex;
            }

            StopAstralReplacement();
        }
        else
        {
            replacementAstral.transform.position = replacementAstral.GetComponent<AstralBody>().thisGridVertex.Coordinate;
            StopAstralReplacement();
        }
    }
    public void StopAstralReplacement()
    {
        GetComponent<GenerateGridVisual>().OnAstralGridVisual(false);
        GetComponent<GenerateGridVisual>().OnAstralIndicator(false);

        replacementAstral = null;

        GetComponent<InputSystem>().OnPressedLeft -= ReGrapAstral;
        GetComponent<InputSystem>().OnReleasedLeft -= ReplacementAstral;
        GetComponent<InputSystem>().OnClickedRight -= StopAstralReplacement;
    }

    public void GrapPray()
    {
        mousePosition = GetComponent<InputSystem>().GetMousePositionOnField();
        GetComponent<GenerateGridVisual>().OnPrayRangeIndicator(true, selectedCardData.PrayRange, false);
    }
    public void PledgePray()
    {
        if (PhaseManager.Instance.CurrentPhase == Phase.Preparation)
        {
            if (!GetComponent<InputSystem>().CanPlacement()) // 기도를 서약할 수 없는 커서 위치에서 서약을 하려고 하면 리턴
            {
                StopPrayPledge();
                return;
            }
            if (GetComponent<HandSystem>().PledgeHandCount >= 3)
            {
                StopPrayPledge();
                return;
            }

            GetComponent<HandSystem>().ImportPledgePrayToHand(selectedCardData);
            GetComponent<BringerSystem>().CurrentEssence -= selectedCardData.Cost;
            GetComponent<BringerSystem>().HandleAstralAnimation();

            selectedCardPrefab.GetComponent<InteractableCard>().DestroyCardPrefab(); // 카드 프리팹 제거
            GetComponent<HandSystem>().HandCount--; // HandSystem에도 카드가 사용되어 배치됨을 알림.
            StopPrayPledge();
        }
        else
        {
            StopPrayPledge();
        }
    }
    public void StopPrayPledge()
    {
        GetComponent<GenerateGridVisual>().OnPrayGridVisual(false);
        GetComponent<GenerateGridVisual>().OnPrayRangeIndicator(false, 1, false);

        selectedCardPrefab = null;
        selectedCardData = null;

        GetComponent<InputSystem>().OnPressedLeft -= GrapPray; // 구독 해제
        GetComponent<InputSystem>().OnReleasedLeft -= PledgePray;
        GetComponent<InputSystem>().OnClickedRight -= StopPrayPledge;
    }

    public void StartPrayCast(int Id, GameObject cardPrefab)
    {
        if (PhaseManager.Instance.CurrentPhase == Phase.Battle)
        {
            selectedCardData = (CardData)(OriginDeck.Find(card => card.Id == Id)).Clone();
            selectedCardPrefab = cardPrefab;

            GetComponent<GenerateGridVisual>().OnPrayGridVisual(true);

            GetComponent<InputSystem>().OnPressedLeft += GrabPrayCast;
            GetComponent<InputSystem>().OnReleasedLeft += CastPray;
            GetComponent<InputSystem>().OnClickedRight += StopPrayCast;
        }
    }
    public void GrabPrayCast()
    {
        mousePosition = GetComponent<InputSystem>().GetMousePositionOnField();
        GetComponent<GenerateGridVisual>().OnPrayRangeIndicator(true, selectedCardData.PrayRange, true);
    }
    public void CastPray()
    {
        if (PhaseManager.Instance.CurrentPhase == Phase.Battle)
        {
            if (!GetComponent<InputSystem>().CanPlacement()) // 기도를 서약할 수 없는 커서 위치에서 서약을 하려고 하면 리턴
            {
                StopPrayPledge();
                return;
            }

            GameObject go = Instantiate(selectedCardData.Prefab);
            go.GetComponent<Pray>().SetPrayInfo(gridVertex, selectedCardData, thisPlayerTag); // 영체 정보 설정 // 추후 반드시 활성화
            go.GetComponent<Pray>().CastPray();
            go.transform.position = gridPosition;

            GetComponent<HandSystem>().PledgeHandCount--;
            selectedCardPrefab.GetComponent<InteractableCard>().DestroyCardPrefab(); // 카드 프리팹 제거
            StopPrayPledge();
        }
        else
        {
            StopPrayCast();
        }
    }
    public void StopPrayCast()
    {
        GetComponent<GenerateGridVisual>().OnPrayGridVisual(false);
        GetComponent<GenerateGridVisual>().OnPrayRangeIndicator(false, 1, false);

        selectedCardPrefab = null;
        selectedCardData = null;

        GetComponent<InputSystem>().OnPressedLeft -= GrabPrayCast;
        GetComponent<InputSystem>().OnReleasedLeft -= CastPray;
        GetComponent<InputSystem>().OnClickedRight -= StopPrayCast;
    }
}
