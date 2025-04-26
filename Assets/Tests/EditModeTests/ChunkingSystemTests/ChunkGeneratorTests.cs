using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using ChunkingSystem;

namespace EditModeTests
{
    public class ChunkGeneratorTests
    {
        [Test]
        public void InstantiatesGameObject()
        {
            GameObject actual = ChunkGenerator.RequestNewChunk();
            Assert.That(actual, Is.Not.Null);         
        }
    }
}
