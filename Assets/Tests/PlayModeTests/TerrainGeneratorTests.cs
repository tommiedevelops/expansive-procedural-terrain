using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Core;

public class TerrainGeneratorTests {

    [UnityTest]
    public IEnumerator CanInitialiseTerrainGenerator() {
        var go = new GameObject();
        var cameraGO = new GameObject();
        cameraGO.transform.position = Vector3.zero;
        cameraGO.transform.LookAt(Vector3.one);

        var viewerGO = new GameObject();

        var viewer = viewerGO.AddComponent<QTViewer>();

        var terrainGeneratorUnderTest = go.AddComponent<TerrainGenerator>();

        viewer.SetCameraTransform(cameraGO.transform);
        
        Assert.That(terrainGeneratorUnderTest, Is.Not.Null);

        yield return null;
    }
}
