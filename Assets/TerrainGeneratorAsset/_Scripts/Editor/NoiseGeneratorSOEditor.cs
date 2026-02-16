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
        // Experimental NoiseLayerSO Editor within array
        var noiseGen = (NoiseGeneratorSO)target;
        var noiseLayers = noiseGen.GetNoiseLayers();

        for (int i = 0; i < noiseLayers.Count; i++)
        {
            if(noiseLayers[i] == null)
            {
                // Create field for user to drag noise layer in
                noiseLayers[i] = (NoiseLayerSO)EditorGUILayout.ObjectField(
                        label: "Noise Layer",
                        obj: noiseLayers[i],
                        objType: typeof(NoiseLayerSO),
                        allowSceneObjects: false
                    );
            }
            else
            {
                bool showPreview = false;
                showPreview = EditorGUILayout.Toggle("Show Texture Preview", showPreview);

                var editor = CreateEditor(noiseLayers[i], typeof(NoiseLayerSOEditor));
                editor.OnInspectorGUI(); // Display usual Serialized Fields

                if (showPreview)
                    OnInspectorGUI_DrawNoiseTexturePreview(noiseLayers, i);
            }

            if (GUILayout.Button(new GUIContent("Remove Layer")))
                    noiseLayers.Remove(noiseLayers[i]);

        }

        GUILayout.Space(10f);
        // Add New Layer Button
        if (GUILayout.Button(new GUIContent("Add New Layer"))) noiseLayers.Add(null);

    }
    private void OnInspectorGUI_DrawNoiseTexturePreview(List<NoiseLayerSO> noiseLayers, int i)
    {
        // Draw a texture preview
        const int texPreviewLength = 200;
        Rect previewRect = GUILayoutUtility.GetRect(texPreviewLength, texPreviewLength);
        Texture2D tex = OnInspectorGUI_GenerateTexturePreview(noiseLayers[i], texPreviewLength);
        EditorGUI.DrawPreviewTexture(previewRect, tex);
    }
    private Texture2D OnInspectorGUI_GenerateTexturePreview(NoiseLayerSO noise, int texWidth)
    {
        var tex = new Texture2D(texWidth, texWidth, TextureFormat.RGBA32, false);

        Func<float, float, float> sampler = noise.Evaluate;

        float pixelLengthX = m_terrainConfig.terrainDimensions.x / (texWidth - 1);
        float pixelLengthZ = m_terrainConfig.terrainDimensions.z / (texWidth - 1);

        Color32[] pixels = new Color32[texWidth * texWidth];

        for (int i = 0; i < texWidth; i++)
        for (int j = 0; j < texWidth; j++)
        {
                float value = Mathf.Clamp01(sampler(i*pixelLengthX, j*pixelLengthZ));
                pixels[j + i * texWidth] = new Color(value, value, value, 1f);
        }

        tex.SetPixels32(pixels);
        tex.Apply(false);

        return tex;
    }
}
