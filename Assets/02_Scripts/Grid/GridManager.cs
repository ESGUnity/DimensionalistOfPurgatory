using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
public class Vertex
{
    public Vector3 Coordinate; // �׸����� �߽� ��ǥ
    public GameObject AstralOnGrid; // �׸��� ���� �����ϴ� ��ü(���ӿ�����Ʈ)
    public bool Visited = false;

    public Vertex(Vector3 coordinate)
    {
        Coordinate = coordinate;
        AstralOnGrid = null;
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
            new Vector3(-Mathf.Sqrt(3) / 2f, 0, 3f / 2f),
            new Vector3(Mathf.Sqrt(3) / 2f, 0, 3f / 2f),
            new Vector3(Mathf.Sqrt(3), 0, 0),
            new Vector3(Mathf.Sqrt(3) / 2f, 0, -3f / 2f),
            new Vector3(-Mathf.Sqrt(3) / 2f, 0, -3f / 2f),
            new Vector3(-Mathf.Sqrt(3), 0, 0)
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

    public Vertex GetGridPosFromWorldPos(Vector3 worldPos) // ������ �ʹ� ������ ���� ���� ����
    {
        float distance = 0;
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
    public Vertex FindTargetVertex(Vertex requesterVertex, string targetTag, out int depth)
    {
        depth = 0;
        Queue<Vertex> queue = new();
        HashSet<Vertex> visitedVertex = new();
        Vertex targetVertex = null;

        visitedVertex.Add(requesterVertex);
        queue.Enqueue(requesterVertex);

        while (queue.Count > 0)
        {
            int levelSize = queue.Count;

            for (int i = 0; i < levelSize; i++)
            {
                Vertex vertex = queue.Dequeue();

                if (vertex.AstralOnGrid != null && vertex.AstralOnGrid.tag == targetTag)
                {
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

            if (targetVertex != null) // ���� �ݺ����̶� �߰����� break���� �ʿ��ߴ�.
            {
                break;
            }

            depth++;
        }

        return targetVertex;
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