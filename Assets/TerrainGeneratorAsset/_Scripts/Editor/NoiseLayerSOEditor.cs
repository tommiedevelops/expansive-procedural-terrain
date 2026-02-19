
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

        // List Serialized Fields
        var it = serializedObject.GetIterator();
        bool enterChildren = true;
        while(it.NextVisible(enterChildren))
        {
            if (it.propertyPath == "m_Script") continue;
            EditorGUILayout.PropertyField(it, includeChildren: true);
            enterChildren = false;
        }

        // Draw Texture Preview
        const int texPreviewLength = 200;
        Rect previewRect = GUILayoutUtility.GetRect(texPreviewLength, texPreviewLength);
        Texture2D tex = GeneratePreviewTexture(noiseLayer, texPreviewLength);
        EditorGUI.DrawPreviewTexture(previewRect, tex);

        serializedObject.ApplyModifiedProperties(); 
    }
    private Texture2D GeneratePreviewTexture(NoiseLayerSO noise, int texWidth)
    {
        var tex = new Texture2D(texWidth, texWidth, TextureFormat.RGBA32, false);

        Func<float, float, float> sampler = noise.Evaluate;

        float pixelLengthX = m_terrainConfig.terrainDimensions.x / (texWidth - 1);
        float pixelLengthZ = m_terrainConfig.terrainDimensions.z / (texWidth - 1);

        Color32[] pixels = new Color32[texWidth * texWidth];

        for (int i = 0; i < texWidth; i++)
            for (int j = 0; j < texWidth; j++)
            {
                float value = Mathf.Clamp01(sampler(i * pixelLengthX, j * pixelLengthZ));
                pixels[j + i * texWidth] = new Color(value, value, value, 1f);
            }

        tex.SetPixels32(pixels);
        tex.Apply(false);

        return tex;
    }

    public void SetTerrainConfigSO(TerrainConfigSO terrainConfig)
    {
       m_terrainConfig = terrainConfig; 
    }

}