using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
public class Vertex
{
    public Vector3 Coordinate; // �׸����� �߽� ��ǥ
    public GameObject AstralOnGrid; // �׸��� ���� �����ϴ� ��ü(���ӿ�����Ʈ)

    public Vertex(Vector3 coordinate)
    {
        Coordinate = coordinate;
        AstralOnGrid = null;
    }

    public override bool Equals(object obj) // ��ųʸ��� Contains � ���̴� ���� �޼���
    {
        float epsilon = 0.0001f;
        if (obj is Vertex vertex)
        {
            return Mathf.Abs(Coordinate.x - vertex.Coordinate.x) < epsilon && Mathf.Abs(Coordinate.y - vertex.Coordinate.y) < epsilon && Mathf.Abs(Coordinate.z - vertex.Coordinate.z) < epsilon;
        }
        return false;
    }

    public override int GetHashCode()
    {
        float epsilon = 0.0001f;

        int hash = 17;

        hash = 31 * hash + Mathf.RoundToInt(Coordinate.x / epsilon);
        hash = 31 * hash + Mathf.RoundToInt(Coordinate.y / epsilon);
        hash = 31 * hash + Mathf.RoundToInt(Coordinate.z / epsilon);

        return hash;
    }
}

public class GridGraph
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
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = coordinate;  // ť�� ��ġ ����
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
}

public class GridSystem : MonoBehaviour
{
    public GridGraph Grids = new GridGraph();
    public GameObject Prefab;

    private void Start()
    {
        Vertex originVertex = Grids.AddVertex(new Vector3(0, 0, 0));
        CreateHexRecursion(4, originVertex);
    }
    public void CreateHexRecursion(int executionNumber, Vertex vertex) // ��͸� ���� �� �׸��� �׷��� ����. �� �׷����� ������ ������� Ŀ����.
                                                                       // �Ű������� ��� ���� Ƚ���̴�. ��͸� 1�� �����ϸ� ���η� 3ĭ¥�� �׸��尡 �� ���̰� 2�� �����ϸ� 5ĭ, 3�� �����ϸ� 7ĭ ¥�� �׸��尡 �� ���̴�.
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
}