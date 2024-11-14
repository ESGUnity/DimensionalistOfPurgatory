using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GenerateHex : MonoBehaviour
{
    int polygonPoints = 6;
    float outerRadius = 0.95f;

    Mesh mesh;
    Vector3[] vertices;
    int[] indices;

    void Awake()
    {
        mesh = new Mesh();

        GetComponent<MeshFilter>().mesh = mesh;
        DrawFilled(polygonPoints, outerRadius);
    }

    void DrawFilled(int sides, float radius)
    {
        vertices = GetCircumferencePoints(sides, radius);
        indices = DrawFilledIndices(vertices);
        GeneratePolygon(vertices, indices);
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

    void GeneratePolygon(Vector3[] vertices, int[] indices)
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = indices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }
}
