
using TerrainGeneratorAsset;
using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;

[CustomEditor(typeof(NoiseLayerSO))]
public class NoiseLayerSOEditor : Editor
{
    private TerrainConfigSO m_terrainConfig;
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        NoiseLayerSO noiseLayer = target as NoiseLayerSO;

        GUIStyle centredBold = new GUIStyle(EditorStyles.label);
        centredBold.alignment = TextAnchor.MiddleCenter;
        centredBold.fontStyle = FontStyle.Bold;

        EditorGUILayout.LabelField(noiseLayer.name, centredBold);

        // List Serialized Fields
        var it = serializedObject.GetIterator();
        bool enterChildren = true;
        while (it.NextVisible(enterChildren))
        {
            if (it.propertyPath == "m_Script") continue;
            EditorGUILayout.PropertyField(it, includeChildren: true);
            enterChildren = false;
        }

        EditorGUILayout.Space();

        EditorTexturePreviewer.DrawNoiseLayerPreviewTexture(m_terrainConfig, noiseLayer);

        EditorGUILayout.Space(5f);

        serializedObject.ApplyModifiedProperties();
    }
  
    public void SetTerrainConfigSO(TerrainConfigSO terrainConfig)
    {
       m_terrainConfig = terrainConfig; 
    }

}