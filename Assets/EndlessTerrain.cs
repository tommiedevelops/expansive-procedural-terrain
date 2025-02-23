using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

public class EndlessTerrain : MonoBehaviour {

    #region Fields
    [Header("References")]
    [SerializeField] Transform viewer;

    [Header("Variables")]
    public const float maxViewDst = 300;
    private int chunkSize;
    private int chunksVisibleInViewDst;

    Dictionary<Vector2, TerrainChunk> terrainChunkDict = new();
    List<TerrainChunk> terrainChunksVisibleLastUpdate = new();
    #endregion

    public static Vector2 viewerPosition;

    // Methods
    private void Start() {
        chunkSize = MapGenerator.MAP_CHUNK_SIZE_PLUSONE - 1;
        chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / chunkSize);
    }

    private void Update() {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);
        UpdateVisibleChunks();
    }

    void UpdateVisibleChunks() {

        for (int i = 0; i < terrainChunksVisibleLastUpdate.Count; i++) {
            terrainChunksVisibleLastUpdate[i].SetVisible(false);
        }

        terrainChunksVisibleLastUpdate.Clear();

        int currentChunkCoordX = Mathf.RoundToInt(viewer.position.x / chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewer.position.z / chunkSize);

        for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++) {
            for(int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++) {
                var viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                if (terrainChunkDict.ContainsKey(viewedChunkCoord)) {
                    terrainChunkDict[viewedChunkCoord].UpdateTerrainChunk();
                    if(terrainChunkDict[viewedChunkCoord].IsVisible()) {
                        terrainChunksVisibleLastUpdate.Add(terrainChunkDict[viewedChunkCoord]);
                    }

                } else {
                    terrainChunkDict.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, transform));
                }
            }
        }
    }

    public class TerrainChunk {
        private const float DEFAULT_PLANE_SCALE = 10f;
        GameObject meshObject;
        Vector2 position;
        Bounds bounds;
        public TerrainChunk(Vector2 coord, int size, Transform parent) {
            position = coord * size;
            bounds = new Bounds(position, Vector2.one * size);

            Vector3 positionV3 = new Vector3(position.x, 0f, position.y);
            meshObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
            meshObject.transform.position = positionV3;
            meshObject.transform.localScale = Vector3.one * size / DEFAULT_PLANE_SCALE;
            meshObject.transform.parent = parent;
            SetVisible(false);
        }

        public void UpdateTerrainChunk() {
            float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
            bool isVisible = viewerDstFromNearestEdge <= maxViewDst;    
            SetVisible(isVisible);
        }

        public void SetVisible(bool isVisible) {
            meshObject.SetActive(isVisible);
        }

        public bool IsVisible() {
            return meshObject.activeSelf;
        }
    }
}
