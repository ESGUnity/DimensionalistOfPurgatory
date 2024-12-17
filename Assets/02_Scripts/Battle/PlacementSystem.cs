using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour // ����ν�� �� ���� �÷��̾� ��ġ�� ����� �� ����. �� �÷��̾��� ��ġ���� ����� �����Ѵ�.
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
                previewAstral = Instantiate(selectedCardData.Prefab); // ������ ��ü�� ���� �ڵ�
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
            foreach (Renderer renderer in go.GetComponentsInChildren<Renderer>()) // ��Ƽ���� ����
            {
                renderer.material = astralMaterial;
            }

            GetComponent<BringerSystem>().CurrentEssence -= selectedCardData.Cost;
            GetComponent<BringerSystem>().HandleAstralAnimation();
            go.transform.eulerAngles = new Vector3(0, 0, 0); // ȸ���� ���� �Ҵ�. �׻� �������� ������

            selectedCardPrefab.GetComponent<InteractableCard>().DestroyCardPrefab(); // ī�� ������ ����
            GetComponent<HandSystem>().HandCount--; // HandSystem���� ī�尡 ���Ǿ� ���а� �ٵ��� �����
            StopAstralPlacement();
        }
        else
        {
            StopAstralPlacement();
        }
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
            if (!GetComponent<InputSystem>().CanPlacementAstral()) // ��ü�� ���� �� ���� ������ OnReleasedLeft�� ����ȴٸ� ����
            {
                replacementAstral.transform.position = replacementAstral.GetComponent<AstralBody>().thisGridVertex.Coordinate;
                StopAstralReplacement();
                return;
            }
            if (gridVertex.AstralOnGrid != null) // �������� �׸��忡 �̹� ��ü�� �ִٸ� ����
            {
                replacementAstral.transform.position = replacementAstral.GetComponent<AstralBody>().thisGridVertex.Coordinate;
                StopAstralReplacement();
                return;
            }

            replacementAstral.transform.position = gridPosition; // ��ü ��ġ ����
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
            if (!GetComponent<InputSystem>().CanPlacement()) // �⵵�� ������ �� ���� Ŀ�� ��ġ���� ������ �Ϸ��� �ϸ� ����
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

            selectedCardPrefab.GetComponent<InteractableCard>().DestroyCardPrefab(); // ī�� ������ ����
            GetComponent<HandSystem>().HandCount--; // HandSystem���� ī�尡 ���Ǿ� ��ġ���� �˸�.
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

        GetComponent<InputSystem>().OnPressedLeft -= GrapPray; // ���� ����
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
            if (!GetComponent<InputSystem>().CanPlacement()) // �⵵�� ������ �� ���� Ŀ�� ��ġ���� ������ �Ϸ��� �ϸ� ����
            {
                StopPrayPledge();
                return;
            }

            GameObject go = Instantiate(selectedCardData.Prefab);
            go.GetComponent<Pray>().SetPrayInfo(gridVertex, selectedCardData, thisPlayerTag); // ��ü ���� ���� // ���� �ݵ�� Ȱ��ȭ
            go.GetComponent<Pray>().CastPray();
            go.transform.position = gridPosition;

            GetComponent<HandSystem>().PledgeHandCount--;
            selectedCardPrefab.GetComponent<InteractableCard>().DestroyCardPrefab(); // ī�� ������ ����
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
