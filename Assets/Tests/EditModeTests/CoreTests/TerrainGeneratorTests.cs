using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.ChunkingSystem;
using _Scripts.Core;
using NUnit.Framework;
using UnityEngine;
using ChunkingSystem;

namespace EditModeTests
{
    public class TerrainGeneratorTests
    {
        [Test]
        public void Testing_IdentifyLeafNodesNotActive()
        {
            var go = new GameObject();
            var terrainGeneratorUnderTest = go.AddComponent<TerrainGenerator>();
            
            var chunkData1 = new ChunkManager.ChunkData()
            {
                SideLength = 1f
            };
            
            var chunkData2 = new ChunkManager.ChunkData()
            {
                SideLength = 2f
            };

            var chunkData3 = new ChunkManager.ChunkData()
            {
                SideLength = 3f
            };

            var activeChunks = new Dictionary<ChunkManager.ChunkData, GameObject>();
            var chunksFromLeafNodes = new List<ChunkManager.ChunkData>();

            activeChunks[chunkData1] = new GameObject();
            activeChunks[chunkData2] = new GameObject();
            
            chunksFromLeafNodes.Add(chunkData1);
            chunksFromLeafNodes.Add(chunkData2);
            chunksFromLeafNodes.Add(chunkData3);
            
            var chunksToAdd = TerrainGenerator.IdentifyLeafNodesNotActive(chunksFromLeafNodes, activeChunks.Keys);
            
            Assert.That(chunksToAdd.Count, Is.EqualTo(1));
            Assert.That(chunksToAdd[0], Is.EqualTo(chunkData3));
        }
        
        
    }
}
