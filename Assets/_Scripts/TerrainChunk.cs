using UnityEngine;
using System;
using System.Threading;
using UnityEngine.AI;
using static PlaneMeshGenerator;
public class TerrainChunk {
    /* Terrain Chunk class responsible for its own thread to generate itself */

    // Fields
    #region Fields
    // CONSTS
    private const float DEFAULT_SCALE = 1.0f;

    // VARS
    private const int CHUNK_SIZE = RealTerrainManager.MAX_NUM_VERTICES_PER_SIDE;
    private readonly GameObject chunkObject;
    Bounds bounds; // Used for retrieving distance from chunk edge to viewer

    Vector2 chunkCoords;
    int levelOfDetailIndex;
    NoiseSettings noiseSettings;
    #endregion

    // CONSTRUCTOR
    public TerrainChunk(Vector2 chunkCoords, int levelOfDetailIndex, NoiseSettings noiseSettings) {
        /* PARAM DESCRIPTIONS
         * viewerChunkCoords: The viewer's position in Chunk Coordinates (World coordinates / CHUNK_SIZE)
         * levelOfDetailIndex: The index of the factors of CHUNK_SIZE to use in generating the chunk
         */

        this.chunkCoords = chunkCoords;
        this.levelOfDetailIndex = levelOfDetailIndex;
        this.noiseSettings = noiseSettings;

        // Creating the Mesh
        Mesh newMesh = GenerateMesh(chunkCoords, levelOfDetailIndex, noiseSettings);

        // Creating the GameObject
        chunkObject = new GameObject("Chunk", typeof(MeshFilter), typeof(MeshRenderer));
        chunkObject.GetComponent<MeshFilter>().mesh = newMesh;
        chunkObject.GetComponent<MeshRenderer>().material = UnityEngine.Rendering.GraphicsSettings.defaultRenderPipeline.defaultMaterial;
        chunkObject.transform.position = new Vector3(chunkCoords.x, 0f, chunkCoords.y) * CHUNK_SIZE;

        // Creating the Bounds
        var viewerChunkCoordsCenter = new Vector2(chunkCoords.x + 0.5f, chunkCoords.y + 0.5f);
        bounds = new Bounds(viewerChunkCoordsCenter * CHUNK_SIZE, Vector2.one * (float)CHUNK_SIZE / 2);
    }

    // METHODS
    private static Mesh GenerateMesh(Vector2 chunkCoords, int levelOfDetailIndex, NoiseSettings noiseSettings) {
        int resolutionScale = RealTerrainManager.FACTORS_OF_MAX_NUM_VERTICES_PER_SIDE[levelOfDetailIndex];
        Mesh newMesh = PlaneMeshGenerator.GeneratePlaneMesh(new MeshData(CHUNK_SIZE / resolutionScale, CHUNK_SIZE / resolutionScale, DEFAULT_SCALE * resolutionScale));
        noiseSettings.width = CHUNK_SIZE / resolutionScale;
        noiseSettings.length = CHUNK_SIZE / resolutionScale;
        PerlinNoise.ApplyPerlinNoise(chunkCoords * CHUNK_SIZE, ref newMesh, noiseSettings);
        return newMesh;
    }
    public void UpdateTerrainChunkActiveState(Vector3 viewerPosition, float renderDistance) {
        // Calculate the distance from the bounds of the chunk to viewer position
        float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
        bool isVisible = viewerDstFromNearestEdge <= renderDistance;
        SetVisible(isVisible);
    }
    public void UpdateNoise() {
        Mesh newMesh = GenerateMesh(chunkCoords, levelOfDetailIndex, noiseSettings);
        PerlinNoise.ApplyPerlinNoise(chunkCoords * CHUNK_SIZE, ref newMesh, noiseSettings);
        chunkObject.GetComponent<MeshFilter>().mesh = newMesh;
    }

    public void UpdateTerrainChunkLOD(int LODIndex) {
        // EXXY
        Mesh newMesh = GenerateMesh(chunkCoords * CHUNK_SIZE, LODIndex, noiseSettings);

        chunkObject.GetComponent<MeshFilter>().mesh = newMesh;
    }
    
    // SETTERS & GETTERS
    private void SetVisible(bool isVisible) {
        chunkObject.SetActive(isVisible);
    }
}
