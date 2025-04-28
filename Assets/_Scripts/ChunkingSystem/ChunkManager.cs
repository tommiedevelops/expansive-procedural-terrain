using UnityEngine;
using System.Collections.Generic;
using UnityEditorInternal;
using System.Linq;
using static ChunkingSystem.ChunkPool;
using System;

namespace ChunkingSystem {
    public class ChunkManager {

        ChunkPool chunkPool;
        List<GameObject> chunksToBeRecycled;

        public ChunkManager() {
            this.chunkPool = new ChunkPool();
            this.chunksToBeRecycled = new List<GameObject>();
        }

        public GameObject InstantiateNewGameObject() {
            return new GameObject();
        }

        public void SetChunkPosition(GameObject chunk, Vector3 position) {
            chunk.transform.position = position;
        }

        public GameObject RequestChunkFromChunkPool() {
            return chunkPool.DequeueChunk();
        }

        public void SetChunkPool(ChunkPool chunkPool) {
            this.chunkPool = chunkPool;
        }

        public ChunkPool GetChunkPool() {
            return chunkPool;
        }

        public List<GameObject> GetChunksToBeRecycled() {
            return this.chunksToBeRecycled;
        }

        public void RecycleChunks() {
            foreach(GameObject chunk in chunksToBeRecycled) {
                chunkPool.EnqueueChunk(chunk);
            }
            chunksToBeRecycled.Clear();
        }
    }

}