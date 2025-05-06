using System.Collections;
using _Scripts.Core;
using _Scripts.QuadTreeSystem;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.PlayModeTests
{
    public class TerrainGeneratorTests {
        
        [UnityTest]
        public IEnumerator Can_Initialise_A_TerrainGenerator()
        {
            // Mock camera
            var cameraGO = new GameObject();
            var camera = cameraGO.AddComponent<Camera>();
            
            // Prepare terrainGenerator
            var gameObject = new GameObject();
            var terrainGeneratorUnderTest = gameObject.AddComponent<TerrainGenerator>();
            terrainGeneratorUnderTest.SetCamera(camera);
            
            yield return null; // Call the start method
            
            Assert.That(terrainGeneratorUnderTest, Is.Not.Null);
            Assert.That(terrainGeneratorUnderTest.GetQuadTree(), Is.Not.Null);
            Assert.That(terrainGeneratorUnderTest.GetViewer(), Is.Not.Null);
            Assert.That(terrainGeneratorUnderTest.GetChunkManager(), Is.Not.Null);
            Assert.That(terrainGeneratorUnderTest.GetLODManager(), Is.Not.Null);
        }
        
    }
}
