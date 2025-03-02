using UnityEngine;

public class PerlinNoise : MonoBehaviour {
    private float noiseScale = 0.1f;  // Controls the frequency of the noise
    private float heightMultiplier = 2f; // Controls the height variation
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

        for (int i = 0; i < vertices.Length; i++) {
            float x = vertices[i].x;
            float z = vertices[i].z;

            // Add Perlin noise to the existing Y position
            vertices[i].y += Mathf.PerlinNoise(x * noiseScale, z * noiseScale) * heightMultiplier;
        }

        mesh.vertices = vertices;
        mesh.RecalculateNormals(); // Update normals for correct lighting
        meshFilter.sharedMesh = mesh;
    }

    // Setters
    public void SetNoiseScale(float noiseScale) => this.noiseScale = noiseScale;
    public void SetHeightMultiplier(float heightMultiplier) => this.heightMultiplier = heightMultiplier;
}
