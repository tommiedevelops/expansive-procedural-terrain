using System;
using TerrainGenerator.NoiseSystem;
using UnityEngine;
using UnityEngine.UIElements;

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

    public static class PlaneMeshGenerator {
        public static Mesh GeneratePlaneMeshFromHeightMap(HeightMap heightMap, SquareMeshData squareMeshData) {
            var vertices = GenerateVertexGridFromHeightMap(heightMap, squareMeshData);
            var triangles = GenerateTriangleArray(squareMeshData.NumVerticesPerSide);

            // Assign mesh properties
            Mesh mesh = new() {
                //name = $"Plane Mesh: Dim: {width} x {length}. Scale: {scale}",
                vertices = vertices,
                triangles = triangles,
            };

            mesh.RecalculateNormals();

            return mesh;
        }
        public static Vector3[] GenerateVertexGridFromHeightMap(HeightMap heightMap, SquareMeshData meshData) {
            var numVerts = meshData.NumVerticesPerSide;
            var distBetweenPoints = meshData.DistanceBetweenPoints;

            var totalVertices = numVerts * numVerts;
            var vertices = new Vector3[totalVertices];

            var vertexIndex = 0;
            for (var z = 0; z < numVerts; z++) {
                for (var x = 0; x < numVerts; x++) {
                    vertices[vertexIndex] = new Vector3(x * distBetweenPoints,
                        heightMap.GetPoint(x, z),
                        z * distBetweenPoints);
                    vertexIndex++;
                }
            }

            return vertices;
        }
        public static int[] GenerateTriangleArray(int numVertsPerSide) {

            int totalTrianglePoints = (numVertsPerSide - 1) * (numVertsPerSide - 1) * 6;

            int[] triangles = new int[totalTrianglePoints];

            // Generate triangles
            int triIndex = 0;
            for (int z = 0; z < (numVertsPerSide - 1); z++) {
                for (int x = 0; x < (numVertsPerSide - 1); x++) {
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
        internal static void GenerateVertices(float lengthX, float lengthZ, int resolutionX, int resolutionZ, Vector3 originWS, ref Vector3[] vertices, Func<float, float, float> heightFunc) {

            float distanceBetweenXPoints = lengthX / resolutionX;
            float distanceBetweenZPoints = lengthZ / resolutionZ;

            for (int x = 0; x < resolutionX; x++)
                for (int z = 0; z < resolutionZ; z++) {
                    float xCoord = (x - originWS.x) * distanceBetweenXPoints;
                    float zCoord = (z - originWS.z) * distanceBetweenZPoints;
                    float yCoord = heightFunc(xCoord, zCoord);

                    vertices[z * resolutionX + x] = new Vector3(xCoord, yCoord, zCoord);
                }
        }
        internal static void GenerateTriangles(int resolutionX, int resolutionZ, ref int[] triangles) {
            int triIndex = 0;
            for (int z = 0; z < resolutionZ - 1; z++)
                for (int x = 0; x < resolutionX - 1; x++) {
                    int bottomLeft = z * resolutionX + x;
                    int bottomRight = bottomLeft + 1;
                    int topLeft = bottomLeft + resolutionX;
                    int topRight = topLeft + 1;

                    triangles[triIndex++] = bottomLeft;
                    triangles[triIndex++] = topLeft;
                    triangles[triIndex++] = topRight;

                    triangles[triIndex++] = bottomLeft;
                    triangles[triIndex++] = topRight;
                    triangles[triIndex++] = bottomRight;
                }
        }
        internal static void GenerateUVs(int resolutionX, int resolutionZ, ref Vector2[] uvs) {
            for (int x = 0; x < resolutionX; x++)
                for (int z = 0; z < resolutionZ; z++) {
                    uvs[z * resolutionX + x] = new Vector2((float)x / (resolutionX - 1), (float)z/(resolutionZ - 1));
                }
        }
        public static Mesh GeneratePlaneMesh(float lengthX,
                                             float lengthZ,
                                             int resolutionX,
                                             int resolutionZ,
                                             Vector3 originWS,
                                             Func<float, float, float> heightFunc) {

            int vertexCount = resolutionZ * resolutionX;
            int trianglesCount = (resolutionX - 1) * (resolutionZ - 1) * 6;

            Vector3[] vertices = new Vector3[vertexCount];
            int[] triangles = new int[trianglesCount];
            Vector2[] uvs = new Vector2[vertexCount];

            GenerateVertices(lengthX, lengthZ, resolutionX, resolutionZ, originWS, ref vertices, heightFunc);
            GenerateTriangles(resolutionX, resolutionZ, ref triangles);
            GenerateUVs(resolutionX, resolutionZ, ref uvs);

            Mesh mesh = new() {
                vertices = vertices,
                triangles = triangles,
                uv = uvs
            };

            mesh.RecalculateNormals();

            return mesh;
        }

    } 
}
