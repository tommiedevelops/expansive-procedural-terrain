using System;
using UnityEngine;
using UnityEngine.UIElements;

public static class PlaneMeshGenerator {

    public struct MeshData {
        public int numVerticesX;
        public int numVerticesZ;
        public float meshLength;

        public MeshData(int numVerticesX, int numVerticesZ, float meshLength) {
            this.numVerticesX = numVerticesX;
            this.numVerticesZ = numVerticesZ;
            this.meshLength = meshLength;
        }
    }
    public static Mesh GeneratePlaneMesh(MeshData meshData) {
        // You need to create instantiate a meshData struct first and then pass it into
        // this function.

        int width = meshData.numVerticesX;
        int length = meshData.numVerticesZ;
        float scale = meshData.meshLength;

        var vertices = GenerateVertexGrid(scale, scale, width, length);

        // Generate triangles
        var triangles = GenerateTriangleArray(vertices, width, length);

        // Assign mesh properties
        Mesh mesh = new() {
            //name = $"Plane Mesh: Dim: {width} x {length}. Scale: {scale}",
            vertices = vertices,
            triangles = triangles,
        };

        mesh.RecalculateNormals();

        return mesh;
    }

    public static Vector3[] GenerateVertexGrid(float worldSpaceXLength, float worldSpaceZLength, int numXVerts, int numZVerts) {
        float distBetweenXPoints = worldSpaceXLength / (numXVerts-1);
        float distBetweenZPoints = worldSpaceZLength / (numZVerts-1);

        int totalVertices = numXVerts * numZVerts;
        Vector3[] vertices = new Vector3[totalVertices];

        int vertexIndex = 0;

        for(int z = 0; z < numZVerts; z++) {
            for(int x = 0; x < numXVerts; x++) {
                vertices[vertexIndex] = new Vector3(x * distBetweenXPoints, 0f, z * distBetweenZPoints);
                vertexIndex++;
            }
        }

        return vertices;
    }
    public static int[] GenerateTriangleArray(Vector3[] vertexArray, int numXVerts, int numZVerts) {

        int totalTrianglePoints = (numXVerts - 1) * (numZVerts - 1) * 6;

        int[] triangles = new int[totalTrianglePoints];

        // Generate triangles
        int triIndex = 0;
        for (int z = 0; z < (numZVerts-1); z++) {
            for (int x = 0; x < (numXVerts-1); x++) {
                int bottomLeft = z * numXVerts + x;
                int bottomRight = bottomLeft + 1;
                int topLeft = bottomLeft + numZVerts;
                int topRight = topLeft + 1;

                triangles[triIndex++] = bottomLeft;
                triangles[triIndex++] = topLeft;
                triangles[triIndex++] = topRight;

                triangles[triIndex++] = bottomLeft;
                triangles[triIndex++] = topRight;
                triangles[triIndex++] = bottomRight;
            }
        }

        return triangles;
    }
}
