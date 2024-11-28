using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

public class GenerateGridVisual : MonoBehaviour
{
    [SerializeField] GameObject YellowHexPrefab;
    [SerializeField] GameObject GreenHexPrefab;
    [SerializeField] GameObject RedHexPrefab;
    [SerializeField] TMP_Text PrayPledgeAvailableText;

    List<GameObject> AstralGridList = new();
    List<GameObject> PrayGridList = new();
    Dictionary<Vector3, GameObject> WholeGridList = new();
    GameObject greenAstralIndicator;
    GameObject redAstralIndicator;
    string thisPlayerTag;
    [SerializeField] Vector3 gridPosition;
    Vector3 mousePosition;
    Vertex gridVertex;
    [SerializeField] Vector3 lastPosition;
    Vector3[] prayRange2 = new Vector3[]
    {
        new Vector3(0, 0, 0),
        new Vector3(-Mathf.Sqrt(3) / 2f, 0, 3f / 2f),
        new Vector3(Mathf.Sqrt(3) / 2f, 0, 3f / 2f),
        new Vector3(Mathf.Sqrt(3), 0, 0),
        new Vector3(Mathf.Sqrt(3) / 2f, 0, -3f / 2f),
        new Vector3(-Mathf.Sqrt(3) / 2f, 0, -3f / 2f),
        new Vector3(-Mathf.Sqrt(3), 0, 0)
    };
    Vector3[] prayRange3 = new Vector3[]
    {
        new Vector3(0, 0, 0),
        new Vector3(-Mathf.Sqrt(3) / 2f, 0, 3f / 2f),
        new Vector3(Mathf.Sqrt(3) / 2f, 0, 3f / 2f),
        new Vector3(Mathf.Sqrt(3), 0, 0),
        new Vector3(Mathf.Sqrt(3) / 2f, 0, -3f / 2f),
        new Vector3(-Mathf.Sqrt(3) / 2f, 0, -3f / 2f),
        new Vector3(-Mathf.Sqrt(3), 0, 0),
        // 2번째 칸
        new Vector3(0, 0, 3f),
        new Vector3(Mathf.Sqrt(3), 0, 3f),
        new Vector3(-Mathf.Sqrt(3), 0, 3f),
        new Vector3(3 * Mathf.Sqrt(3) / 2f, 0, 3f / 2f),
        new Vector3(-3 * Mathf.Sqrt(3) / 2f, 0, 3f / 2f),
        new Vector3(2 * Mathf.Sqrt(3), 0, 0),
        new Vector3(-2 * Mathf.Sqrt(3), 0, 0),
        new Vector3(3 * Mathf.Sqrt(3) / 2f, 0, -3f / 2f),
        new Vector3(-3 * Mathf.Sqrt(3) / 2f, 0, -3f / 2f),
        new Vector3(0, 0, -3f),
        new Vector3(Mathf.Sqrt(3), 0, -3f),
        new Vector3(-Mathf.Sqrt(3), 0, -3f)
    };

    private void Awake()
    {
        thisPlayerTag = gameObject.tag;
    }

    private void Start()
    {
        CreateAstralGrid();
        CreatePrayGrid();
        CreateAstralIndicator();
        CreateWholeGrid();

        OnAstralGridVisual(false);
        OnPrayGridVisual(false);
    }

    private void Update()
    {
        gridPosition = (GridManager.Instance.GetGridPosFromWorldPos(mousePosition)).Coordinate;
        gridVertex = GridManager.Instance.GetGridPosFromWorldPos(mousePosition);
    }

    void CreateAstralGrid()
    {
        if (thisPlayerTag == "Player")
        {
            foreach (Vertex vertex in GridManager.Instance.Grids.Vertices)
            {
                if (vertex.Coordinate.z < 0)
                {
                    GameObject go = Instantiate(YellowHexPrefab);
                    go.transform.position = vertex.Coordinate + new Vector3(0, 0.01f, 0);
                    AstralGridList.Add(go);
                }
            }
        }
        else if (thisPlayerTag == "OpponentAI")
        {
            foreach (Vertex vertex in GridManager.Instance.Grids.Vertices)
            {
                if (vertex.Coordinate.z > 0)
                {
                    GameObject go = Instantiate(YellowHexPrefab);
                    go.transform.position = vertex.Coordinate + new Vector3(0, 0.01f, 0);
                    AstralGridList.Add(go);
                }
            }
        }
    }
    void CreatePrayGrid()
    {
        foreach (Vertex vertex in GridManager.Instance.Grids.Vertices)
        {
            GameObject go = Instantiate(YellowHexPrefab);
            go.transform.position = vertex.Coordinate + new Vector3(0, 0.01f, 0);
            PrayGridList.Add(go);
        }
    }
    void CreateAstralIndicator()
    {
        greenAstralIndicator = Instantiate(GreenHexPrefab);
        greenAstralIndicator.SetActive(false);
        redAstralIndicator = Instantiate(RedHexPrefab);
        redAstralIndicator.SetActive(false);
    }
    void CreateWholeGrid()
    {
        foreach (Vertex vertex in GridManager.Instance.Grids.Vertices)
        {
            GameObject go = Instantiate(GreenHexPrefab);
            go.transform.position = vertex.Coordinate + new Vector3(0, 0.015f, 0);
            go.SetActive(false);
            WholeGridList[go.transform.position] = go;
        }
    }

