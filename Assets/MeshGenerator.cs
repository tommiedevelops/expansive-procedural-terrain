using UnityEngine;

public static class MeshGenerator {
    public static MeshData GenerateTerrainMeshDataFromHeightMap(float[,] noiseMap, NoiseSettings settings) {

        // I don't get the below transformation. Why is this important? Maybe try without it after done. //demo
        MeshData meshData = GenerateMeshData(noiseMap, settings);

        return meshData;

    }

    private static MeshData GenerateMeshData(float[,] noiseMap, NoiseSettings settings) {

        // Transform position so it is centred abou the origin
        float topLeftX = (settings.width - 1) / -2f;
        float topLeftZ = (settings.length - 1) / 2f;

        // Index to use in case of mesh simplification
        int meshSimplificationIncrement = (settings.previewLOD == 0) ? 1 : settings.previewLOD * 2;
        int verticesPerLine = (settings.width - 1) / meshSimplificationIncrement + 1;

        int vertexIndex = 0;
        var meshData = new MeshData(verticesPerLine, verticesPerLine);

        for (int y = 0; y < settings.length; y += meshSimplificationIncrement) {
            for (int x = 0; x < settings.width; x += meshSimplificationIncrement) {
                meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, settings.amplitudeEnvelope.Evaluate(noiseMap[x, y]) * settings.heightMultiplier, topLeftZ - y);
                meshData.uvs[vertexIndex] = new Vector2(x / (float)settings.width, y / (float)settings.length);

                if (x < settings.width - 1 && y < settings.width - 1) {
                    meshData.AddTriangle(vertexIndex, vertexIndex + verticesPerLine + 1, vertexIndex + verticesPerLine);
                    meshData.AddTriangle(vertexIndex + verticesPerLine + 1, vertexIndex, vertexIndex + 1);
                }

                vertexIndex++;
            }
        }

        return meshData;
    }
}

public class MeshData {
    /* This class converts MeshData into a Mesh Object */
    public int[] triangles;
    public Vector3[] vertices;
    public Vector2[] uvs;

    int triangleIndex = 0;
    public MeshData(int meshWidth, int meshHeight) {
        vertices = new Vector3[meshWidth * meshHeight];
        uvs = new Vector2[meshWidth * meshHeight];
        triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
    }
    public void AddTriangle(int a, int b, int c) {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }
    public Mesh CreateMesh() {
        var mesh = new Mesh {
            vertices = vertices,
            triangles = triangles,
            uv = uvs
        };
        mesh.RecalculateNormals();
        return mesh;
    }
}