using System;
using _Scripts.NoiseSystem;
using UnityEngine;

namespace _Scripts.ChunkingSystem
{
    public static class PlaneMeshGenerator
    {

        public struct MeshData
        {
            public readonly int numVerticesX;
            public readonly int numVerticesZ;
            public readonly float meshLength;

            public MeshData(int numVerticesX, int numVerticesZ, float meshLength)
            {
                this.numVerticesX = numVerticesX;
                this.numVerticesZ = numVerticesZ;
                this.meshLength = meshLength;
            }
        }

        public static Mesh GenerateFlatPlaneMesh(MeshData meshData)
        {
            // You need to create instantiate a meshData struct first and then pass it into
            // this function.

            int width = meshData.numVerticesX;
            int length = meshData.numVerticesZ;
            float scale = meshData.meshLength;

            var vertices = GenerateVertexGrid(scale, scale, width, length);

            // Generate triangles
            var triangles = GenerateTriangleArray(vertices, width, length);

            // Assign mesh properties
            Mesh mesh = new()
            {
                //name = $"Plane Mesh: Dim: {width} x {length}. Scale: {scale}",
                vertices = vertices,
                triangles = triangles,
            };

            mesh.RecalculateNormals();

            return mesh;
        }

        public static Mesh GeneratePlaneMeshFromHeightMap(HeightMap heightMap, MeshData meshData)
        {
            if (heightMap.GetGridWidth() != meshData.numVerticesX) throw new Exception("Widths dont match");
            if (heightMap.GetGridLength() != meshData.numVerticesZ) throw new Exception("Lengths dont match");
            
            int width  = meshData.numVerticesX;
            int length = meshData.numVerticesZ;
            float scale = meshData.meshLength;

            var vertices = GenerateVertexGridFromHeightMap(heightMap, scale, scale, width, length);
            var triangles = GenerateTriangleArray(vertices, width, length);
            
            // Assign mesh properties
            Mesh mesh = new()
            {
                //name = $"Plane Mesh: Dim: {width} x {length}. Scale: {scale}",
                vertices = vertices,
                triangles = triangles,
            };

            mesh.RecalculateNormals();

            return mesh;
        }
        
        public static Vector3[] GenerateVertexGrid(float worldSpaceXLength, float worldSpaceZLength, int numXVerts,
            int numZVerts)
        {
            var distBetweenXPoints = worldSpaceXLength / (numXVerts - 1);
            var distBetweenZPoints = worldSpaceZLength / (numZVerts - 1);

            var totalVertices = numXVerts * numZVerts;
            var vertices = new Vector3[totalVertices];

            var vertexIndex = 0;

            for (var z = 0; z < numZVerts; z++)
            {
                for (var x = 0; x < numXVerts; x++)
                {
                    vertices[vertexIndex] = new Vector3(x * distBetweenXPoints, 0f, z * distBetweenZPoints);
                    vertexIndex++;
                }
            }

            return vertices;
        }

        
        public static Vector3[] GenerateVertexGridFromHeightMap(HeightMap heightMap, float worldSpaceXLength, float worldSpaceZLength, int numXVerts,
            int numZVerts)
        {
            var distBetweenXPoints = worldSpaceXLength / (numXVerts - 1);
            var distBetweenZPoints = worldSpaceZLength / (numZVerts - 1);

            var totalVertices = numXVerts * numZVerts;
            var vertices = new Vector3[totalVertices];

            var vertexIndex = 0;

            for (var z = 0; z < numZVerts; z++)
            {
                for (var x = 0; x < numXVerts; x++)
                {
                    vertices[vertexIndex] = new Vector3(x * distBetweenXPoints,
                        heightMap.GetPoint(x,z),
                        z * distBetweenZPoints);
                    vertexIndex++;
                }
            }

            return vertices;
        }
        public static int[] GenerateTriangleArray(Vector3[] vertexArray, int numXVerts, int numZVerts)
        {

            int totalTrianglePoints = (numXVerts - 1) * (numZVerts - 1) * 6;

            int[] triangles = new int[totalTrianglePoints];

            // Generate triangles
            int triIndex = 0;
            for (int z = 0; z < (numZVerts - 1); z++)
            {
                for (int x = 0; x < (numXVerts - 1); x++)
                {
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
}
