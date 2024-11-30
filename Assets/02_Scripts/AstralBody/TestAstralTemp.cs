using UnityEngine;

public class TestAstralTemp : MonoBehaviour
{
    [SerializeField] GameObject astralPrefab;
    void Start()
    {
        TempAstralSpawner();
    }


    void TempAstralSpawner()
    {
        GameObject go = Instantiate(astralPrefab);
        go.transform.position = GridManager.Instance.GetGridPosFromWorldPos(new Vector3(-4.7f, 0, 4)).Coordinate;
        go.tag = "Opponent";
        GridManager.Instance.GetGridPosFromWorldPos(new Vector3(-4.7f, 0, 4)).AstralOnGrid = go;

        GameObject go1 = Instantiate(astralPrefab);
        go1.transform.position = GridManager.Instance.GetGridPosFromWorldPos(new Vector3(2.3f, 0, 5)).Coordinate;
        go1.tag = "Opponent";
        GridManager.Instance.GetGridPosFromWorldPos(new Vector3(2.3f, 0, 5)).AstralOnGrid = go1;
    }
}
