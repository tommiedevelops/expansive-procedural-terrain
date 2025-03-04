using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEditor.Rendering;
using System.Runtime.CompilerServices;

public class InfiniteTerrain : MonoBehaviour {
    private const int CHUNK_SIZE = 240;
    [SerializeField] Transform viewer;
    [SerializeField] float renderDistance = CHUNK_SIZE;
    int chunksRenderDistance;
    Vector2 viewerChunkCoords;

    private Dictionary<Vector2, TerrainChunkV2> terrainChunks = new();

    private void Start() {
        UpdateVariables();
    }

    private void Update() {

        UpdateVariables();
        for (int yOffset = -chunksRenderDistance; yOffset <= chunksRenderDistance; yOffset++) {
            for(int xOffset = -chunksRenderDistance; xOffset <= chunksRenderDistance; xOffset++) {
                var specificChunkCoords = new Vector2(viewerChunkCoords.x + xOffset, viewerChunkCoords.y + yOffset);

                if (terrainChunks.ContainsKey(specificChunkCoords)) {
                    // do something nice
                } else {
                    terrainChunks.Add(specificChunkCoords, new TerrainChunkV2(specificChunkCoords));
                }
            }
        }

    }
    private void UpdateVariables() {
        viewerChunkCoords = new Vector2(Mathf.FloorToInt((float)viewer.position.x / CHUNK_SIZE), Mathf.FloorToInt((float)viewer.position.z / CHUNK_SIZE));
        chunksRenderDistance = Mathf.RoundToInt(renderDistance / CHUNK_SIZE);
    }

}

public class TerrainChunkV2 {
    private const float DEFAULT_SCALE = 1.0f;
    private const int CHUNK_SIZE = 240;
    private readonly GameObject chunkObject;

    public TerrainChunkV2(Vector2 viewerChunkCoords) {
        Mesh newMesh = PlaneMeshGenerator.GeneratePlaneMesh(CHUNK_SIZE, CHUNK_SIZE, DEFAULT_SCALE);
        chunkObject = new GameObject("Chunk", typeof(MeshFilter), typeof(MeshRenderer));
        chunkObject.GetComponent<MeshFilter>().mesh = newMesh;
        // use default material for now
        chunkObject.GetComponent<MeshRenderer>().material = UnityEngine.Rendering.GraphicsSettings.defaultRenderPipeline.defaultMaterial;
        chunkObject.transform.position = new Vector3(viewerChunkCoords.x, 0f, viewerChunkCoords.y) * CHUNK_SIZE;
    }

}
