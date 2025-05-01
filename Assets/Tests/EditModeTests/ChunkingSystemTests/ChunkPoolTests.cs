using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using ChunkingSystem;
using UnityEditor.Animations;
using Unity.Hierarchy;
using System.Net.Mail;

namespace EditModeTests {
    public class ChunkPoolTests {

        #region simple tests
        [Test]
        public void Can_Instantiate_A_Chunk_Pool() {
            var chunkPool = new ChunkPool();
            Assert.That(chunkPool, Is.Not.Null);
        }

        [Test]
        public void Chunk_Pool_Has_A_Queue() {
            var chunkPool = new ChunkPool();
            Queue<GameObject> queue = chunkPool.GetChunkQueue();
            Assert.That(queue, Is.Not.Null);
        }

        [Test]
        public void Can_Add_Chunk_To_ChunkPool() {
            var chunkPool = new ChunkPool();
            var chunk = new GameObject();

            chunkPool.AddChunkToPool(chunk);

            var queue = chunkPool.GetChunkQueue();

            Assert.That(queue.Dequeue(), Is.EqualTo(chunk));
            
        }

        [Test] 
        public void Can_Add_Multiple_Chunks_To_ChunkPool() {
            var chunkPool = new ChunkPool();
            int numChunksToAdd = 5;

            for(int i = 0; i < numChunksToAdd; i++) {
                chunkPool.AddChunkToPool(new GameObject());
            }

            Assert.That(chunkPool.GetChunkQueue().Count, Is.EqualTo(5));

        }

        [Test]
        public void Can_Retrieve_Chunk_From_Chunk_Pool() {
            var chunkPool = new ChunkPool();
            chunkPool.AddChunkToPool(new GameObject());

            var chunk = chunkPool.RequestChunk();

            Assert.That(chunk, Is.Not.Null);
        }

        [Test]
        public void Can_Recycle_A_Chunk_With_A_Mesh() {
            var chunkPool = new ChunkPool();
            var chunkAdded = new GameObject();
            Mesh mesh = PlaneMeshGenerator.GeneratePlaneMesh(new PlaneMeshGenerator.MeshData(2, 2, 1));
            var meshFilter = chunkAdded.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;

            chunkPool.AddChunkToPool(chunkAdded);
            var chunkReceived = chunkPool.RequestChunk();

            Assert.That(chunkReceived, Is.EqualTo(chunkAdded));
        }
        #endregion

        [Test]
        public void Can_Request_Chunk_With_Mesh_Of_Given_Size_And_LOD_NULL() {
            var chunkPool = new ChunkPool();

            float meshSideLength = 1;
            int lod = 0; // highest resolution

            var receivedChunk = chunkPool.RequestChunk(meshSideLength, lod);

            Assert.That(receivedChunk, Is.Null);
        }

        [Test]
        public void Can_Request_Chunk_With_Mesh_Of_Given_Size_And_LOD_POSITIVE() {
            var chunkPool = new ChunkPool();

            float size = 2f; // 2 world units by 2 world units
            int lod = 0; // highest resolution


            Mesh mesh = PlaneMeshGenerator.GeneratePlaneMesh(new PlaneMeshGenerator.MeshData(2, 2, size));
            var chunkAdded = new GameObject();
            var mf = chunkAdded.AddComponent<MeshFilter>();
            mf.mesh = mesh;

            chunkPool.AddChunkToPool(chunkAdded);

            GameObject receivedChunk = chunkPool.RequestChunk(size, lod);
            var receivedMesh = receivedChunk.GetComponent<MeshFilter>().mesh;
            var vertices = receivedMesh.vertices;

            // check that there are the correct number of vertices
            Assert.That(vertices.Length, Is.EqualTo(4));

            // check that the side length is correct
            Assert.That((vertices[1] - vertices[0]).magnitude, Is.EqualTo(size));
            Assert.That((vertices[3] - vertices[2]).magnitude, Is.EqualTo(size));

        }

    }
}
