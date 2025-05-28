using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.ChunkingSystem {
    public class ChunkPool {
        private readonly Dictionary<(float, Vector2), Queue<GameObject>> _chunkPool = new();
        public Queue<GameObject> GetChunkQueue(float sideLength, Vector2 botLeftPoint) {
            return _chunkPool[(sideLength, botLeftPoint)];
        }
        public void RecycleChunk(GameObject chunk, float sideLength, Vector2 botLeftPoint)
        {
            if (!_chunkPool.ContainsKey((sideLength, botLeftPoint)))
                _chunkPool[(sideLength, botLeftPoint)] = new Queue<GameObject>();
            
            _chunkPool[(sideLength, botLeftPoint)].Enqueue(chunk);
        }
        public GameObject RequestChunk(float sideLength, Vector2 botLeftPoint)
        {
            _chunkPool.TryGetValue((sideLength, botLeftPoint), out var chunkQueue);
            return chunkQueue?.Count > 0 ? chunkQueue.Dequeue() : null;
        }
    }
}