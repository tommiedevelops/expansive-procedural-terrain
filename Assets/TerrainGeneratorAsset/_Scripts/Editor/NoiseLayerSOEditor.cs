
using TerrainGeneratorAsset;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NoiseLayerSO))]
public class NoiseLayerSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI(); // Display usual serialized properties
    }
}