using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
public class Vertex
{
    public Vector3 Coordinate; // �׸����� �߽� ��ǥ
    public GameObject AstralOnGrid; // �׸��� ���� �����ϴ� ��ü(���ӿ�����Ʈ)
    public bool Visited;

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
        if (!Vertices.Contains(vertex))
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
            v.Visited = false;
        }
    }
    public object Clone()
    {
        return new GridGraph { Vertices = this.Vertices, Adjacencies = this.Adjacencies };
    }
}

public class GridSystem : MonoBehaviour
{
    public GridGraph Grids = new GridGraph();
    public GameObject HexPrefab;

    private static GridSystem instance;
    public static GridSystem Instance
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
        CreateHexRecursion(4, originVertex);
    }
    public void CreateHexRecursion(int executionNumber, Vertex vertex) // ��͸� ���� ������ �׸��带 �׷����μ� ����. �� �׸���� ������ ������� Ŀ����. executionNumber�� ��� ���� Ƚ��.
    {
        if (executionNumber <= 0)
        {
            return;
        }

        Vector3[] directions = new Vector3[] // �������� 6�������� ��ǥ�� �̵��ϱ� ���� ���� �迭
        {
            new Vector3(-Mathf.Sqrt(3) / 2f, 0, 3f / 2f),
            new Vector3(Mathf.Sqrt(3) / 2f, 0, 3f / 2f),
            new Vector3(Mathf.Sqrt(3), 0, 0),
            new Vector3(Mathf.Sqrt(3) / 2f, 0, -3f / 2f),
            new Vector3(-Mathf.Sqrt(3) / 2f, 0, -3f / 2f),
            new Vector3(-Mathf.Sqrt(3), 0, 0)
        };

        // �� �������� ���ο� ������ �߰��ϰ� ��� ȣ��
        foreach (var direction in directions)
        {
            Vector3 newCoord = vertex.Coordinate + direction;
            Vertex newVertex = Grids.AddVertex(newCoord); // �ߺ��� ��ǥ�� ���ϸ鼭 ������ �߰�
            Grids.AddEdge(vertex, newVertex); // ���� �� ���� �߰�
            CreateHexRecursion(executionNumber - 1, newVertex); // ���� �ܰ�� ��� ȣ��
        }
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
    public List<Vector3> GridVisualBFS(int prayRange, Vertex startVertex)
    {
        List<Vector3> vertices = new List<Vector3>();

        //Queue<Vertex> queue = new Queue<Vertex>();
        //startVertex.Visited = true;
        //queue.Enqueue(startVertex);

        //while (!(Vector3.Distance(queue.Peek().Coordinate, startVertex.Coordinate) > prayRange * Mathf.Sqrt(3))) // ť�� Peek�� Vertex�� ��ǥ�� ���� ���� ��ǥ �Ÿ��� ���� ��, �⵵ �������� ũ�ٸ� ��ȯ
        //{
        //    Vertex vertex = queue.Dequeue();
        //    vertices.Add(vertex.Coordinate);

        //    foreach (Vertex v in Grids.Adjacencies[vertex])
        //    {
        //        if (!v.Visited)
        //        {
        //            v.Visited = true;;
        //            queue.Enqueue(v);
        //        }
        //    }
        //}
        ////Grids.ClearVisited();

        //return vertices;

        vertices.Add(startVertex.Coordinate);
        return vertices;
    }
}