using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.ChunkingSystem {
    public class ChunkPool {
        private readonly Dictionary<float, Queue<GameObject>> _chunkPool = new();
        public Queue<GameObject> GetChunkQueue(float sideLength) {
            return _chunkPool[sideLength];
        }
        public void RecycleChunk(GameObject chunk, float sideLength)
        {
            if (!_chunkPool.ContainsKey(sideLength))
                _chunkPool[sideLength] = new Queue<GameObject>();
            
            _chunkPool[sideLength].Enqueue(chunk);
        }
        public GameObject RequestChunk(float sideLength)
        {
            _chunkPool.TryGetValue(sideLength, out var chunkQueue);
            return chunkQueue?.Count > 0 ? chunkQueue.Dequeue() : null;
        }
    }
}