
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
        NoiseLayerSO noiseLayer = target as NoiseLayerSO;
        const int texPreviewLength = 200;
        Rect previewRect = GUILayoutUtility.GetRect(texPreviewLength, texPreviewLength);
        Texture2D tex = OnInspectorGUI_GenerateTexturePreview(noiseLayer, texPreviewLength);
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