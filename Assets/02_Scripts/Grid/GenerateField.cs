using UnityEngine;

public class GenerateField : MonoBehaviour
{
    [Header("SetEssential!!!")]
    [SerializeField] string thisPlayerTag;
    [SerializeField] GameObject hexColliderPrefab;
    [SerializeField] GameObject hexAstralColliderPrefab;


    void Start()
    {
        GenerateAstralField();
        GenerateEntireField();
    }

    void GenerateAstralField()
    {
        if (thisPlayerTag == "Player")
        {
            foreach (Vertex vertex in GridSystem.Instance.Grids.Vertices)
            {
                if (vertex.Coordinate.z < 0)
                {
                    GameObject go = Instantiate(hexAstralColliderPrefab);
                    SetLayerRecursively(go, "PlayerAstralField");
                    go.transform.SetParent(transform);
                    go.transform.position = vertex.Coordinate;
                }
            }
        }
        else if (thisPlayerTag == "OpponentAI")
        {
            foreach (Vertex vertex in GridSystem.Instance.Grids.Vertices)
            {
                if (vertex.Coordinate.z > 0)
                {
                    GameObject go = Instantiate(hexAstralColliderPrefab);
                    SetLayerRecursively(go, "OpponentAstralField");
                    go.transform.SetParent(transform);
                    go.transform.position = vertex.Coordinate;
                }
            }
        }
    }
    void GenerateEntireField()
    {
        if (thisPlayerTag == "Player")
        {
            foreach (Vertex vertex in GridSystem.Instance.Grids.Vertices)
            {
                GameObject go = Instantiate(hexColliderPrefab);
                SetLayerRecursively(go, "PlayerField");
                go.transform.SetParent(transform);
                go.transform.position = vertex.Coordinate;
            }
        }
        else if (thisPlayerTag == "OpponentAI")
        {
            foreach (Vertex vertex in GridSystem.Instance.Grids.Vertices)
            {
                GameObject go = Instantiate(hexColliderPrefab);
                SetLayerRecursively(go, "OpponentField");
                go.transform.SetParent(transform);
                go.transform.position = vertex.Coordinate;
            }
        }
    }
    void SetLayerRecursively(GameObject obj, string layer)
    {
        obj.layer = LayerMask.NameToLayer(layer);
        foreach (Transform child in obj.transform)
        {
            child.gameObject.layer = LayerMask.NameToLayer(layer);
        }
    }
}
