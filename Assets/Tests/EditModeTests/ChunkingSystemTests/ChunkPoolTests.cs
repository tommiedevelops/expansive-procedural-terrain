using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using ChunkingSystem;
using UnityEditor.Animations;
using Unity.Hierarchy;
using System.Net.Mail;
using _Scripts.ChunkingSystem;
using _Scripts.NoiseSystem;

namespace EditModeTests {
    public class ChunkPoolTests {
        
        [Test]
        public void Can_Instantiate_A_Chunk_Pool() {
            var chunkPool = new ChunkPool();
            Assert.That(chunkPool, Is.Not.Null);
        }

        [Test]
        public void Can_Add_Chunk_To_ChunkPool() {
            var chunkPool = new ChunkPool();
            var chunk = new GameObject();
            const float sideLength = 1f;
            var botLeftPoint = Vector2.zero;

            chunkPool.RecycleChunk(chunk, sideLength, botLeftPoint);

            var queue = chunkPool.GetChunkQueue(sideLength, botLeftPoint);

            Assert.That(queue.Dequeue(), Is.EqualTo(chunk));
            
        }

        [Test] 
        public void Can_Add_Multiple_Chunks_To_ChunkPool() {
            var chunkPool = new ChunkPool();
            const int numChunksToAdd = 5;
            const float sideLength = 1f;
            var botLeftPoint = Vector2.zero;
            
            for(int i = 0; i < numChunksToAdd; i++) {
                chunkPool.RecycleChunk(new GameObject(), sideLength, botLeftPoint);
            }

            Assert.That(chunkPool.GetChunkQueue(sideLength, botLeftPoint).Count, Is.EqualTo(numChunksToAdd));

        }
        
        [Test]
        public void Can_Recycle_A_Chunk_With_A_Mesh() {
            var chunkPool = new ChunkPool();
            var chunkAdded = new GameObject();
            const float sideLength = 1f;
            var botLeftPoint = Vector2.zero;
            const int numVertices = 2;
            var mesh = PlaneMeshGenerator.GeneratePlaneMeshFromHeightMap(
                new HeightMap(0,0),
                new PlaneMeshGenerator.SquareMeshData(numVertices, numVertices)
                );
            var meshFilter = chunkAdded.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;

            chunkPool.RecycleChunk(chunkAdded, sideLength, botLeftPoint);
            var chunkReceived = chunkPool.RequestChunk(sideLength, botLeftPoint);

            Assert.That(chunkReceived, Is.EqualTo(chunkAdded));
        }
        
        [Test]
        public void Can_Request_Chunk_With_Mesh_Of_Given_Size_NULL() {
            var chunkPool = new ChunkPool();

            const float sideLength = 1;
            var botLeftPoint  = Vector2.zero;
            var receivedChunk = chunkPool.RequestChunk(sideLength, botLeftPoint);

            Assert.That(receivedChunk, Is.Null);
        }

        [Test]
        public void Can_Recycle_Then_Request_Chunk_Of_Given_Size_No_Mesh()
        {
            var chunkPoolUnderTest = new ChunkPool();
            const float sideLength = 1f;
            var botLeftPoint = Vector2.zero;
            var chunk = new GameObject();
            
            chunkPoolUnderTest.RecycleChunk(chunk, sideLength, botLeftPoint);
            var receivedChunk = chunkPoolUnderTest.RequestChunk(sideLength, botLeftPoint);
            
            Assert.That(receivedChunk, Is.EqualTo(chunk));

        }

        [Test]
        public void Can_Recycle_Then_Request_Chunk_Of_Given_Size_With_Mesh()
        {
            var chunkPoolUnderTest = new ChunkPool();
            const float sideLength = 1f;
            var botLeftPoint = Vector2.zero;
            const int numVertices = 2;
            var chunk = new GameObject();
            var meshFilter = chunk.AddComponent<MeshFilter>();
            var mesh = PlaneMeshGenerator.GeneratePlaneMeshFromHeightMap(
                new HeightMap(0,0),
                new PlaneMeshGenerator.SquareMeshData(numVertices, numVertices)
            );
            meshFilter.sharedMesh = mesh;
            
            chunkPoolUnderTest.RecycleChunk(chunk, sideLength, botLeftPoint);
            var receivedChunk = chunkPoolUnderTest.RequestChunk(sideLength, botLeftPoint);
            
            Assert.That(receivedChunk, Is.EqualTo(chunk));


        }

        [Test]
        public void Can_Recycle_And_Request_Chunks_Of_Various_Sizes()
        {
            var chunkPoolUnderTest = new ChunkPool();
            
            var botLeftPointForAll = Vector2.zero;
            
            var chunk1 = new GameObject();
            var chunk2 = new GameObject();
            var chunk3 = new GameObject();

            chunk1.AddComponent<MeshFilter>().sharedMesh =
                PlaneMeshGenerator.GeneratePlaneMeshFromHeightMap(
                    new HeightMap(0, 0),
                    new PlaneMeshGenerator.SquareMeshData(2, 1)
                    );

            chunk2.AddComponent<MeshFilter>().sharedMesh =
                PlaneMeshGenerator.GeneratePlaneMeshFromHeightMap(
                    new HeightMap(0, 0),
                    new PlaneMeshGenerator.SquareMeshData(2, 1)
                );

            chunk3.AddComponent<MeshFilter>().sharedMesh =
                PlaneMeshGenerator.GeneratePlaneMeshFromHeightMap(
                    new HeightMap(0, 0),
                    new PlaneMeshGenerator.SquareMeshData(2, 1)
                );
            
            chunkPoolUnderTest.RecycleChunk(chunk1, 1, botLeftPointForAll);
            chunkPoolUnderTest.RecycleChunk(chunk2, 2, botLeftPointForAll);
            chunkPoolUnderTest.RecycleChunk(chunk3, 3, botLeftPointForAll);
            
            var receivedChunk1 = chunkPoolUnderTest.RequestChunk(1, botLeftPointForAll);
            var receivedChunk2 = chunkPoolUnderTest.RequestChunk(2, botLeftPointForAll);
            var receivedChunk3 = chunkPoolUnderTest.RequestChunk(3, botLeftPointForAll);
            
            Assert.That(receivedChunk1, Is.EqualTo(chunk1));
            Assert.That(receivedChunk2, Is.EqualTo(chunk2));
            Assert.That(receivedChunk3, Is.EqualTo(chunk3));
        }
    }
}
