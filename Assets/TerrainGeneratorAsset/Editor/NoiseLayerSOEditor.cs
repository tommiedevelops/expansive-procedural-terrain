
using TerrainGeneratorAsset;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NoiseLayerSO))]
public class NoiseLayerSOEditor : Editor
{
    internal Texture2D _previewTexture;
    internal Vector2Int _previewResolution;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (_previewTexture)
            GUILayout.Label(_previewTexture, GUILayout.Width(_previewResolution.x), GUILayout.Height(_previewResolution.y));


    }
    private Texture2D OnInspectorGUI_GenerateNoisePreview(NoiseLayerSO layerSo, int width, int height)
    {
        var tex = new Texture2D(width, height);
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                var value = layerSo.Evaluate(x, y);
                tex.SetPixel(x, y, new Color(value, value, value));
            }
        }
        tex.Apply();
        return tex;
    }
}