using System.Collections.Generic;
using UnityEngine;

public class OpponentAstralSpawner : MonoBehaviour
{
    [SerializeField] Material astralMaterial;

    List<CardData> originPlayerDeck;
    string thisPlayerTag;
    List<CardData> hand = new();
    List<CardData> removeData = new();
    void Start()
    {
        thisPlayerTag = gameObject.tag;
        PhaseManager.Instance.OnPreparation += ResetAstral;

        if (DeckManager.Instance.opponentDeckNumber == 4) // �������� �ε��� ���� ��
        {
            GetComponent<BringerSystem>().MaxEssence += 6;
            GetComponent<BringerSystem>().CurrentEssence = GetComponent<BringerSystem>().MaxEssence;
        }
        if (DeckManager.Instance.opponentDeckNumber == 3) // �������� �ε��� ���� ��
        {
            GetComponent<BringerSystem>().MaxEssence += 4;
            GetComponent<BringerSystem>().CurrentEssence = GetComponent<BringerSystem>().MaxEssence;
        }
        if (DeckManager.Instance.opponentDeckNumber == 2) // �������� �ε��� ���� ��
        {
            GetComponent<BringerSystem>().MaxEssence += 2;
            GetComponent<BringerSystem>().CurrentEssence = GetComponent<BringerSystem>().MaxEssence;
        }
    }
    private void Update()
    {
        if (CardManager.Instance.CountDeck(thisPlayerTag) <= 0)
        {
            CardManager.Instance.FillDeck(thisPlayerTag);
        }
        if (PhaseManager.Instance.CurrentPhase == Phase.Preparation)
        {
            SpawnAstral();
        }
        if (hand.Count < 5)
        {
            hand.Add(CardManager.Instance.DrawCard(thisPlayerTag));
        }
    }
    void ResetAstral() // �������� �ɰ��� ���� ����.. ��.....
    {
        if (PhaseManager.Instance.phaseStorageBattleInfo.OpponentAstral.Count > 0)
        {
            foreach (GameObject go in PhaseManager.Instance.phaseStorageBattleInfo.OpponentAstral)
            {
                go.GetComponent<AstralBody>().thisGridVertex.AstralOnGrid = null;
                PhaseManager.Instance.phaseStorageBattleInfo.OpponentAstralOriginPos[go].AstralOnGrid = go;
                go.transform.position = PhaseManager.Instance.phaseStorageBattleInfo.OpponentAstralOriginPos[go].Coordinate;
                go.GetComponent<AstralBody>().thisGridVertex = PhaseManager.Instance.phaseStorageBattleInfo.OpponentAstralOriginPos[go];
                go.transform.eulerAngles = new Vector3(0, 180, 0);
            }
        }
    }
    void SpawnAstral()
    {
        foreach (CardData card in hand)
        {
            if (card.IsAstral)
            {
                if (card.Cost <= GetComponent<BringerSystem>().CurrentEssence && card.Cost <= GetComponent<BringerSystem>().LimitEssence) // �� �� �ִ� ī�尡 �ִٸ�
                {
                    Vertex gridVertex;
                    int count = 0;
                    float[] x = { -6.06f, -5.19f, -4.33f, -3.46f, -2.59f, -1.73f, -0.86f, 0, 0.86f, 1.73f, 2.59f, 3.46f, 4.33f, 5.19f, 6.06f };
                    float[] z = { 1.5f, 3f, 4.5f, 6f };

                    while (count < 500)
                    {
                        count++;

                        gridVertex = GridManager.Instance.GetGridPosFromWorldPos(new Vector3(x[Random.Range(0, 15)], 0, z[Random.Range(0, 4)]));

                        if (gridVertex.AstralOnGrid != null)
                        {
                            return;
                        }
                        if (gridVertex != null && gridVertex.AstralOnGrid == null)
                        {
                            //Debug.Log("���ߤ����Ⱦ˾ƤӤ������ƤӸ��ƶ�Ӥ���");
                            GameObject go = Instantiate(card.Prefab);
                            go.GetComponent<AstralBody>().SetAstralInfo(gridVertex, card, thisPlayerTag);
                            go.transform.position = gridVertex.Coordinate;
                            foreach (Renderer renderer in go.GetComponentsInChildren<Renderer>()) // ��Ƽ���� ����
                            {
                                renderer.material = astralMaterial;
                            }

                            GetComponent<BringerSystem>().CurrentEssence -= card.Cost;
                            go.transform.eulerAngles = new Vector3(0, 180f, 0); // �� ��ü�� ȸ���� ���� ���ش�.
                            removeData.Add(card);

                            break;
                        }
                    }
                }
            }
            else
            {
                removeData.Add(card);
            }
        }

        foreach (CardData card in removeData)
        {
            hand.Remove(card);
        }
    }
}
