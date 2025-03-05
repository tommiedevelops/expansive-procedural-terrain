using UnityEngine;
using System.Collections.Generic;
using System;

public class EndlessTerrain : MonoBehaviour {

    #region Declarations
    public class TerrainChunkOLD {
        /* The properties here encompass a single Terrain Chunk, defined
         * as a 240x240 unit grid of vertices
         * 
         * TODO: Refactor using C# event system.
         */

        GameObject meshObject;
        Vector2 position;
        Bounds bounds;
        LODInfo[] detailLevels;
        LODMesh[] lodMeshes;

        //MapData mapData;
        bool mapDataReceived;
        int previousLODIndex = -1;

        MeshRenderer meshRenderer;
        MeshFilter meshFilter;

        public TerrainChunkOLD(Vector2 coord, int size, Transform parent, LODInfo[] detailLevels, Material material) {
            this.detailLevels = detailLevels;
            position = coord * size;
            bounds = new Bounds(position, Vector2.one * size);

            var positionV3 = new Vector3(position.x, 0f, position.y);
            meshObject = new GameObject("Terrain Chunk");
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshRenderer.material = material;
            meshObject.transform.position = positionV3;
            meshObject.transform.parent = parent;
            SetVisible(false);

            lodMeshes = new LODMesh[detailLevels.Length];
            for (int i = 0; i < detailLevels.Length; i++) {
                lodMeshes[i] = new LODMesh(detailLevels[i].lod);
            }

           //TerrainGenerator.Instance.RequestMapData(OnMapDataReceived);
        }

        public void UpdateTerrainChunk() {

            if (!mapDataReceived) return;

            float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
            bool isVisible = viewerDstFromNearestEdge <= maxViewDst;

            if (isVisible) {
                int lodIndex = 0;

                for (int i = 0; i < detailLevels.Length - 1; i++) {
                    if (viewerDstFromNearestEdge > detailLevels[i].visibleDstThreshold) {
                        lodIndex = i + 1;
                    } else {
                        break;
                    }
                }

                if (lodIndex != previousLODIndex) {
                    LODMesh lodMesh = lodMeshes[lodIndex];
                    if (lodMesh.hasMesh) {
                        meshFilter.mesh = lodMesh.mesh;
                        previousLODIndex = lodIndex;
                    } else if (!lodMesh.hasRequestedMesh) {
                        //lodMesh.RequestMesh(mapData);
                    }
                }
            }
            SetVisible(isVisible);


        }

        //void OnMapDataReceived(MapData mapData) {
        //    this.mapData = mapData;
        //    mapDataReceived = true;
        //}

        public void SetVisible(bool isVisible) {
            meshObject.SetActive(isVisible);
        }

        public bool IsVisible() {
            return meshObject.activeSelf;
        }

    }
    #endregion
    #region Fields
    [Header("References")]
    [SerializeField] Transform viewer;
    [SerializeField] Material mapMaterial;
    [SerializeField] LODInfo[] detailLevels;

    [Header("Variables")]
    public static float maxViewDst;
    private int chunkSize;
    private int chunksVisibleInViewDst;
    public static Vector2 viewerPosition;

    Dictionary<Vector2, TerrainChunkOLD> terrainChunkDict = new();
    List<TerrainChunkOLD> terrainChunksVisibleLastUpdate = new();
    #endregion

    // Unity Methods
    private void Start() {
        maxViewDst = detailLevels[detailLevels.Length - 1].visibleDstThreshold;
        chunkSize = TerrainGenerator.MAP_CHUNK_SIZE_PLUSONE - 1;
        chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / chunkSize);
    }

    private void Update() {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);
        UpdateVisibleChunks();
    }

    // Endless Terrain Methods
    private void UpdateVisibleChunks() {

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
                    terrainChunkDict.Add(viewedChunkCoord, new TerrainChunkOLD(viewedChunkCoord, chunkSize, transform, detailLevels, mapMaterial));
                }
            }
        }
    }

    
}

public class LODMesh {
    public Mesh mesh;
    public bool hasRequestedMesh;
    public bool hasMesh;
    int lod;

    public LODMesh(int lod) {
        this.lod = lod;
    }

    void OnMeshDataReceived(MeshData meshData) {
        mesh = meshData.CreateMesh();
        hasMesh = true;
    }
    //public void RequestMesh(MapData mapData) {
    //    hasRequestedMesh = true;
    //    MapGenerator.Instance.RequestMeshData(mapData, lod, OnMeshDataReceived);
    //}
}

[Serializable]
public struct LODInfo {
    public int lod;
    public float visibleDstThreshold;
}