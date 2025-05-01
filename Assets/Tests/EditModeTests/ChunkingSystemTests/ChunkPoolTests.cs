using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using ChunkingSystem;
using UnityEditor.Animations;
using Unity.Hierarchy;
using System.Net.Mail;
using _Scripts.ChunkingSystem;

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

            chunkPool.RecycleChunk(chunk, sideLength);

            var queue = chunkPool.GetChunkQueue(sideLength);

            Assert.That(queue.Dequeue(), Is.EqualTo(chunk));
            
        }

        [Test] 
        public void Can_Add_Multiple_Chunks_To_ChunkPool() {
            var chunkPool = new ChunkPool();
            const int numChunksToAdd = 5;
            const float sideLength = 1f;
            
            for(int i = 0; i < numChunksToAdd; i++) {
                chunkPool.RecycleChunk(new GameObject(), sideLength);
            }

            Assert.That(chunkPool.GetChunkQueue(sideLength).Count, Is.EqualTo(numChunksToAdd));

        }
        
        [Test]
        public void Can_Recycle_A_Chunk_With_A_Mesh() {
            var chunkPool = new ChunkPool();
            var chunkAdded = new GameObject();
            const float sideLength = 1f;
            const int numVertices = 2;
            var mesh = PlaneMeshGenerator.GeneratePlaneMesh(new PlaneMeshGenerator.MeshData(numVertices, numVertices, sideLength));
            var meshFilter = chunkAdded.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;

            chunkPool.RecycleChunk(chunkAdded, sideLength);
            var chunkReceived = chunkPool.RequestChunk(sideLength);

            Assert.That(chunkReceived, Is.EqualTo(chunkAdded));
        }
        
        [Test]
        public void Can_Request_Chunk_With_Mesh_Of_Given_Size_NULL() {
            var chunkPool = new ChunkPool();

            const float sideLength = 1;
            var receivedChunk = chunkPool.RequestChunk(sideLength);

            Assert.That(receivedChunk, Is.Null);
        }

        [Test]
        public void Can_Recycle_Then_Request_Chunk_Of_Given_Size_No_Mesh()
        {
            var chunkPoolUnderTest = new ChunkPool();
            const float sideLength = 1f;
            var chunk = new GameObject();
            
            chunkPoolUnderTest.RecycleChunk(chunk, sideLength);
            var receivedChunk = chunkPoolUnderTest.RequestChunk(sideLength);
            
            Assert.That(receivedChunk, Is.EqualTo(chunk));

        }

        [Test]
        public void Can_Recycle_Then_Request_Chunk_Of_Given_Size_With_Mesh()
        {
            var chunkPoolUnderTest = new ChunkPool();
            const float sideLength = 1f;
            const int numVertices = 2;
            var chunk = new GameObject();
            var meshFilter = chunk.AddComponent<MeshFilter>();
            var mesh = PlaneMeshGenerator.GeneratePlaneMesh(new PlaneMeshGenerator.MeshData(numVertices, numVertices, sideLength));
            meshFilter.sharedMesh = mesh;
            
            chunkPoolUnderTest.RecycleChunk(chunk, sideLength);
            var receivedChunk = chunkPoolUnderTest.RequestChunk(sideLength);
            
            Assert.That(receivedChunk, Is.EqualTo(chunk));


        }

    }
}
