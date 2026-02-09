using UnityEngine;
using UnityEditor;
using TerrainGeneratorAsset;
using System.Collections.Generic;
using NUnit.Framework;

[CustomEditor(typeof(NoiseGeneratorSO))]
public class NoiseGeneratorSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Experimental NoiseLayerSO Editor within array
        var noiseGen = (NoiseGeneratorSO)target;
        var noiseLayers = noiseGen.GetNoiseLayers();


        for (int i = 0; i < noiseLayers.Count; i++)
        {
            if(noiseLayers[i] == null)
            {
                // Create filed for user to drag noise layer in
                noiseLayers[i] = (NoiseLayerSO)EditorGUILayout.ObjectField(
                        label: "Noise Layer",
                        obj: noiseLayers[i],
                        objType: typeof(NoiseLayerSO),
                        allowSceneObjects: false
                    );
            }
            else
            {
                Editor editor = CreateEditor(noiseLayers[i]);
                editor.OnInspectorGUI();

            }

            if(GUILayout.Button(new GUIContent("Remove Layer")))
                    noiseLayers.Remove(noiseLayers[i]);

        }

        GUILayout.Space(10f);
        // Add New Layer Button
        if (GUILayout.Button(new GUIContent("Add New Layer"))) noiseLayers.Add(null);

    }
}
