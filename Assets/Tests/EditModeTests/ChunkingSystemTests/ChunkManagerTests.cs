using System;
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

        [Test]
        public void Testing_Rendering_One_Chunk()
        {
            // initialise a chunkManager managing a single chunk
            var chunkManagerUnderTest = new ChunkManager();
            
            // mock a List<ChunkData> to recycle
            var chunksToRender = new List<ChunkManager.ChunkData>();
            
            const float sideLength = 1f;
            
            var chunkData = new ChunkManager.ChunkData
            {
                BotLeftPoint = Vector2.zero,
                SideLength = sideLength,
            };

            chunksToRender.Add(chunkData);
            
            // Request to render the chunk (aka add it to activeChunks)
            chunkManagerUnderTest.RequestChunksToBeRendered(chunksToRender);
            
            Assert.That(chunkManagerUnderTest.GetChunkPool(), Is.Not.Null); // there should be a chunkPool
            Assert.That(chunkManagerUnderTest.GetActiveChunks()[chunkData], Is.Not.Null);
        }

        [Test]
        public void Testing_Rendering_Two_Chunks()
        {
            var chunkManagerUnderTest = new ChunkManager();
            
            // mock a List<ChunkData> to recycle
            var chunksToRender = new List<ChunkManager.ChunkData>();
            if (chunksToRender == null) throw new ArgumentNullException(nameof(chunksToRender));

            var chunkData1 = new ChunkManager.ChunkData
            {
                BotLeftPoint = Vector2.zero,
                SideLength = 1f
            };
            
            var chunkData2 = new ChunkManager.ChunkData
            {
                BotLeftPoint = Vector2.zero,
                SideLength = 2f
            };

            chunksToRender.Add(chunkData1);
            chunksToRender.Add(chunkData2);
            
            chunkManagerUnderTest.RequestChunksToBeRendered(chunksToRender);
            
            Assert.That(chunkManagerUnderTest.GetChunkPool(), Is.Not.Null);
            Assert.That(chunkManagerUnderTest.GetActiveChunks()[chunkData1], Is.Not.Null);
            Assert.That(chunkManagerUnderTest.GetActiveChunks()[chunkData2], Is.Not.Null);
            
        }

        [Test]
        public void Can_Render_Chunk_Then_Recycle_It()
        {
            var chunkManagerUnderTest = new ChunkManager();
            var chunkData = new ChunkManager.ChunkData
            {
                BotLeftPoint = Vector2.zero,
                SideLength = 1f
            };
            
            // first render the chunk
            var chunksToRender = new List<ChunkManager.ChunkData> { chunkData };
            chunkManagerUnderTest.RequestChunksToBeRendered(chunksToRender);
            
            // Check that its been added to the active chunks list (sanity check here)
            Assert.That(chunkManagerUnderTest.GetActiveChunks()[chunkData], Is.Not.Null);
            
            // then recycle the chunk
            var chunksToRecycle = new List<ChunkManager.ChunkData> { chunkData };
            chunkManagerUnderTest.RecycleChunks(chunksToRecycle);
            chunkManagerUnderTest.GetActiveChunks().TryGetValue(chunkData, out var result);
            
            // Check that it's been taken out of the active chunks list
            Assert.That(result, Is.Null);
            
            // Check that it's been added to the chunksToBeRecycled list
            Assert.That(chunkManagerUnderTest.GetChunksToBeRecycled()[chunkData], Is.Not.Null);


        }
        
        // Can add chunks to be recycled to the chunk pool
            // maybe do some input validation here? That the mesh is of the correct size 
        
        
    }
}
