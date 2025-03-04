using UnityEngine;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine.UIElements;

public class InfiniteTerrain : MonoBehaviour {

    public static InfiniteTerrain Instance { get; private set; }

    public const int CHUNK_SIZE = 240;
    [SerializeField] Transform viewer;
    [SerializeField] float renderDistance = CHUNK_SIZE;
    [SerializeField] public Material redMaterial;

    int chunksRenderDistance;
    Vector2 viewerChunkCoords;

    private Dictionary<Vector2, TerrainChunkV2> terrainChunks = new();

    private void Awake() {
        if (Instance == null) Instance = this;
    }
    private void Start() {
        UpdateVariables();
    }

    private void Update() {

        UpdateVariables();
        for (int yOffset = -chunksRenderDistance; yOffset <= chunksRenderDistance; yOffset++) {
            for(int xOffset = -chunksRenderDistance; xOffset <= chunksRenderDistance; xOffset++) {
                var specificChunkCoords = new Vector2(viewerChunkCoords.x + xOffset, viewerChunkCoords.y + yOffset);

                if (terrainChunks.ContainsKey(specificChunkCoords)) {
                    terrainChunks[specificChunkCoords].UpdateTerrainChunk();
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

    // Getter
    public Vector2 GetViewerPosition() { return new Vector2(viewer.position.x, viewer.position.z); }
    public float GetRenderDistance() { return renderDistance; }

}

public class TerrainChunkV2 {
    private const float DEFAULT_SCALE = 1.0f;
    private const int CHUNK_SIZE = InfiniteTerrain.CHUNK_SIZE;
    private readonly GameObject chunkObject;

    Bounds bounds;

    public TerrainChunkV2(Vector2 viewerChunkCoords) {
        Mesh newMesh = PlaneMeshGenerator.GeneratePlaneMesh(CHUNK_SIZE, CHUNK_SIZE, DEFAULT_SCALE);
        chunkObject = new GameObject("Chunk", typeof(MeshFilter), typeof(MeshRenderer));
        chunkObject.GetComponent<MeshFilter>().mesh = newMesh;
        // use default material for now
        chunkObject.GetComponent<MeshRenderer>().material = UnityEngine.Rendering.GraphicsSettings.defaultRenderPipeline.defaultMaterial;
        chunkObject.transform.position = new Vector3(viewerChunkCoords.x, 0f, viewerChunkCoords.y) * CHUNK_SIZE;
        var viewerChunkCoordsCenter = new Vector2(viewerChunkCoords.x + 0.5f, viewerChunkCoords.y + 0.5f);
        bounds = new Bounds(viewerChunkCoordsCenter * CHUNK_SIZE, Vector2.one * (float)CHUNK_SIZE/2);
    }

    public void UpdateTerrainChunk() {
        float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(InfiniteTerrain.Instance.GetViewerPosition()));
        bool isVisible = viewerDstFromNearestEdge <= InfiniteTerrain.Instance.GetRenderDistance();
        SetVisible(isVisible);
    }

    private void SetVisible(bool isVisible) {
        chunkObject.SetActive(isVisible);
    }
}
