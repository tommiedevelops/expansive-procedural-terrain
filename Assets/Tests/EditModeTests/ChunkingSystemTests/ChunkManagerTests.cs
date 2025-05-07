using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using ChunkingSystem;
using System.Collections.Generic;
using _Scripts.ChunkingSystem;
using _Scripts.QuadTreeSystem;

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
                NumVertices = 2
            };

            chunksToRender.Add(chunkData);
            
            // Request to render the chunk (aka add it to activeChunks)
            chunkManagerUnderTest.RequestNewChunksFromChunkData(chunksToRender);
            
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
                SideLength = 1f,
                NumVertices = 2
            };
            
            var chunkData2 = new ChunkManager.ChunkData
            {
                BotLeftPoint = Vector2.zero,
                SideLength = 2f,
                NumVertices = 2
            };

            chunksToRender.Add(chunkData1);
            chunksToRender.Add(chunkData2);
            
            chunkManagerUnderTest.RequestNewChunksFromChunkData(chunksToRender);
            
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
                SideLength = 1f,
                NumVertices = 2
                
            };
            
            // first render the chunk
            var chunksToRender = new List<ChunkManager.ChunkData> { chunkData };
            chunkManagerUnderTest.RequestNewChunksFromChunkData(chunksToRender);
            
            // Check that it's been added to the active chunks list (sanity check here)
            Assert.That(chunkManagerUnderTest.GetActiveChunks()[chunkData], Is.Not.Null);
            
            // then recycle the chunk
            var chunksToRecycle = new List<ChunkManager.ChunkData> { chunkData };
            chunkManagerUnderTest.RecycleChunks(chunksToRecycle);
            chunkManagerUnderTest.GetActiveChunks().TryGetValue(chunkData, out var result);
            
            // Check that it's been taken out of the active chunks list
            Assert.That(result, Is.Null);

        }

        [Test]
        public void Can_Create_Chunk_Correctly_From_ChunkData()
        {
            // Prepare test chunkData
            var chunkData = new ChunkManager.ChunkData()
            {
                BotLeftPoint = Vector2.zero,
                SideLength = 1f,
                NumVertices = 2
            };
            
            // Create chunk from chunkData
            var chunk = ChunkManager.CreateChunk(chunkData);
            
            // Check that it matches expectations
            Assert.That(chunk, Is.Not.Null);
            var meshFilter = (MeshFilter)chunk.GetComponent(typeof(MeshFilter));
            Assert.That(meshFilter, Is.Not.Null);
            Assert.That(meshFilter.sharedMesh, Is.Not.Null);
            Assert.That(meshFilter.sharedMesh.vertices, Has.Length.EqualTo(4));
            Assert.That(meshFilter.sharedMesh.triangles, Has.Length.EqualTo(6));
            Assert.That((meshFilter.sharedMesh.vertices[1] - meshFilter.sharedMesh.vertices[0]).magnitude,  Is.EqualTo(1f));
        }

        [Test]
        public void Can_Create_Chunk_From_ChunkData()
        {
            var chunkData = new ChunkManager.ChunkData()
            {
                BotLeftPoint = Vector2.zero,
                SideLength = 187.5f,
                NumVertices = 240
            };
            
            var chunk = ChunkManager.CreateChunk(chunkData);
            var meshFilter = chunk.GetComponent(typeof(MeshFilter)) as MeshFilter;
            var mesh = meshFilter?.sharedMesh;
            
            
            Assert.That(chunk, Is.Not.Null);
            Assert.That(meshFilter, Is.Not.Null);
            Assert.That(mesh, Is.Not.Null);
            Assert.That(mesh.vertices, Has.Length.EqualTo(240*240));
            
        }
    }
}