    public void OnAstralGridVisual(bool turnOn)
    {
        if (!turnOn)
        {
            foreach (GameObject go in AstralGridList)
            {
                go.SetActive(false);
            }
        }
        else
        {
            foreach (GameObject go in AstralGridList)
            {
                go.SetActive(true);
            }
        }
    }
    public void OnPrayGridVisual(bool turnOn)
    {
        if (!turnOn)
        {
            foreach (GameObject go in PrayGridList)
            {
                go.SetActive(false);
            }
        }
        else
        {
            foreach (GameObject go in PrayGridList)
            {
                go.SetActive(true);
            }
        }
    }
    public void OnAstralIndicator(bool turnOn)
    {
        mousePosition = GetComponent<InputSystem>().GetMousePositionOnAstralField();

        if (turnOn)
        {
            if (GetComponent<InputSystem>().CanPlacementAstral())
            {
                if (gridVertex.AstralOnGrid != null)
                {
                    greenAstralIndicator.SetActive(false);
                    redAstralIndicator.SetActive(true);
                    redAstralIndicator.transform.position = gridPosition + new Vector3(0, 0.015f, 0);

                    return;
                }

                greenAstralIndicator.SetActive(true);
                redAstralIndicator.SetActive(false);
                greenAstralIndicator.transform.position = gridPosition + new Vector3(0, 0.015f, 0);
            }
            else
            {
                greenAstralIndicator.SetActive(false);
            }
        }
        else
        {
            greenAstralIndicator.SetActive(false);
            redAstralIndicator.SetActive(false);
        }
    }
    public void OnPrayRangeIndicator(bool turnOn, int prayRange)
    {
        mousePosition = GetComponent<InputSystem>().GetMousePositionOnField();
        float epsilon = 0.001f;
        Vector3 newCoord;

        if (turnOn)
        {
            if (lastPosition != gridPosition)
            {
                lastPosition = gridPosition;

                foreach (Vector3 pos in WholeGridList.Keys)
                {
                    WholeGridList[pos].SetActive(false);
                }

                if (GetComponent<InputSystem>().CanPlacement())
                {
                    switch (prayRange)
                    {
                        case 1:
                            newCoord = gridPosition;

                            foreach (Vector3 pos in WholeGridList.Keys)
                            {
                                if (!WholeGridList[pos].activeSelf)
                                {
                                    if (Mathf.Abs(newCoord.x - pos.x) < epsilon && Mathf.Abs(newCoord.z - pos.z) < epsilon)
                                    {
                                        WholeGridList[pos].SetActive(true);
                                    }
                                    else
                                    {
                                        WholeGridList[pos].SetActive(false);
                                    }
                                    if (gridPosition.x == 0 && gridPosition.z == 0) // 가운데 그리드일 때 별도 처리. 왜 그런지 모르겟네
                                    {
                                        WholeGridList[pos].SetActive(true);
                                    }
                                }
                            }
                            break;
                        case 2:
                            foreach (Vector3 direction in prayRange2)
                            {
                                newCoord = gridPosition + direction;

                                foreach (Vector3 pos in WholeGridList.Keys)
                                {
                                    if (!WholeGridList[pos].activeSelf)
                                    {
                                        if (Mathf.Abs(newCoord.x - pos.x) < epsilon && Mathf.Abs(newCoord.z - pos.z) < epsilon)
                                        {
                                            WholeGridList[pos].SetActive(true);
                                        }
                                        else
                                        {
                                            WholeGridList[pos].SetActive(false);
                                        }
                                    }
                                }
                            }
                            break;
                        case 3:
                            foreach (Vector3 direction in prayRange3)
                            {
                                newCoord = gridPosition + direction;

                                foreach (Vector3 pos in WholeGridList.Keys)
                                {
                                    if (!WholeGridList[pos].activeSelf)
                                    {
                                        if (Mathf.Abs(newCoord.x - pos.x) < epsilon && Mathf.Abs(newCoord.z - pos.z) < epsilon)
                                        {
                                            WholeGridList[pos].SetActive(true);
                                        }
                                        else
                                        {
                                            WholeGridList[pos].SetActive(false);
                                        }
                                    }
                                }
                            }
                            break;
                    }

                    PrayPledgeAvailableText.gameObject.SetActive(true);
                    PrayPledgeAvailableText.text = "기도 서약하기";
                    PrayPledgeAvailableText.gameObject.transform.position = gridPosition + new Vector3(0, 1f, 0);
                    PrayPledgeAvailableText.gameObject.transform.LookAt(PrayPledgeAvailableText.gameObject.transform.position + GetComponent<InputSystem>().PlayerCamera.transform.forward);
                }
                else
                {
                    foreach (Vector3 pos in WholeGridList.Keys)
                    {
                        WholeGridList[pos].SetActive(false);
                    }

                    PrayPledgeAvailableText.gameObject.SetActive(false);
                }
            }
            else if (!GetComponent<InputSystem>().CanPlacement())
            {
                foreach (Vector3 pos in WholeGridList.Keys)
                {
                    WholeGridList[pos].SetActive(false);
                }

                lastPosition = new Vector3(0, 0, 0);

                PrayPledgeAvailableText.gameObject.SetActive(false);
            }
            else
            {
                lastPosition = gridPosition;
            }
        }
        else
        {
            foreach (Vector3 pos in WholeGridList.Keys)
            {
                WholeGridList[pos].SetActive(false);
            }

            PrayPledgeAvailableText.gameObject.SetActive(false);
        }
    }
}
