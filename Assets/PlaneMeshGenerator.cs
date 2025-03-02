using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class PlaneMeshGenerator : MonoBehaviour {
    private int width = 10;
    private int length = 10;
    private float scale = 1f;

    private void Awake() {
        if (width < 0) width = 0;
        if (length < 0) length = 0;

        GeneratePlaneMesh();
    }
    public void GeneratePlaneMesh() {
        Mesh mesh = new Mesh();
        mesh.name = "Generated Plane";

        int verticesCountX = width + 1;
        int verticesCountZ = length + 1;
        int totalVertices = verticesCountX * verticesCountZ;
        int totalTriangles = width * length * 6;

        Vector3[] vertices = new Vector3[totalVertices];
        int[] triangles = new int[totalTriangles];
        Vector2[] uv = new Vector2[totalVertices];

        // Generate vertices and UVs
        for (int z = 0; z < verticesCountZ; z++) {
            for (int x = 0; x < verticesCountX; x++) {
                int index = z * verticesCountX + x;
                vertices[index] = new Vector3(x * scale, 0, z * scale);
                uv[index] = new Vector2((float)x / width, (float)z / length);
            }
        }

        // Generate triangles
        int triIndex = 0;
        for (int z = 0; z < length; z++) {
            for (int x = 0; x < width; x++) {
                int bottomLeft = z * verticesCountX + x;
                int bottomRight = bottomLeft + 1;
                int topLeft = bottomLeft + verticesCountX;
                int topRight = topLeft + 1;

                triangles[triIndex++] = bottomLeft;
                triangles[triIndex++] = topLeft;
                triangles[triIndex++] = topRight;

                triangles[triIndex++] = bottomLeft;
                triangles[triIndex++] = topRight;
                triangles[triIndex++] = bottomRight;
            }
        }

        // Assign mesh properties
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateNormals();

        // Assign mesh to a MeshFilter and MeshRenderer
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (!meshFilter) {
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }

        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (!meshRenderer) {
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
        }

        meshFilter.sharedMesh = mesh;

    }

    // Setters
    public void SetWidth(int width) => this.width = width;
    public void SetLength(int length) => this.length = length;
    public void SetScale(float scale) => this.scale = scale;
}
