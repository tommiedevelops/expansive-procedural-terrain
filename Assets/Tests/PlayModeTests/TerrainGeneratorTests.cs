using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Core;

public class TerrainGeneratorTests {

    [UnityTest]
    public IEnumerator CanInitialiseTerrainGenerator() {

        // Arrange
        var go = new GameObject();
        var terrainGeneratorUnderTest = go.AddComponent<TerrainGenerator>();

        // Act


        // Assert
        yield return null;
    }
}
