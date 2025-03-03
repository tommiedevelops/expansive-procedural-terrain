using UnityEngine;

public class PerlinNoise : MonoBehaviour {
    private float noiseScale = 0.1f;  // Controls the frequency of the noise
    private float heightMultiplier = 2f; // Controls the height variation
    private int octaves = 1;
    private float persistance = 0.5f;
    private float lacunarity = 1f;
    private int seed = 0;
    private Vector2 offsetV2 = Vector2.zero;
    AnimationCurve heightCurve;

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

        // No noise required
        if (noiseScale == 0) return;

        mesh = meshFilter.sharedMesh;
        Vector3[] vertices = mesh.vertices;
        Vector2[] octaveOffsets = GenerateOctaveOffsets(octaves, seed, offsetV2);

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

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

                float sampleX = (x / noiseScale + octaveOffsets[j].x) * frequency;
                float sampleY = (z / noiseScale - octaveOffsets[j].y) * frequency;

                float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

                noiseHeight += perlinValue * amplitude;
                amplitude *= persistance;
                frequency *= lacunarity;

                vertices[i].y = noiseHeight;
            }

            // Handle height Curve
            vertices[i].y *= heightCurve.Evaluate(vertices[i].y);

            // Handle height multiplier
            vertices[i].y *= heightMultiplier;


            if (noiseHeight > maxNoiseHeight) maxNoiseHeight = noiseHeight;
            else if (noiseHeight < minNoiseHeight) minNoiseHeight = noiseHeight;
        }

        mesh.vertices = vertices;
        mesh.RecalculateNormals(); // Update normals for correct lighting
        RenormalizeHeightMap(mesh, maxNoiseHeight, minNoiseHeight);
        meshFilter.sharedMesh = mesh;
    }

    // Helpers
    private void RenormalizeHeightMap(Mesh mesh, float maxNoiseHeight, float minNoiseHeight) {
        /* 2D Perlin Noise Helper Function
           Re-normalizes noise map to be between 0 and 1.
        */
        for(int i = 0; i < mesh.vertices.Length; i++)
            mesh.vertices[i].y = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, mesh.vertices[i].y);
    }
    private Vector2[] GenerateOctaveOffsets(int octaves, int seed, Vector2 offsetV2) {
        /* Generates offsets for each octave based on the provided offset value */
        const int MIN = -100000;
        const int MAX = 100000;
        var prng = new System.Random(seed);
        var octaveOffsets = new Vector2[octaves];

        for (int i = 0; i < octaves; i++) {
            float offsetX = prng.Next(MIN, MAX) + offsetV2.x;
            float offsetY = prng.Next(MIN, MAX) + offsetV2.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        return octaveOffsets;
    }
    // Setters
    public void SetHeightCurve(AnimationCurve heightCurve) => this.heightCurve = heightCurve;
    public void SetNoiseScale(float noiseScale) => this.noiseScale = noiseScale;
    public void SetHeightMultiplier(float heightMultiplier) => this.heightMultiplier = heightMultiplier;

    public void SetLacunarity(float lacunarity) => this.lacunarity = lacunarity;

    public void SetPersistance(float persistance) => this.persistance = persistance;

    public void SetOctaves(int octaves) => this.octaves = octaves;

    public void SetSeed(int seed) => this.seed = seed;

    public void SetOffsetV2(Vector2 offsetV2) => this.offsetV2 = offsetV2;
}
