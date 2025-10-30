using UnityEngine;
using UnityEditor;
using _Scripts.NoiseSystem;
using System.Collections.Generic;
using _Scripts.NoiseSystem.ScriptableObjects;
using _Scripts.ChunkingSystem;

[CustomEditor(typeof(NoiseGenerator))]
public class NoiseGeneratorEditor : Editor
{
    
    bool previewEnabled = false;
    private int gridResolution = 10;
    private float sideLength = 10f;
    private float heightMultiplier = 1f;
    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("3D Preview Settings");
        base.OnInspectorGUI();
        gridResolution = EditorGUILayout.IntField("Grid Side Length", gridResolution);
        sideLength = EditorGUILayout.FloatField("Side Length", sideLength);
        heightMultiplier = EditorGUILayout.FloatField("Height Multiplier", heightMultiplier);
    }  
    
    void OnSceneGUI()
    {
        var noiseGenerator = target as NoiseGenerator;
        if (!noiseGenerator) return;
        
        var heightMap = noiseGenerator.GenerateNoiseMap(Vector2.zero, 1f, heightMultiplier, gridResolution, gridResolution);
        var meshData = new PlaneMeshGenerator.SquareMeshData(gridResolution, sideLength);
        var mesh = PlaneMeshGenerator.GeneratePlaneMeshFromHeightMap(heightMap, meshData);
        Material mat = new  Material(Shader.Find("Standard"));
        Matrix4x4 matrix = Matrix4x4.TRS(
            noiseGenerator.transform.position,
            noiseGenerator.transform.rotation,
            noiseGenerator.transform.localScale
        );
        
        mat.SetPass(0);
        
        Graphics.DrawMeshNow(mesh, Vector3.zero, Quaternion.identity);
    }
    
    
    
}
