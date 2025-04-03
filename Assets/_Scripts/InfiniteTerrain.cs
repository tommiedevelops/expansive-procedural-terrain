using UnityEngine;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using System;

public class InfiniteTerrain : MonoBehaviour {
    // TODO: IMPLEMENT QUAD TREE FOR EXPANSIVE TERRAIN 


    // DOES THIS RLY NEED TO BE A SINGLETON?
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

    // Dictionary for storing terrainChunks based on their coordinate in chunk space
    private Dictionary<Vector2, TerrainChunk> terrainChunks = new();

    // Unity core methods
    private void Awake() {
        if (Instance == null) Instance = this;
        ValidateNoiseSettings(); 
    }
    private void Update() {
        // Every frame

        // Update the viewer's position in chunk coordinates
        viewerChunkCoords = new Vector2(Mathf.FloorToInt((float)viewer.position.x / CHUNK_SIZE), Mathf.FloorToInt((float)viewer.position.z / CHUNK_SIZE));

        // Generate or update the chunks within range of the viewer
        for (int yOffset = -chunksRenderDistance; yOffset <= chunksRenderDistance; yOffset++) {
            for (int xOffset = -chunksRenderDistance; xOffset <= chunksRenderDistance; xOffset++) {
                var specificChunkCoords = new Vector2(viewerChunkCoords.x + xOffset, viewerChunkCoords.y + yOffset);

                if (terrainChunks.ContainsKey(specificChunkCoords)) {
                    // THIS IS THE EXPENSIVE OPERATION
                    //terrainChunks[specificChunkCoords].UpdateTerrainChunkActiveState(viewer.position, renderDistance);
                    //terrainChunks[specificChunkCoords].UpdateTerrainChunkLOD(CalculateLODIndex(specificChunkCoords));
                } else {
                    terrainChunks.Add(specificChunkCoords, new TerrainChunk(specificChunkCoords, CalculateLODIndex(specificChunkCoords), noiseSettings));
                }
            }
        }
    }
    private void OnValidate() {
        // I should really be configuring the noise separately then 
        // running it on here.
        ValidateNoiseSettings();
        //UpdateNoise();
    }
    
    // HELPERS
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


    // Getter
    public Vector2 GetViewerPosition() { return new Vector2(viewer.position.x, viewer.position.z); }
    public float GetRenderDistance() { return renderDistance; }

}
