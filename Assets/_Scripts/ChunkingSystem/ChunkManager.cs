using System;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;

namespace _Scripts.ChunkingSystem {
    public class ChunkManager {
        public struct ChunkData : IEquatable<ChunkData>
        {
            public float SideLength;
            public Vector2 BotLeftPoint;

            public bool Equals(ChunkData other)
            {
                return SideLength.Equals(other.SideLength) && BotLeftPoint.Equals(other.BotLeftPoint);
            }

            public override bool Equals(object obj)
            {
                return obj is ChunkData other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(SideLength, BotLeftPoint);
            }
        }
        
        private readonly ChunkPool _chunkPool = new();
        private readonly Dictionary<ChunkData, GameObject> _chunksToBeRecycled = new();
        private readonly Dictionary<ChunkData, GameObject> _activeChunks = new();

        public static GameObject CreateChunk(ChunkData chunkData)
        {
            var gameObject = new GameObject
            {
                transform =
                {
                    position = chunkData.BotLeftPoint,
                    rotation = Quaternion.identity,
                    localScale = new Vector3(1, 1, 1)
                }
            };

            var mesh = PlaneMeshGenerator.GeneratePlaneMesh(new PlaneMeshGenerator
                .MeshData(2, 2, chunkData.SideLength));
            
            gameObject.AddComponent<MeshFilter>().sharedMesh = mesh;
            gameObject.AddComponent<MeshRenderer>().material = new Material(Shader.Find($"Diffuse"));
            return gameObject;
        }
        public static void SetChunkPosition(GameObject chunk, Vector3 position) { chunk.transform.position = position; }
        public Dictionary<ChunkData, GameObject> GetActiveChunks() { return _activeChunks; }
        public Dictionary<ChunkData, GameObject> GetChunksToBeRecycled() { return _chunksToBeRecycled; }
        public void RequestChunksToBeRendered(List<ChunkData> chunkDataList)
        {

            foreach (ChunkData chunkData in chunkDataList)
            {
                _activeChunks[chunkData] = new GameObject();
            }
            
        }

        public void RecycleChunks(List<ChunkData> culledChunks)
        {
            foreach (var chunkData in culledChunks)
            {
                var chunkRemoved = _activeChunks[chunkData];
                _activeChunks.Remove(chunkData);
                _chunksToBeRecycled[chunkData] = chunkRemoved;
            }
        }

        public ChunkPool GetChunkPool()
        {
            return _chunkPool;
        }

        public void GiveRecycledChunksToChunkPool()
        {
            foreach (var chunkData in _chunksToBeRecycled.Keys)
            {
                var chunk = _chunksToBeRecycled[chunkData];
                _chunkPool.RecycleChunk(chunk, chunkData.SideLength);
            }
            _chunksToBeRecycled.Clear();
        }
    }

}