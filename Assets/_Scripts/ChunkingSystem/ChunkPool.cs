using UnityEngine;
using System.Collections.Generic;
using System;

namespace ChunkingSystem {
    public class ChunkPool {
        
        Dictionary<(float sideLength, int lod), Queue<GameObject>> chunkPool = new();
        private Queue<GameObject> chunkQueue = new();
        public Queue<GameObject> GetChunkQueue() {
            return chunkQueue;
        }
        
        public void AddChunkToPool(GameObject chunk) {
            chunkQueue.Enqueue(chunk);
        }
        
        public GameObject RequestChunk() {
            return chunkQueue.Count > 0 ? chunkQueue.Dequeue() : null;
        }

        public GameObject RequestChunk(float meshSideLength, int lod) {
            throw new NotImplementedException();
        }
    }
}