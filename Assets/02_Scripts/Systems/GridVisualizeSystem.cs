using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GridVisualizeSystem : MonoBehaviour
{
    public GameObject HexPrefab;

    List<GameObject> astralGridVisualList = new();
    List<GameObject> prayGridVisualList = new();

    private void Start()
    {
        CreateAstralGridVisual();
        CreatePrayGridVisual();
        OnPrayGridVisual(false);
        OnAstralGridVisual(true);
    }
    public void CreateAstralGridVisual()
    {
        foreach (Vertex vertex in GridSystem.Instance.Grids.Vertices)
        {
            if (vertex.Coordinate.z < 0)
            {
                GameObject go = Instantiate(HexPrefab);
                go.transform.position = vertex.Coordinate + new Vector3(0, 0.2f, 0);
                astralGridVisualList.Add(go);
            }
        }
    }
    public void CreatePrayGridVisual()
    {
        foreach (Vertex vertex in GridSystem.Instance.Grids.Vertices)
        {
            GameObject go = Instantiate(HexPrefab);
            go.transform.position = vertex.Coordinate + new Vector3(0, 0.2f, 0);
            prayGridVisualList.Add(go);
        }
    }
    public void OnAstralGridVisual(bool turnOn)
    {
        if (!turnOn)
        {
            foreach (GameObject go in astralGridVisualList)
            {
                go.SetActive(false);
            }
        }
        else
        {
            foreach (GameObject go in astralGridVisualList)
            {
                go.SetActive(true);
            }
        }
    }
    public void OnPrayGridVisual(bool turnOn)
    {
        if (!turnOn)
        {
            foreach (GameObject go in prayGridVisualList)
            {
                go.SetActive(false);
            }
        }
        else
        {
            foreach (GameObject go in prayGridVisualList)
            {
                go.SetActive(true);
            }
        }
    }
}
