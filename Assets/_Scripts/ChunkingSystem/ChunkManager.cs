using System;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;
using Random = System.Random;
using _Scripts.NoiseSystem;
using static _Scripts.ChunkingSystem.PlaneMeshGenerator;
// ReSharper disable All

namespace _Scripts.ChunkingSystem {
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
    public class ChunkManager {

        private NoiseGenerator _noiseGen;
        
        private readonly ChunkPool _chunkPool = new();
        private readonly Dictionary<ChunkData, GameObject> _activeChunks = new();

        private float _heightRange;

        public ChunkManager(NoiseGenerator noiseGen, float heightRange) {
            _noiseGen = noiseGen;
            _heightRange = heightRange;
        }
        private Mesh PrepareMesh(NoiseGenerator noiseGen, ChunkData cd) {
            var meshData = new SquareMeshData(cd.NumVertices, cd.SideLength);
            HeightMap heightMap = _noiseGen.GenerateNoiseMap(cd.BotLeftPoint, meshData.DistanceBetweenPoints, _heightRange, cd.NumVertices, cd.NumVertices);
            Mesh mesh = GeneratePlaneMeshFromHeightMap(heightMap,meshData);
            return mesh;
        }

        private GameObject CreateGameObject(Mesh mesh, ChunkData cd) {
                
            var go = new GameObject($"BL{cd.BotLeftPoint}, SL: {cd.SideLength}, NV: {cd.NumVertices}")
            {
                transform =
                {
                    position = new Vector3(cd.BotLeftPoint.x, 0f, cd.BotLeftPoint.y),
                    rotation = Quaternion.identity,
                    localScale = new Vector3(1, 1, 1)
                }
            };

            go.AddComponent<MeshFilter>().sharedMesh = mesh;
            var material = go.AddComponent<MeshRenderer>().material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            
            var color = (float)new Random((int)cd.SideLength).NextDouble();
            material.color = new Color(color, color, color);

            return go;
        }

        public GameObject CreateGameObjectFromChunkData(ChunkData chunkData)
        {
            Mesh mesh = PrepareMesh(_noiseGen, chunkData);
            return CreateGameObject(mesh, chunkData);
        }
        public static void SetChunkPosition(GameObject chunk, Vector3 position) { chunk.transform.position = position; }
        public Dictionary<ChunkData, GameObject> GetActiveChunks() { return _activeChunks; }
        public void CreateNewChunksFromChunkData(List<ChunkData> chunkDataList)
        {
            foreach (var chunkData in chunkDataList)
                _activeChunks[chunkData] = CreateGameObjectFromChunkData(chunkData);
        }
        public void RecycleChunks(List<ChunkData> culledChunks)
        {
            foreach (var chunkData in culledChunks)
            {
                if (!_activeChunks.TryGetValue(chunkData, out GameObject chunkRemoved))
                {
                    // A chunk has not been generated for this node.
                    continue;
                }
                chunkRemoved.SetActive(false);
                _activeChunks.Remove(chunkData);
                _chunkPool.RecycleChunk(chunkRemoved, chunkData.SideLength, chunkData.BotLeftPoint);
            }
        }
        public ChunkPool GetChunkPool() { return _chunkPool; }
        public void RequestChunks(List<ChunkData> chunksToAdd)
        {
            foreach (var chunkData in chunksToAdd)
            {
                var chunk = _chunkPool.RequestChunk(chunkData.SideLength, chunkData.BotLeftPoint);

                if (chunk is null)
                {
                    chunk = CreateGameObjectFromChunkData(chunkData);
                    _activeChunks[chunkData] = chunk;
                } else {
                    chunk.transform.position = new Vector3(chunkData.BotLeftPoint.x, 0f, chunkData.BotLeftPoint.y);
                    chunk.SetActive(true);
                    _activeChunks[chunkData] = chunk;
                }

            }
        }
        public void SetNoiseGenerator(NoiseGenerator noiseGenerator)
        {
            _noiseGen = noiseGenerator;
        }
    }

}