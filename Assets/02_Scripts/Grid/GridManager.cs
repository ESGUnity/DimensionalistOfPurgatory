using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
public class Vertex
{
    public Vector3 Coordinate { get; private set; } // 그리드의 중심 좌표
    public GameObject AstralOnGrid; // 그리드 위에 존재하는 영체(게임오브젝트)
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
            Debug.Log($"나 여기 있어~~{Coordinate}");
        }
    }
    public override bool Equals(object obj) // 딕셔너리 등의 Contains 등에 쓰이는 내장 메서드. 좌표의 오차범위가 epsilon 이내라면 같은 객체라고 판단하도록 Equals 메서드를 임의 수정했다.
                                            // float을 쓰면 어쩔 수 없이 오차범위가 생기기 때문에 별도로 오버라이드하여 메서드를 수정했다.    
    {
        float epsilon = 0.0001f;

        if (obj is Vertex vertex)
        {
            return Mathf.Abs(Coordinate.x - vertex.Coordinate.x) < epsilon && Mathf.Abs(Coordinate.y - vertex.Coordinate.y) < epsilon && Mathf.Abs(Coordinate.z - vertex.Coordinate.z) < epsilon;
        }

        return false;
    }

    public override int GetHashCode() // C#에선 딕셔너리 등에서 키나 밸류를 찾을 때, 빠르게 동작하기 위해 Equals 실행 전에 각 객체의 해쉬코드를 먼저 비교한다.
                                      // 이 해쉬코드를 오버라이드해서 좌표의 근사값을 통해서 정하도록(좌표의 오차범위가 epsilon 이내라면 같은 해쉬코드를 가지도록) 임의 수정하였다.
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
    public List<Vertex> Vertices = new(); // 정점 리스트
    public Dictionary<Vertex, List<Vertex>> Adjacencies = new(); // 간선 리스트. 가중치는 필요없으니 정점과 이어지는 정점 리스트로 간선을 대채한다.

    public GridGraph() // 생성자
    {

    }

    public Vertex AddVertex(Vector3 coordinate) // 정점 추가 메서드
    {
        Vertex vertex = new Vertex(coordinate); // 매개변수 Vector3로 새로운 정점 선언

        if (!Vertices.Contains(vertex)) // 중복되는 vertex를 걸러내는 조건문
        {
            Vertices.Add(vertex); // 정점 리스트에 새로운 정점 추가
            Adjacencies[vertex] = new List<Vertex>(); // 새로운 정점에 이어지는 간선 리스트 추가
        }

        return vertex;
    }

    public void AddEdge(Vertex v, Vertex w) // 정점 간 간선 추가 메서드
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

    Vector3[] directions = new Vector3[] // 육각형의 6방향으로 좌표를 이동하기 위한 벡터 배열
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

        Vertex originVertex = Grids.AddVertex(new Vector3(0, 0, 0)); // 육각형 그리드가 6방향으로 뻗어나갈 원점. 0, 0, 0을 원점으로 하였다.
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

    public void CreateHexBFS(int executionNumber, Vertex vertex) // BFS를 통해 육각형 그리드를 그래프로서 생성. 이 그리드는 육각형 모양으로 커진다. executionNumber는 재귀 실행 횟수.
    {
        int depth = 0;
        Queue<Vertex> queue = new();
        HashSet<Vertex> visitedVertex = new();

        queue.Enqueue(vertex);
        visitedVertex.Add(vertex);

        while (depth < executionNumber) // BFS는 좋은 해결책이 아니었던 것 같다. 가장 테두리의 그리드에는 정점이 연결되지 않아서 복잡한 로직으로 추가로 연결해야했다.
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
                    Vertex newVertex = Grids.AddVertex(newCoord); // 중복된 좌표를 피하면서 정점을 추가
                    Grids.AddEdge(currentVertex, newVertex); // 정점 간 간선 추가

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
    public Vertex GetGridPosFromWorldPos(Vector3 worldPos) // 연산이 너무 많으니 추후 조정 예정
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
    public Vertex FindTargetVertex(Vertex requesterVertex, string targetTag, out int depth) // 목표 및 거리 찾기 메서드
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

                if (vertex.AstralOnGrid != null && vertex.AstralOnGrid.tag == targetTag) // 문제 발견 : vertex.AstralOnGrid는 null이 아닌데 null이라고 뜬다. // BFS는 잘 작동한다. 버그로 못찾은 경우 항상 61번 탐색한다.
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

            if (targetVertex != null) // 이중 반복문이라 추가적인 break문이 필요 // targetVertex가 정해지면 반복문을 더 돌거나 depth를 추가하면 안 된다.
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








//public void CreateHexRecursion(int executionNumber, Vertex vertex) // 재귀를 통해 육각형 그리드를 그래프로서 생성. 이 그리드는 육각형 모양으로 커진다. executionNumber는 재귀 실행 횟수.
//{
//    if (executionNumber <= 0)
//    {
//        return;
//    }

//    // 각 방향으로 새로운 정점을 추가하고 재귀 호출
//    foreach (var direction in directions)
//    {
//        Vector3 newCoord = vertex.Coordinate + direction;
//        Vertex newVertex = Grids.AddVertex(newCoord); // 중복된 좌표를 피하면서 정점을 추가
//        Grids.AddEdge(vertex, newVertex); // 정점 간 간선 추가
//        CreateHexRecursion(executionNumber - 1, newVertex); // 다음 단계로 재귀 호출
//    }
//}


//public List<Vector3> GridVisualBFS(int prayRange, Vertex startVertex)
//{
//    List<Vector3> vertices = new List<Vector3>();

//    //Queue<Vertex> queue = new Queue<Vertex>();
//    //startVertex.Visited = true;
//    //queue.Enqueue(startVertex);

//    //while (!(Vector3.Distance(queue.Peek().Coordinate, startVertex.Coordinate) > prayRange * Mathf.Sqrt(3))) // 큐를 Peek한 Vertex의 좌표와 시작 정점 좌표 거리를 구할 때, 기도 범위보다 크다면 반환
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