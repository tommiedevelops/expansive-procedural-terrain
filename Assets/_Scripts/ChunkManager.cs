using UnityEngine;
using System.Collections.Generic;
using UnityEditorInternal;
using System.Linq;
public class ChunkManager : MonoBehaviour {

    [SerializeField] TerrainGenerator terrainGenerator;
    ChunkPool chunkPool;

    Dictionary<QuadNode, GameObject> activeChunks;

    private void Awake() {
        chunkPool = new ChunkPool();
    }
    private void Start() {
        terrainGenerator.OnLeafNodesReady += HandleLeafNodes;
    }

    private void HandleCulledNodes() { 
        // For handling culled nodes from the terrain generator
        // Deload the node, tell the chunk pool so it can store the node in the pool
    }
    private void HandleLeafNodes(IEnumerable<QuadNode> leafNodes) {
        foreach(QuadNode leafNode in leafNodes) {
            // Try to get a chunk from the chunk pool
            float sideLength = leafNode.GetSideLength();

            // Calculate leafNode LOD
            int lod = 0; // NOT IMPLEMENTED

            ChunkPool.ChunkRequestResult result = chunkPool.RequestChunkFromPool(sideLength, lod); // NOT IMPLEMENTED
            
            // ChunkPool doesn't have what we need
            if(!result.HasChunk) {
                // Request a new chunk to be generated from ChunkGenerator
                // chunk = new chunk generated
                // chunk.SetActive(true)
                // activeChunks[leafNode] = chunk
            } else {
                // pool has what we need
                //GameObject chunk = result.Chunk;
                // activeChunks[leafnode] = chunk;
                //chunk.SetActive(true);
            }
        }
    }

    private void OnDisable() {
        if (terrainGenerator != null)
            terrainGenerator.OnLeafNodesReady -= HandleLeafNodes;
    }

    
}
public class LODSelector {

}
public class ChunkPool {
    // This is a very rough draft but can make it a lot better by
    // sorting the availableChunks based on its ChunkType or maybe
    // by having multiple pools

    // maybe even a dictionary: Dictionary< (int size, int lod), Queue<GameObject> chunkPool>

    Dictionary<(float sideLength, int levelOfDetail), Queue<ChunkRequestResult>> chunkPool = new();

    public struct ChunkRequestResult {
        public GameObject Chunk;
        public ChunkType ReturnType;

        public ChunkRequestResult(GameObject chunk, ChunkType returnType) {
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
    public ChunkRequestResult RequestChunkFromPool(float sideLength, int levelOfDetail) {
        
        return chunkPool[(sideLength, levelOfDetail)].Count > 0
                ? chunkPool[(sideLength, levelOfDetail)].Dequeue()                         
                :new ChunkRequestResult(null, ChunkType.NULL);
    }
}
public class ChunkGenerator {

}