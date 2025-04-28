using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using ChunkingSystem;
using System.Collections.Generic;

namespace EditModeTests
{
    public class ChunkManagerTests {
        [Test]
        public void ChunkManagerCanCreateANewGameObject() {
            // Arrange
            var chunkManagerUnderTest = new ChunkManager();

            // Act
            GameObject gameObject = chunkManagerUnderTest.InstantiateNewGameObject();

            // Assert
            Assert.That(gameObject, Is.Not.Null);
        }

        [Test]
        public void ChunkManagerCanSetChunkPosition() {
            //Arrange
            var chunkManagerUnderTest = new ChunkManager();
            GameObject gameObjectUnderTest = new GameObject();
            Vector3 expectedPosition = Vector3.zero;

            //Act
            chunkManagerUnderTest.SetChunkPosition(gameObjectUnderTest, Vector3.zero);

            //Assert
            Assert.That(gameObjectUnderTest.transform.position, Is.EqualTo(expectedPosition));
            
        }

        [Test]
        public void ChunkManagerCanRequestChunkFromChunkPool() {
            // Arrange
            var chunkManagerUnderTest = new ChunkManager();
            var chunkPool = chunkManagerUnderTest.GetChunkPool();
            
            GameObject chunkToAddToPool = new GameObject();

            chunkPool.EnqueueChunk(chunkToAddToPool);

            // Act
            GameObject receivedGameObject = chunkManagerUnderTest.RequestChunkFromChunkPool();

            // Assert
            Assert.That(receivedGameObject, Is.Not.Null);


        }

        [Test]
        public void ChunkManagerCanRecycleChunksWithNoMesh() {
            // Arrange
            var chunkManagerUnderTest = new ChunkManager();
            var chunkPool = chunkManagerUnderTest.GetChunkPool();
            string chunkName = "ChunkToBeRecycled";

            var chunkToBeRecycled = new GameObject(chunkName);
            List<GameObject> chunksToBeRecycled = chunkManagerUnderTest.GetChunksToBeRecycled();
            chunksToBeRecycled.Add(chunkToBeRecycled);

            // Act
            chunkManagerUnderTest.RecycleChunks();
            var chunkRetrievedFromChunkPool = chunkPool.DequeueChunk();

            // Assert
            Assert.That(chunkRetrievedFromChunkPool, Is.Not.Null);
            Assert.That(chunkRetrievedFromChunkPool.name, Is.EqualTo(chunkName));
            Assert.That(!chunksToBeRecycled.Contains(chunkToBeRecycled));

        }
    }
}
