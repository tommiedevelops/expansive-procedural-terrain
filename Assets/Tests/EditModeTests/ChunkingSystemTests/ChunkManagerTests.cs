using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using ChunkingSystem;
using System.Collections.Generic;
using _Scripts.ChunkingSystem;

namespace EditModeTests
{
    public class ChunkManagerTests
    {
        [Test]
        public void Can_Instantiate_A_Chunk_Manager()
        {
            var chunkManagerUnderTest = new ChunkManager();
            Assert.That(chunkManagerUnderTest, Is.Not.Null);
        }
        
        [Test]
        public void Can_Set_Chunk_Position()
        {
            var chunkManagerUnderTest = new ChunkManager();
            var gameObjectUnderTest = new GameObject();
            var expectedPosition = Vector3.zero;

            ChunkManager.SetChunkPosition(gameObjectUnderTest, Vector3.zero);
            Assert.That(gameObjectUnderTest.transform.position, Is.EqualTo(expectedPosition));
        }

        [Test]
        public void Stores_Reference_To_All_Active_Chunks()
        {
            var chunkManagerUnderTest = new ChunkManager();
            var activeChunks = chunkManagerUnderTest.GetActiveChunks();
            Assert.That(activeChunks, Is.Not.Null);
        }

        [Test]
        public void Stores_Reference_To_All_Chunks_To_Be_Recycled()
        {
            var chunkManagerUnderTest = new ChunkManager();
            var chunksToBeRecycled = chunkManagerUnderTest.GetChunksToBeRecycled();
            Assert.That(chunksToBeRecycled, Is.Not.Null);
        }

        [Test]
        public void Can_Create_A_New_Chunk()
        {
            var mesh = PlaneMeshGenerator.GeneratePlaneMesh(new PlaneMeshGenerator.MeshData(2, 2, 1));
            var gameObject = ChunkManager.CreateChunk(mesh);
            Assert.That(gameObject, Is.Not.Null);
            Assert.That(gameObject.GetComponent(typeof(MeshFilter)), Is.Not.Null);
        }
        
    }
}
