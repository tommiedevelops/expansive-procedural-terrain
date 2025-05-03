using System;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;

namespace _Scripts.ChunkingSystem {
    public class ChunkManager {
        
        private ChunkPool _chunkPool = new();
        private readonly List<GameObject> _chunksToBeRecycled = new();
        private readonly List<GameObject> _activeChunks = new();

        public static GameObject CreateChunk(Mesh mesh)
        {
            var gameObject = new GameObject
            {
                transform =
                {
                    position = Vector3.zero,
                    rotation = Quaternion.identity,
                    localScale = new Vector3(1, 1, 1)
                }
            };

            gameObject.AddComponent<MeshFilter>().mesh = mesh;
            gameObject.AddComponent<MeshRenderer>().material = new Material(Shader.Find($"Diffuse"));
            return gameObject;
        }
        public static void SetChunkPosition(GameObject chunk, Vector3 position) { chunk.transform.position = position; }
        public List<GameObject> GetActiveChunks() { return _activeChunks; }
        public List<GameObject> GetChunksToBeRecycled() { return _chunksToBeRecycled; } 
        
    }

}