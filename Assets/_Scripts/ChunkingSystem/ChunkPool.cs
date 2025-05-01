using UnityEngine;
using System.Collections.Generic;
using System;

namespace ChunkingSystem {
    public class ChunkPool {

        Queue<GameObject> chunkQueue;

        public ChunkPool() {
            chunkQueue = new Queue<GameObject>();
        }

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