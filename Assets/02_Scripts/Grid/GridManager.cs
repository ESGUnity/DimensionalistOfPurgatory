using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
public class Vertex
{
    public Vector3 Coordinate { get; private set; } // �׸����� �߽� ��ǥ
    public GameObject AstralOnGrid; // �׸��� ���� �����ϴ� ��ü(���ӿ�����Ʈ)
    public bool Visited = false;

    public Vertex(Vector3 coordinate)
    {
        Coordinate = coordinate;
        AstralOnGrid = null;
    }
    public void Alram()
    {
        if (AstralOnGrid != null)
        {
            Debug.Log($"�� ���� �־�~~{Coordinate}");
        }
    }
    public override bool Equals(object obj) // ��ųʸ� ���� Contains � ���̴� ���� �޼���. ��ǥ�� ���������� epsilon �̳���� ���� ��ü��� �Ǵ��ϵ��� Equals �޼��带 ���� �����ߴ�.
                                            // float�� ���� ��¿ �� ���� ���������� ����� ������ ������ �������̵��Ͽ� �޼��带 �����ߴ�.    
    {
        float epsilon = 0.0001f;

        if (obj is Vertex vertex)
        {
            return Mathf.Abs(Coordinate.x - vertex.Coordinate.x) < epsilon && Mathf.Abs(Coordinate.y - vertex.Coordinate.y) < epsilon && Mathf.Abs(Coordinate.z - vertex.Coordinate.z) < epsilon;
        }

        return false;
    }

    public override int GetHashCode() // C#���� ��ųʸ� ��� Ű�� ����� ã�� ��, ������ �����ϱ� ���� Equals ���� ���� �� ��ü�� �ؽ��ڵ带 ���� ���Ѵ�.
                                      // �� �ؽ��ڵ带 �������̵��ؼ� ��ǥ�� �ٻ簪�� ���ؼ� ���ϵ���(��ǥ�� ���������� epsilon �̳���� ���� �ؽ��ڵ带 ��������) ���� �����Ͽ���.
    {
        float epsilon = 0.0001f;

        int hash = 17;

        hash = 31 * hash + Mathf.RoundToInt(Coordinate.x / epsilon);
        hash = 31 * hash + Mathf.RoundToInt(Coordinate.y / epsilon);
        hash = 31 * hash + Mathf.RoundToInt(Coordinate.z / epsilon);

        return hash;
    }
}

public class GridGraph : ICloneable
{
    public List<Vertex> Vertices = new(); // ���� ����Ʈ
    public Dictionary<Vertex, List<Vertex>> Adjacencies = new(); // ���� ����Ʈ. ����ġ�� �ʿ������ ������ �̾����� ���� ����Ʈ�� ������ ��ä�Ѵ�.

    public GridGraph() // ������
    {

    }

    public Vertex AddVertex(Vector3 coordinate) // ���� �߰� �޼���
    {
        Vertex vertex = new Vertex(coordinate); // �Ű����� Vector3�� ���ο� ���� ����

        if (!Vertices.Contains(vertex)) // �ߺ��Ǵ� vertex�� �ɷ����� ���ǹ�
        {
            Vertices.Add(vertex); // ���� ����Ʈ�� ���ο� ���� �߰�
            Adjacencies[vertex] = new List<Vertex>(); // ���ο� ������ �̾����� ���� ����Ʈ �߰�
        }

        return vertex;
    }

    public void AddEdge(Vertex v, Vertex w) // ���� �� ���� �߰� �޼���
    {
        if (!Adjacencies[v].Contains(w) && !Adjacencies[w].Contains(v))
        {
            Adjacencies[v].Add(w);
            Adjacencies[w].Add(v);
            //GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //go.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            //go.transform.position = (v.Coordinate + w.Coordinate) / 2;
            //go.GetComponent<MeshRenderer>().material.color = Color.green;
        }
    }

    public void ClearVisited()
    {
        foreach (Vertex v in Vertices)
        {
            if (v.Visited)
            {
                v.Visited = false;
            }
        }
    }
    public object Clone()
    {
        return new GridGraph { Vertices = this.Vertices, Adjacencies = this.Adjacencies };
    }
}

public class GridManager : MonoBehaviour
{
    public GridGraph Grids = new();

