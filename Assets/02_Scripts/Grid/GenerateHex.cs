using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GenerateHex : MonoBehaviour
{
    public int polygonPoints = 6;
    public float outerRadius = 0.95f;
    public float innerRadius;

    Mesh mesh;
    Vector3[] vertices;
    int[] indices;

    void Awake()
    {
        mesh = new Mesh();

        GetComponent<MeshFilter>().mesh = mesh;
        if (innerRadius == 0)
        {
            DrawFilled(polygonPoints, outerRadius);
        }
        else
        {
            DrawHollow(polygonPoints, outerRadius, innerRadius);
        }
    }

    void DrawFilled(int sides, float radius)
    {
        vertices = GetCircumferencePoints(sides, radius);
        indices = DrawFilledIndices(vertices);
        GeneratePolygon(vertices, indices);
    }
    int[] DrawFilledIndices(Vector3[] vertices)
    {
        int triangleCount = vertices.Length - 2;
        List<int> indices = new List<int>();

        for (int i = 0; i < triangleCount; i++)
        {
            indices.Add(0);
            indices.Add(i + 2);
            indices.Add(i + 1);
        }
        return indices.ToArray();
    }
    Vector3[] GetCircumferencePoints(int sides, float radius)
    {
        Vector3[] points = new Vector3[sides];
        float anglePerStep = 2 * Mathf.PI * ((float)1 / sides);

        for (int i = 0; i < sides; i++)
        {
            Vector2 point = Vector2.zero;
            float angle = anglePerStep * i;

            point.x = Mathf.Cos(angle) * radius;
            point.y = Mathf.Sin(angle) * radius;

            points[i] = point;
        }
        return points;
    }
    void GeneratePolygon(Vector3[] vertices, int[] indices)
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = indices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }
    void DrawHollow(int sides, float outerRadius, float innerRadius)
    {
        Vector3[] outerPoints = GetCircumferencePoints(sides, outerRadius);
        Vector3[] innerPoints = GetCircumferencePoints(sides, innerRadius);

        List<Vector3> points = new List<Vector3>();
        points.AddRange(outerPoints);
        points.AddRange(innerPoints);

        vertices = points.ToArray();
        indices = DrawHollowIndices(sides);
        GeneratePolygon(vertices, indices);
    }
    int[] DrawHollowIndices(int sides)
    {
        List<int> indices = new List<int>();

        for (int i = 0; i < sides; i++)
        {
            int outerIndex = i;
            int innerIndex = i + sides;

            indices.Add(outerIndex);
            indices.Add(innerIndex);
            indices.Add((outerIndex + 1) % sides);

            indices.Add(innerIndex);
            indices.Add(sides + ((innerIndex + 1) % sides));
            indices.Add((outerIndex + 1) % sides);
        }

        return indices.ToArray();
    }
}
