
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

        noiseLayer.isExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(noiseLayer.isExpanded, noiseLayer.name);

        if (noiseLayer.isExpanded)
        {
            // List Serialized Fields
            var it = serializedObject.GetIterator();
            bool enterChildren = true;
            while (it.NextVisible(enterChildren))
            {
                if (it.propertyPath == "m_Script" || it.propertyPath == "isExpanded") continue;
                EditorGUILayout.PropertyField(it, includeChildren: true);
                enterChildren = false;
            }

            EditorGUILayout.Space();

            EditorNoisePreviewer.DrawNoisePreview(m_terrainConfig, noiseLayer.Evaluate);

            EditorGUILayout.Space(5f);

        }

        EditorGUILayout.EndFoldoutHeaderGroup();
        serializedObject.ApplyModifiedProperties();
    }
  
    public void SetTerrainConfigSO(TerrainConfigSO terrainConfig)
    {
       m_terrainConfig = terrainConfig; 
    }

}