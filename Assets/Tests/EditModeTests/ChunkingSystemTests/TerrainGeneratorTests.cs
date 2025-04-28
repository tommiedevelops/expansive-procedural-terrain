using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Core;

namespace EditModeTests
{
    public class TerrainGeneratorTests
    {
        // A Test behaves as an ordinary method
        [Test]
        public void CanGenerateAQuadTree() {
            // Arrangle
            var go = new GameObject();
            var terrainGeneratorUnderTest = go.AddComponent<TerrainGenerator>();

            var go2 = new GameObject();
            var viewer = go2.AddComponent<QTViewer>();

            // Act
            QuadTree tree = terrainGeneratorUnderTest.GenerateQuadTree(viewer);

            // Assert
            Assert.That(tree, Is.Not.Null);

            // CleanUp
            Object.DestroyImmediate(go);
            Object.DestroyImmediate(go2);
        }

    }
}
