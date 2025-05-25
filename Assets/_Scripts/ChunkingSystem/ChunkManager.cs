using System;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;
using Random = System.Random;
using _Scripts.NoiseSystem;

namespace _Scripts.ChunkingSystem {
    public class ChunkManager {
        public struct ChunkData : IEquatable<ChunkData>
        {
            public float SideLength;
            public int NumVertices;
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

            public void Print()
            {
                Debug.Log($"SL:  {SideLength}, BL: {BotLeftPoint}");
            }
        }
        
        private readonly ChunkPool _chunkPool = new();
        private readonly Dictionary<ChunkData, GameObject> _activeChunks = new();
        private static NoiseGenerator _noiseGenerator;
        
        // ReSharper disable Unity.PerformanceAnalysis
        public static GameObject CreateChunk(ChunkData chunkData)
        {
            if (_noiseGenerator == null)
            {
                throw new ArgumentException("No Noise Generator has been created");
            }
            
            var gameObject = new GameObject($"BL{chunkData.BotLeftPoint}, SL: {chunkData.SideLength}, NV: {chunkData.NumVertices}")
            {
                transform =
                {
                    position = new Vector3(chunkData.BotLeftPoint.x, 0f, chunkData.BotLeftPoint.y),
                    rotation = Quaternion.identity,
                    localScale = new Vector3(1, 1, 1)
                }
            };
            
            // Generate Mesh
            var mesh = PlaneMeshGenerator.GenerateFlatPlaneMesh(
                new PlaneMeshGenerator.MeshData(chunkData.NumVertices, chunkData.NumVertices, chunkData.SideLength)
                );
            
            // Apply noise
            _noiseGenerator.SetGridDimensions(chunkData.NumVertices, chunkData.NumVertices);
            _noiseGenerator.ApplyNoise(mesh, chunkData.BotLeftPoint, 100);
            
            // Assign mesh to gameObject
            gameObject.AddComponent<MeshFilter>().sharedMesh = mesh;
            var material = gameObject.AddComponent<MeshRenderer>().material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            var color = (float)new Random((int)chunkData.SideLength).NextDouble();
            material.color = new Color(color, color, color);
            return gameObject;
        }
        public static void SetChunkPosition(GameObject chunk, Vector3 position) { chunk.transform.position = position; }
        public Dictionary<ChunkData, GameObject> GetActiveChunks() { return _activeChunks; }
        public void RequestNewChunksFromChunkData(List<ChunkData> chunkDataList)
        {
            
            foreach (var chunkData in chunkDataList)
            {
                _activeChunks[chunkData] = CreateChunk(chunkData);
            }
            
        }
        // ReSharper disable Unity.PerformanceAnalysis
        public void RecycleChunks(List<ChunkData> culledChunks)
        {
            foreach (var chunkData in culledChunks)
            {
                if (!_activeChunks.TryGetValue(chunkData, out GameObject chunkRemoved))
                {
                    // A chunk has not been generated for this node.
                    continue;
                };
                chunkRemoved.SetActive(false);
                _activeChunks.Remove(chunkData);
                _chunkPool.RecycleChunk(chunkRemoved, chunkData.SideLength);
            }
        }
        public ChunkPool GetChunkPool() { return _chunkPool; }

        public void RequestChunks(List<ChunkData> chunksToAdd)
        {
            foreach (var chunkData in chunksToAdd)
            {
                var chunk = _chunkPool.RequestChunk(chunkData.SideLength);

                if (chunk is null)
                {
                    chunk = CreateChunk(chunkData);
                    _activeChunks[chunkData] = chunk;
                } else {
                    chunk.transform.position = new Vector3(chunkData.BotLeftPoint.x, 0f, chunkData.BotLeftPoint.y);
                    chunk.SetActive(true);
                    _activeChunks[chunkData] = chunk;
                }

            }
        }

        public static void SetNoiseGenerator(NoiseGenerator noiseGenerator)
        {
            _noiseGenerator = noiseGenerator;
        }
    }

}