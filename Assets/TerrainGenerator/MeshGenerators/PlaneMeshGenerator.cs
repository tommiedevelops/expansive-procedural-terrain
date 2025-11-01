using TerrainGenerator.NoiseSystem;
using UnityEngine;

namespace TerrainGenerator.MeshGenerators
{
    public struct SquareMeshData
    {
        public readonly int NumVerticesPerSide;
        public readonly float SideLength;
        public readonly float DistanceBetweenPoints;

        public SquareMeshData(int numVerticesPerSide, float sideLength)
        {
            this.NumVerticesPerSide = numVerticesPerSide;
            this.SideLength = sideLength;
            this.DistanceBetweenPoints =  (float)SideLength / (float)(NumVerticesPerSide - 1);
        }
    }

    public static class PlaneMeshGenerator
    {

        public static Mesh GeneratePlaneMeshFromHeightMap(HeightMap heightMap, SquareMeshData squareMeshData)
        {
            var vertices = GenerateVertexGridFromHeightMap(heightMap, squareMeshData);
            var triangles = GenerateTriangleArray(squareMeshData.NumVerticesPerSide);
            
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

        public static Vector3[] GenerateVertexGridFromHeightMap(HeightMap heightMap, SquareMeshData meshData)
        {
            var numVerts =  meshData.NumVerticesPerSide;
            var distBetweenPoints = meshData.DistanceBetweenPoints;
            
            var totalVertices = numVerts * numVerts;
            var vertices = new Vector3[totalVertices];

            var vertexIndex = 0;
            for (var z = 0; z < numVerts; z++)
            {
                for (var x = 0; x < numVerts; x++)
                {
                    vertices[vertexIndex] = new Vector3(x * distBetweenPoints,
                        heightMap.GetPoint(x,z),
                        z * distBetweenPoints);
                    vertexIndex++;
                }
            }

            return vertices;
        }
        public static int[] GenerateTriangleArray(int numVertsPerSide)
        {

            int totalTrianglePoints = (numVertsPerSide - 1) * (numVertsPerSide - 1) * 6;

            int[] triangles = new int[totalTrianglePoints];

            // Generate triangles
            int triIndex = 0;
            for (int z = 0; z < (numVertsPerSide - 1); z++)
            {
                for (int x = 0; x < (numVertsPerSide - 1); x++)
                {
                    int bottomLeft = z * numVertsPerSide + x;
                    int bottomRight = bottomLeft + 1;
                    int topLeft = bottomLeft + numVertsPerSide;
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
