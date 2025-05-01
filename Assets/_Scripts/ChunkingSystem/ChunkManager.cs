using System;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;

namespace _Scripts.ChunkingSystem {
    public class ChunkManager {
        
        private ChunkPool _chunkPool = new();
        private readonly List<GameObject> _chunksToBeRecycled = new();
        private readonly List<GameObject> _activeChunks = new();
        public static GameObject InstantiateNewGameObject() { return new GameObject(); }
        public static void SetChunkPosition(GameObject chunk, Vector3 position) { chunk.transform.position = position; }
        public List<GameObject> GetActiveChunks() { return _activeChunks; }
        public List<GameObject> GetChunksToBeRecycled() { return _chunksToBeRecycled; } 
        
        
    }

}