using UnityEngine;
using UnityEditor;
using TerrainGeneratorAsset;
using System.Collections.Generic;
using NUnit.Framework;
using System;

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
                editor.OnInspectorGUI(); // Display the serializable paremeters

                // Draw a texture preview

                const int texPreviewLength = 200;
                Rect previewRect = GUILayoutUtility.GetRect(texPreviewLength, texPreviewLength);
                Texture tex = GenerateTexturePreview(noiseLayers[i], texPreviewLength);
                EditorGUI.DrawPreviewTexture(previewRect, tex);
            }

            if (GUILayout.Button(new GUIContent("Remove Layer")))
                    noiseLayers.Remove(noiseLayers[i]);

        }

        GUILayout.Space(10f);
        // Add New Layer Button
        if (GUILayout.Button(new GUIContent("Add New Layer"))) noiseLayers.Add(null);

    }

    internal static Texture2D GenerateTexturePreview(NoiseLayerSO noise, int texWidth)
    {
        var tex = new Texture2D(texWidth, texWidth, TextureFormat.RGBA32, false);

        const float xMin = 0.0f;
        const float xMax = 1.0f;
        const float yMin = 0.0f;
        const float yMax = 1.0f;

        Func<float, float, float> sampler = noise.Evaluate;

        Color32[] pixels = new Color32[texWidth * texWidth];

        for (int i = 0; i < texWidth; i++)
        for (int j = 0; j < texWidth; j++)
        {
                float u = Mathf.Lerp(xMin, xMax, (float)i / (texWidth - 1));
                float v = Mathf.Lerp(yMin, yMax, (float)j / (texWidth - 1));

                float value = Mathf.Clamp01(sampler(u, v));
                pixels[j + i * texWidth] = new Color(value, value, value, 1f);
        }

        tex.SetPixels32(pixels);
        tex.Apply(false);

        return tex;
    }
}
