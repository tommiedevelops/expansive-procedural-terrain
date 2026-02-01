using UnityEngine;
using UnityEditor;
using TerrainGeneratorAsset;

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