    Vector3[] directions = new Vector3[] // �������� 6�������� ��ǥ�� �̵��ϱ� ���� ���� �迭
    {
        new Vector3(Mathf.Sqrt(3) / 2f, 0, 3f / 2f),
        new Vector3(Mathf.Sqrt(3), 0, 0),
        new Vector3(Mathf.Sqrt(3) / 2f, 0, -3f / 2f),
        new Vector3(-Mathf.Sqrt(3) / 2f, 0, -3f / 2f),
        new Vector3(-Mathf.Sqrt(3), 0, 0),
        new Vector3(-Mathf.Sqrt(3) / 2f, 0, 3f / 2f)
    };
    Vector3[] range2 = new Vector3[]
{
        new Vector3(0, 0, 0),
        new Vector3(-Mathf.Sqrt(3) / 2f, 0, 3f / 2f),
        new Vector3(Mathf.Sqrt(3) / 2f, 0, 3f / 2f),
        new Vector3(Mathf.Sqrt(3), 0, 0),
        new Vector3(Mathf.Sqrt(3) / 2f, 0, -3f / 2f),
        new Vector3(-Mathf.Sqrt(3) / 2f, 0, -3f / 2f),
        new Vector3(-Mathf.Sqrt(3), 0, 0)
};
    Vector3[] range3 = new Vector3[]
{
        new Vector3(0, 0, 0),
        new Vector3(-Mathf.Sqrt(3) / 2f, 0, 3f / 2f),
        new Vector3(Mathf.Sqrt(3) / 2f, 0, 3f / 2f),
        new Vector3(Mathf.Sqrt(3), 0, 0),
        new Vector3(Mathf.Sqrt(3) / 2f, 0, -3f / 2f),
        new Vector3(-Mathf.Sqrt(3) / 2f, 0, -3f / 2f),
        new Vector3(-Mathf.Sqrt(3), 0, 0),
        // 2��° ĭ
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

    private static GridManager instance;
    public static GridManager Instance
    {
        get
        {
            return instance;
        }
    }
    private void Awake()
    {
        if (instance != null) 
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        Vertex originVertex = Grids.AddVertex(new Vector3(0, 0, 0)); // ������ �׸��尡 6�������� ����� ����. 0, 0, 0�� �������� �Ͽ���.
        CreateHexBFS(4, originVertex);
    }
    float c = 3;
    //private void Update()
    //{
    //    c -= Time.deltaTime;

    //    if (c > 0)
    //    {
    //        return;
    //    }
    //    else
    //    {
    //        foreach (var v in Grids.Vertices)
    //        {
    //            v.Alram();
    //        }

    //        c = 1.5f;
    //    }
    //}

    public void CreateHexBFS(int executionNumber, Vertex vertex) // BFS�� ���� ������ �׸��带 �׷����μ� ����. �� �׸���� ������ ������� Ŀ����. executionNumber�� ��� ���� Ƚ��.
    {
        int depth = 0;
        Queue<Vertex> queue = new();
        HashSet<Vertex> visitedVertex = new();

        queue.Enqueue(vertex);
        visitedVertex.Add(vertex);

        while (depth < executionNumber) // BFS�� ���� �ذ�å�� �ƴϾ��� �� ����. ���� �׵θ��� �׸��忡�� ������ ������� �ʾƼ� ������ �������� �߰��� �����ؾ��ߴ�.
        {
            int levelSize = queue.Count;

            for (int i = 0; i < levelSize; i++)
            {
                Vertex currentVertex = queue.Dequeue();

                Vertex lastVertex = null;
                Vertex firstVertex = null;
                int count = 0;
                foreach (var direction in directions)
                {
                    Vector3 newCoord = currentVertex.Coordinate + direction;
                    Vertex newVertex = Grids.AddVertex(newCoord); // �ߺ��� ��ǥ�� ���ϸ鼭 ������ �߰�
                    Grids.AddEdge(currentVertex, newVertex); // ���� �� ���� �߰�

                    queue.Enqueue(newVertex);

                    if (lastVertex == null)
                    {
                        firstVertex = newVertex;
                    }
                    if (lastVertex != null)
                    {
                        Grids.AddEdge(lastVertex, newVertex);
                        count++;
                    }
                    if (count >= 5)
                    {
                        Grids.AddEdge(firstVertex, newVertex);
                    }
                    lastVertex = newVertex;
                }
            }

            depth++;
        }
    }
    public Vertex GetGridPosFromWorldPos(Vector3 worldPos) // ������ �ʹ� ������ ���� ���� ����
    {
        float distance;
        float minimumDistance = float.MaxValue;
        Vertex closestVertex = null;

        foreach (Vertex vertex in Grids.Vertices)
        {
            distance = Vector3.Distance(worldPos, vertex.Coordinate);
            if (distance < minimumDistance)
            {
                minimumDistance = distance;
                closestVertex = vertex;
            }
        }

        return closestVertex;
    }
    public Vertex FindTargetVertex(Vertex requesterVertex, string targetTag, out int depth) // ��ǥ �� �Ÿ� ã�� �޼���
    {
        depth = 0;
        Queue<Vertex> queue = new();
        HashSet<Vertex> visitedVertex = new();
        Vertex targetVertex = null;

        queue.Enqueue(requesterVertex);
        visitedVertex.Add(requesterVertex);
 
        while (queue.Count > 0)
        {
            int levelSize = queue.Count;
            
            for (int i = 0; i < levelSize; i++)
            {
                Vertex vertex = queue.Dequeue();
                //Debug.Log(vertex.Coordinate);

                if (vertex.AstralOnGrid != null && vertex.AstralOnGrid.tag == targetTag) // ���� �߰� : vertex.AstralOnGrid�� null�� �ƴѵ� null�̶�� ���. // BFS�� �� �۵��Ѵ�. ���׷� ��ã�� ��� �׻� 61�� Ž���Ѵ�.
                {
                    //Debug.Log(vertex.AstralOnGrid.name);
                    targetVertex = vertex;
                    break;
                }
                else
                {
                    foreach (Vertex adVertex in Grids.Adjacencies[vertex])
                    {
                        if (!visitedVertex.Contains(adVertex))
                        {
                            queue.Enqueue(adVertex);
                            visitedVertex.Add(adVertex);
                        }
                    }
                }
            }

            if (targetVertex != null) // ���� �ݺ����̶� �߰����� break���� �ʿ� // targetVertex�� �������� �ݺ����� �� ���ų� depth�� �߰��ϸ� �� �ȴ�.
            {
                break;
            }

            depth++;
        }

        return targetVertex;
    }
    public Vertex DecideNextMoveVertex(Vertex start, Vertex target) 
    {
        Vertex moveVertex = null;
        float distance = float.MaxValue;

        foreach (Vertex tempVertex in Grids.Adjacencies[start])
        {
            if (tempVertex == null)
            {
                continue;
            }

            if (tempVertex.AstralOnGrid != null)
            {
                continue;
            }
            else
            {
                if (Vector3.Distance(tempVertex.Coordinate, target.Coordinate) < distance)
                {
                    distance = Vector3.Distance(tempVertex.Coordinate, target.Coordinate);
                    moveVertex = tempVertex;
                }
            }
        }

        return moveVertex;
    }
    public List<GameObject> GetAstralsInRange(Vertex start, int range)
    {
        List<GameObject> targetList = new();

        switch (range)
        {
            case 1:
                targetList.Add(start.AstralOnGrid);
                break;
            case 2:
                foreach (Vector3 direction in range2)
                {
                    Vector3 newCoord = start.Coordinate + direction;
                    Vertex newVertex = Grids.Vertices.Find(v => v.Coordinate.Equals(newCoord));
                    targetList.Add(newVertex.AstralOnGrid);
                }
                break;
            case 3:
                foreach (Vector3 direction in range3)
                {
                    Vector3 newCoord = start.Coordinate + direction;
                    Vertex newVertex = Grids.Vertices.Find(v => v.Coordinate.Equals(newCoord));
                    targetList.Add(newVertex.AstralOnGrid);
                }
                break;
        }

        return targetList;
    }
}








//public void CreateHexRecursion(int executionNumber, Vertex vertex) // ��͸� ���� ������ �׸��带 �׷����μ� ����. �� �׸���� ������ ������� Ŀ����. executionNumber�� ��� ���� Ƚ��.
//{
//    if (executionNumber <= 0)
//    {
//        return;
//    }

//    // �� �������� ���ο� ������ �߰��ϰ� ��� ȣ��
//    foreach (var direction in directions)
//    {
//        Vector3 newCoord = vertex.Coordinate + direction;
//        Vertex newVertex = Grids.AddVertex(newCoord); // �ߺ��� ��ǥ�� ���ϸ鼭 ������ �߰�
//        Grids.AddEdge(vertex, newVertex); // ���� �� ���� �߰�
//        CreateHexRecursion(executionNumber - 1, newVertex); // ���� �ܰ�� ��� ȣ��
//    }
//}


//public List<Vector3> GridVisualBFS(int prayRange, Vertex startVertex)
//{
//    List<Vector3> vertices = new List<Vector3>();

//    //Queue<Vertex> queue = new Queue<Vertex>();
//    //startVertex.Visited = true;
//    //queue.Enqueue(startVertex);

//    //while (!(Vector3.Distance(queue.Peek().Coordinate, startVertex.Coordinate) > prayRange * Mathf.Sqrt(3))) // ť�� Peek�� Vertex�� ��ǥ�� ���� ���� ��ǥ �Ÿ��� ���� ��, �⵵ �������� ũ�ٸ� ��ȯ
//    //{
//    //    Vertex vertex = queue.Dequeue();
//    //    vertices.Add(vertex.Coordinate);

//    //    foreach (Vertex v in Grids.Adjacencies[vertex])
//    //    {
//    //        if (!v.Visited)
//    //        {
//    //            v.Visited = true;;
//    //            queue.Enqueue(v);
//    //        }
//    //    }
//    //}
//    ////Grids.ClearVisited();

//    //return vertices;

//    vertices.Add(startVertex.Coordinate);
//    return vertices;
//}