using UnityEngine;
using System.Collections.Generic;
using UnityEditorInternal;
using System.Linq;
using static ChunkPool;
public class ChunkManager : MonoBehaviour {

    [SerializeField] TerrainGenerator terrainGenerator;
    ChunkPool chunkPool;
    Dictionary<QuadNode, GameObject> activeChunks;

    #region Unity Methods
    private void Awake() {
        chunkPool = new ChunkPool();
    }
    private void Start() {
        terrainGenerator.OnLeafNodesReady += HandleLeafNodes;
        terrainGenerator.OnCulledLeafNodesReady += HandleCulledLeafNodes;
    }
    private void OnDisable() {
        if (terrainGenerator != null) {
            terrainGenerator.OnLeafNodesReady -= HandleLeafNodes;
            terrainGenerator.OnCulledLeafNodesReady -= HandleCulledLeafNodes;
        }
    }
    #endregion

    private void HandleCulledLeafNodes(IEnumerable<QuadNode> culledLeafNodes) {
        throw new System.NotImplementedException();
    }
    private void HandleLeafNodes(IEnumerable<QuadNode> leafNodes) {
        foreach(QuadNode leafNode in leafNodes) {

            ChunkPoolRequestResult result = chunkPool.RequestChunkFromPool(leafNode.GetSideLength(), leafNode.GetLOD()); // NOT IMPLEMENTED

            GameObject leafNodeChunk;

            if(!result.HasChunk)
                leafNodeChunk = ChunkGenerator.RequestNewChunk();
            else
                leafNodeChunk = result.Chunk;

            leafNodeChunk.SetActive(true);
            activeChunks[leafNode] = leafNodeChunk;
        }
    }

}
public class ChunkPool {
    // This is a very rough draft but can make it a lot better by
    // sorting the availableChunks based on its ChunkType or maybe
    // by having multiple pools

    // maybe even a dictionary: Dictionary< (int size, int lod), Queue<GameObject> chunkPool>

    Dictionary<(float sideLength, int levelOfDetail), Queue<ChunkPoolRequestResult>> chunkPool = new();

    public struct ChunkPoolRequestResult {
        public GameObject Chunk;
        public ChunkType ReturnType;

        public ChunkPoolRequestResult(GameObject chunk, ChunkType returnType) {
            Chunk = chunk;
            ReturnType = returnType;
        }

        public bool HasChunk => Chunk != null;
    }
    public enum ChunkType {
        MeshIncluded,
        MeshNotIncluded,
        NULL
    }
    public void AddToPool(ChunkType chunkType, GameObject chunk) {
        chunk.SetActive(false);
    }
    public ChunkPoolRequestResult RequestChunkFromPool(float sideLength, int levelOfDetail) {
        
        return chunkPool[(sideLength, levelOfDetail)].Count > 0
                ? chunkPool[(sideLength, levelOfDetail)].Dequeue()                         
                :new ChunkPoolRequestResult(null, ChunkType.NULL);
    }
}
public static class ChunkGenerator {
    public static GameObject RequestNewChunk() {
        return new GameObject();
    }
}