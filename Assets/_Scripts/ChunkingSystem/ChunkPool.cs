using UnityEngine;
using System.Collections.Generic;
using System;

namespace ChunkingSystem {
    public class ChunkPool {

        Queue<GameObject> chunks;

        public ChunkPool() {
            chunks = new Queue<GameObject>();
        }

        public void EnqueueChunk(GameObject chunk) {
            chunks.Enqueue(chunk);
        }

        public GameObject DequeueChunk() {
            return chunks.Count > 0 ? chunks.Dequeue() : null;
        }
    }
}