using UnityEngine;

public class FiniteTerrain : MonoBehaviour {

    [SerializeField] Material terrainMaterial;
    [SerializeField] NoiseSettings noiseSettings;
    private void Start() {
        Mesh newMesh = PlaneMeshGenerator.GeneratePlaneMesh(new PlaneMeshGenerator.MeshData(100, 100, 1));
        PerlinNoise.ApplyPerlinNoise(Vector2.zero, ref newMesh, noiseSettings);
        GameObject terrain = new GameObject("Terrain", typeof(MeshFilter), typeof(MeshRenderer));
        terrain.GetComponent<MeshFilter>().mesh = newMesh;
        terrain.GetComponent<MeshRenderer>().material = terrainMaterial;
     }
}
