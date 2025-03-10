using UnityEngine;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using System;

public class InfiniteTerrain : MonoBehaviour {

    public static InfiniteTerrain Instance { get; private set; }

    // Pre calculated constants
    // 240 chosen due to a sufficient number of factors
    public const int CHUNK_SIZE = 240;
    public static readonly int[] CHUNK_SIZE_FACTORS = { 1, 2, 3, 4, 6, 8, 10, 12 };

    [SerializeField] Transform viewer;
    [SerializeField] float renderDistance = CHUNK_SIZE * 4;
    [SerializeField] NoiseSettings noiseSettings;

    int chunksRenderDistance;
    Vector2 viewerChunkCoords;

    private Dictionary<Vector2, TerrainChunk> terrainChunks = new();

    private void Awake() {
        if (Instance == null) Instance = this;
        ValidateNoiseSettings();
        
    }
    private void Start() {
        UpdateVariables();
    }

    private void OnValidate() {
        ValidateNoiseSettings();
        UpdateNoise();
    }

    private void UpdateNoise() {
        foreach( TerrainChunk chunk in terrainChunks.Values) {
            chunk.UpdateNoise();
        }
    }

    private void ValidateNoiseSettings() {
        noiseSettings.width = CHUNK_SIZE;
        noiseSettings.length = CHUNK_SIZE;
        if (noiseSettings.persistance > 1) noiseSettings.persistance = 0.99f;
        if (noiseSettings.persistance < 0) noiseSettings.persistance = 0.01f;
        if (noiseSettings.octaves < 0) noiseSettings.octaves = 0;
        if (noiseSettings.octaves > 6) noiseSettings.octaves = 6;
        if (noiseSettings.lacunarity < 0) noiseSettings.lacunarity = 0.01f;
    }

    private void Update() {
        UpdateVariables();
        UpdateChunks();
    }

    private void UpdateChunks() {
        // O(n^2)
        for (int yOffset = -chunksRenderDistance; yOffset <= chunksRenderDistance; yOffset++) {
            for (int xOffset = -chunksRenderDistance; xOffset <= chunksRenderDistance; xOffset++) {
                var specificChunkCoords = new Vector2(viewerChunkCoords.x + xOffset, viewerChunkCoords.y + yOffset);

                if (terrainChunks.ContainsKey(specificChunkCoords)) {
                    terrainChunks[specificChunkCoords].UpdateTerrainChunk();
                    //terrainChunks[specificChunkCoords].UpdateTerrainChunkLOD(CalculateLODIndex(specificChunkCoords));
                } else {
                    terrainChunks.Add(specificChunkCoords, new TerrainChunk(specificChunkCoords, CalculateLODIndex(specificChunkCoords), noiseSettings));
                }
            }
        }
    }

    private int CalculateLODIndex(Vector2 specificChunkCoords) {
        /* Calculates LOD Index based on dist of chunk from viewer*/
        int levelOfDetailIndex;
        Vector2 viewerToChunkVect = specificChunkCoords * InfiniteTerrain.CHUNK_SIZE - GetViewerPosition();
        float viewerToChunkDist = Mathf.Sqrt(viewerToChunkVect.sqrMagnitude);

        // code below is a bit hard coded. make more customisable when ur bothered (QUAD TREE??)
        if (viewerToChunkDist < renderDistance / 3) levelOfDetailIndex = 3;
        else if (viewerToChunkDist < renderDistance * 2 / 3) levelOfDetailIndex = 5;
        else levelOfDetailIndex = 7;

        return levelOfDetailIndex;
    }

    private void UpdateVariables() {
        viewerChunkCoords = new Vector2(Mathf.FloorToInt((float)viewer.position.x / CHUNK_SIZE), Mathf.FloorToInt((float)viewer.position.z / CHUNK_SIZE));
        chunksRenderDistance = Mathf.RoundToInt(renderDistance / CHUNK_SIZE);
    }

    // Getter
    public Vector2 GetViewerPosition() { return new Vector2(viewer.position.x, viewer.position.z); }
    public float GetRenderDistance() { return renderDistance; }

}

public class TerrainChunk {
    // CONSTS
    private const float DEFAULT_SCALE = 1.0f;

    // VARS
    private const int CHUNK_SIZE = InfiniteTerrain.CHUNK_SIZE;
    private readonly GameObject chunkObject;
    Bounds bounds; // Used for retrieving distance from chunk edge to viewer

    Vector2 chunkCoords;
    int levelOfDetailIndex;
    NoiseSettings noiseSettings;

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

    private static Mesh GenerateMesh(Vector2 chunkCoords, int levelOfDetailIndex, NoiseSettings noiseSettings) {
        int resolutionScale = InfiniteTerrain.CHUNK_SIZE_FACTORS[levelOfDetailIndex];
        Mesh newMesh = PlaneMeshGenerator.GeneratePlaneMesh(CHUNK_SIZE / resolutionScale, CHUNK_SIZE / resolutionScale, DEFAULT_SCALE * resolutionScale);
        noiseSettings.width = CHUNK_SIZE / resolutionScale;
        noiseSettings.length = CHUNK_SIZE / resolutionScale;
        PerlinNoise.ApplyPerlinNoise(chunkCoords * CHUNK_SIZE, ref newMesh, noiseSettings);
        return newMesh;
    }

    // METHODS
    public void UpdateTerrainChunk() {
        float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(InfiniteTerrain.Instance.GetViewerPosition()));
        bool isVisible = viewerDstFromNearestEdge <= InfiniteTerrain.Instance.GetRenderDistance();
        SetVisible(isVisible);
    }
   

    public void UpdateNoise() {
        Mesh newMesh = GenerateMesh(chunkCoords, levelOfDetailIndex, noiseSettings);
        PerlinNoise.ApplyPerlinNoise(chunkCoords * CHUNK_SIZE, ref newMesh, noiseSettings);
        chunkObject.GetComponent<MeshFilter>().mesh = newMesh;
    }

    public void UpdateTerrainChunkLOD(int LODIndex) {
        Mesh newMesh = GenerateMesh(chunkCoords * CHUNK_SIZE, LODIndex, noiseSettings);
        chunkObject.GetComponent<MeshFilter>().mesh = newMesh;
    }
    
    // SETTERS & GETTERS
    private void SetVisible(bool isVisible) {
        chunkObject.SetActive(isVisible);
    }
}
