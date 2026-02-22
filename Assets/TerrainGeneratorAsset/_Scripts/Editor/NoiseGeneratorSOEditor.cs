using UnityEngine;
using UnityEditor;
using TerrainGeneratorAsset;
using System.Collections.Generic;
using NUnit.Framework;
using System;
using TMPro;

[CustomEditor(typeof(NoiseGeneratorSO))]
public class NoiseGeneratorSOEditor : Editor
{
    private TerrainConfigSO m_terrainConfig;
    public void SetTerrainConfigSO(TerrainConfigSO terrainConfig)
    {
       m_terrainConfig = terrainConfig; 
    }
    public override void OnInspectorGUI()
    {
        var noiseGen = (NoiseGeneratorSO)target;
        serializedObject.Update();

        if (m_terrainConfig == null) return; // currently not handling this case

        // Experimental NoiseLayerSO Editor within array
        var noiseLayers = noiseGen.GetNoiseLayers();

        // Preview the final result
        EditorGUILayout.LabelField("Final result");
        EditorNoisePreviewer.DrawNoisePreview(m_terrainConfig, noiseGen.GetHeightFunc()); 

        // Preview the individual layers
        for (int i = 0; i < noiseLayers.Count; i++)
        {
            if(noiseLayers[i] == null)
            {
                // Create field for user to drag noise layer in
                noiseLayers[i] = (NoiseLayerSO)EditorGUILayout.ObjectField (
                        label: "Noise Layer",
                        obj: noiseLayers[i],
                        objType: typeof(NoiseLayerSO),
                        allowSceneObjects: false
                );
            }
            else
            {
                // if not already copied, make a copy of noiseLayers[i] and store it inside NoiseGeneratorSO
                var editor = (NoiseLayerSOEditor)Editor.CreateEditor(noiseLayers[i], typeof(NoiseLayerSOEditor));
                editor.SetTerrainConfigSO(m_terrainConfig);
                editor.OnInspectorGUI(); // Display usual Serialized Fields
            }

            if (GUILayout.Button(new GUIContent("Remove Layer")))
                    noiseLayers.Remove(noiseLayers[i]);

        }

        // Add New Layer Button
        if (GUILayout.Button(new GUIContent("Add New Layer"))) noiseLayers.Add(null);

        serializedObject.ApplyModifiedProperties();
    }
}
