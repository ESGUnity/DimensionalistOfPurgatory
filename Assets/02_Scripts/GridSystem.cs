using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
public class Vertex
{
    public Vector3 Coordinate; // 그리드의 중심 좌표
    public GameObject AstralOnGrid; // 그리드 위에 존재하는 영체(게임오브젝트)

    public Vertex(Vector3 coordinate)
    {
        Coordinate = coordinate;
        AstralOnGrid = null;
    }

    public override bool Equals(object obj) // 딕셔너리의 Contains 등에 쓰이는 내장 메서드
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
    public List<Vertex> Vertices = new(); // 정점 리스트
    public Dictionary<Vertex, List<Vertex>> Adjacencies = new(); // 간선 리스트. 가중치는 필요없으니 정점과 이어지는 정점 리스트로 간선을 대채한다.

    public GridGraph() // 생성자
    {

    }

    public Vertex AddVertex(Vector3 coordinate) // 정점 추가 메서드
    {
        Vertex vertex = new Vertex(coordinate); // 매개변수 Vector3로 새로운 정점 선언
        if (!Vertices.Contains(vertex))
        {
            Vertices.Add(vertex); // 정점 리스트에 새로운 정점 추가
            Adjacencies[vertex] = new List<Vertex>(); // 새로운 정점에 이어지는 간선 리스트 추가
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = coordinate;  // 큐브 위치 설정
        }
        return vertex;
    }

    public void AddEdge(Vertex v, Vertex w) // 정점 간 간선 추가 메서드
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
    public void CreateHexRecursion(int executionNumber, Vertex vertex) // 재귀를 통한 헥스 그리드 그래프 생성. 이 그래프는 육각형 모양으로 커진다.
                                                                       // 매개변수는 재귀 실행 횟수이다. 재귀를 1번 실행하면 가로로 3칸짜리 그리드가 될 것이고 2번 실행하면 5칸, 3번 실행하면 7칸 짜리 그리드가 될 것이다.
    {
        if (executionNumber <= 0)
        {
            return;
        }

        Vector3[] directions = new Vector3[] // 육각형의 6방향으로 좌표를 이동하기 위한 벡터 배열
        {
            new Vector3(-Mathf.Sqrt(3) / 2f, 0, 3f / 2f),
            new Vector3(Mathf.Sqrt(3) / 2f, 0, 3f / 2f),
            new Vector3(Mathf.Sqrt(3), 0, 0),
            new Vector3(Mathf.Sqrt(3) / 2f, 0, -3f / 2f),
            new Vector3(-Mathf.Sqrt(3) / 2f, 0, -3f / 2f),
            new Vector3(-Mathf.Sqrt(3), 0, 0)
        };

        // 각 방향으로 새로운 정점을 추가하고 재귀 호출
        foreach (var direction in directions)
        {
            Vector3 newCoord = vertex.Coordinate + direction;
            Vertex newVertex = Grids.AddVertex(newCoord); // 중복된 좌표를 피하면서 정점을 추가
            Grids.AddEdge(vertex, newVertex); // 정점 간 간선 추가
            CreateHexRecursion(executionNumber - 1, newVertex); // 다음 단계로 재귀 호출
        }
    }
}