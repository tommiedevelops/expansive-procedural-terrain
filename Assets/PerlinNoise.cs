using UnityEngine;

public class PerlinNoise : MonoBehaviour {
    private float noiseScale = 0.1f;  // Controls the frequency of the noise
    private float heightMultiplier = 2f; // Controls the height variation
    private int octaves = 1;
    private float persistance = 0.5f;
    private float lacunarity = 1f;

    private Mesh mesh;

    private void Start() {
        ApplyPerlinNoise();
    }

    public void ApplyPerlinNoise() {
        /* Applies Perlin Noise directly to a meshFilter */
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null || meshFilter.sharedMesh == null) {
            Debug.LogError("No mesh found! Make sure a mesh exists before applying noise.");
            return;
        }

        mesh = meshFilter.sharedMesh;
        Vector3[] vertices = mesh.vertices;

        //float maxNoiseHeight = float.MinValue;
        //float minNoiseHeight = float.MaxValue;

        for (int i = 0; i < vertices.Length; i++) {
            float x = vertices[i].x;
            float z = vertices[i].z;

            // Add Perlin noise to the existing Y position
            vertices[i].y += Mathf.PerlinNoise(x * noiseScale, z * noiseScale) * 2 - 1;

            float amplitude = 1f;
            float frequency = 1f;
            float noiseHeight = 1f;

            // loop through octaves
            for (int j = 0; j < octaves; j++) {

                float sampleX = (x / noiseScale) * frequency;
                float sampleY = (z / noiseScale) * frequency;

                float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

                noiseHeight += perlinValue * amplitude;
                amplitude *= persistance;
                frequency *= lacunarity;

                vertices[i].y = noiseHeight;
            }

            // Handle height multiplier
            vertices[i].y *= heightMultiplier;
        }

        mesh.vertices = vertices;
        mesh.RecalculateNormals(); // Update normals for correct lighting
        meshFilter.sharedMesh = mesh;
    }

    // Setters
    public void SetNoiseScale(float noiseScale) => this.noiseScale = noiseScale;
    public void SetHeightMultiplier(float heightMultiplier) => this.heightMultiplier = heightMultiplier;

    public void SetLacunarity(float lacunarity) => this.lacunarity = lacunarity;

    public void SetPersistance(float persistance) => this.persistance = persistance;

    public void SetOctaves(int octaves) => this.octaves = octaves;
}
