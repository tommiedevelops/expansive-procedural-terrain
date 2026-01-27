using UnityEngine;
using UnityEditor;
using TerrainGenerator.NoiseSystem;
using TerrainGenerator.MeshGenerators;

[CustomEditor(typeof(NoiseGeneratorSO))]
public class NoiseGeneratorSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawDefaultInspector();
        serializedObject.ApplyModifiedProperties();
    }

}
