using UnityEngine;
using UnityEditor;
using TerrainGeneratorAsset;

[CustomEditor(typeof(NoiseGeneratorSO))]
public class NoiseGeneratorSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);

        // Experimental NoiseLayerSO Editor within array
        var noiseGen = (NoiseGeneratorSO)target;

        var noiseLayers = noiseGen.GetNoiseLayers();
        if (noiseLayers == null) return;

        foreach(var layer in noiseLayers)
        {
            Editor editor = CreateEditor(layer);
            editor.OnInspectorGUI();
        }

    }
}
