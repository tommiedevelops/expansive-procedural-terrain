using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Core;

public class TerrainGeneratorTests
{
    GameObject go;
    QTViewer viewer;

    [SetUp]
    public void SetUp() {
        go = new GameObject();

        var go2 = new GameObject();

    }

    [UnityTest]
    public IEnumerator CanInitialiseTerrainGenerator()
    {
        // Arrange
        var go = new GameObject();
        var terrainGeneratorUnderTest = go.AddComponent<TerrainGenerator>();

        // Act

        // Assert
        yield return null;
    }
}
