using System;
using UnityEngine;

namespace TerrainGeneratorAsset
{
    public static class PlaneMeshGenerator {
        public static int[] GenerateTriangleArray(int numVertsPerSide) {

            int totalTrianglePoints = (numVertsPerSide - 1) * (numVertsPerSide - 1) * 6;
            int[] triangles = new int[totalTrianglePoints];

            // Generate triangles
            int triIndex = 0;
            for (int z = 0; z < (numVertsPerSide - 1); z++)
            for (int x = 0; x < (numVertsPerSide - 1); x++) 
            {
                 int botLeft  = z * numVertsPerSide + x;
                 int botRight = botLeft + 1;
                 int topLeft  = botLeft + numVertsPerSide;
                 int topRight = topLeft + 1;

                 triangles[triIndex++] = botLeft;
                 triangles[triIndex++] = topLeft;
                 triangles[triIndex++] = topRight;

                 triangles[triIndex++] = botLeft;
                 triangles[triIndex++] = topRight;
                 triangles[triIndex++] = botRight;
            }

            return triangles;
        }
        internal static void GenerateVertices(float lengthX, float lengthZ, int resolutionX, int resolutionZ,ref Vector3[] vertices, Func<float, float, float> heightFunc) {

            float distanceBetweenXPoints = lengthX / resolutionX;
            float distanceBetweenZPoints = lengthZ / resolutionZ;

            float startX = -0.5f * lengthX;
            float startZ = -0.5f * lengthZ;

            for (int x = 0; x < resolutionX; x++)
            for (int z = 0; z < resolutionZ; z++) 
            {
                 float xCoord = startX + x * distanceBetweenXPoints;
                 float zCoord = startZ + z * distanceBetweenZPoints;
                 float yCoord = heightFunc(xCoord, zCoord);

                 vertices[z * resolutionX + x] = new Vector3(xCoord, yCoord, zCoord);
            }
        }
        internal static void GenerateTriangles(int resolutionX, int resolutionZ, ref int[] triangles) {
            int triIndex = 0;
            for (int z = 0; z < resolutionZ - 1; z++)
            for (int x = 0; x < resolutionX - 1; x++) 
            {
                    int botLeft  = z * resolutionX + x;
                    int botRight = botLeft + 1;
                    int topLeft  = botLeft + resolutionX;
                    int topRight = topLeft + 1;

                    triangles[triIndex++] = botLeft;
                    triangles[triIndex++] = topLeft;
                    triangles[triIndex++] = topRight;

                    triangles[triIndex++] = botLeft;
                    triangles[triIndex++] = topRight;
                    triangles[triIndex++] = botRight;
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
                                             Func<float, float, float> heightFunc) {

            // Generates plane mesh using heightFunc for y values in Object Space. 
            // The bottom left point will be (-lengthX, -lengthZ) and the top right point will be (lengthX, lengthZ)
            int vertexCount = resolutionZ * resolutionX;
            int triIndexCount = (resolutionX - 1) * (resolutionZ - 1) * 6; // every triangle has 3 indicies

            Vector3[] vertices  = new Vector3[vertexCount];
            int[]     triangles = new int[triIndexCount];
            Vector2[] uvs       = new Vector2[vertexCount];

            GenerateVertices(lengthX, lengthZ, resolutionX, resolutionZ, ref vertices, heightFunc);
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
