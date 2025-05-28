using _Scripts.NoiseSystem;
using _Scripts.NoiseSystem.ScriptableObjects;
using UnityEditor;
using UnityEngine;

namespace _Scripts.Editor
{
    public class NoiseLayerPreviewWindow : EditorWindow
    {
        [MenuItem("Tools/Noise Layer Preview")]
        public static void ShowWindow()
        {
            var window = GetWindow<NoiseLayerPreviewWindow>();
            window.titleContent = new GUIContent("Noise Layer Preview");
            window.Show();
        }
        
        private NoiseLayerSO _noiseLayerSo;
        private Texture2D _previewTexture;
        private SerializedObject _serializedSettings;
        private Vector2Int _previewResolution = new Vector2Int(256, 256);
        
        private void OnGUI()
        {
            _noiseLayerSo = (NoiseLayerSO)EditorGUILayout.ObjectField("Noise Layer", _noiseLayerSo, typeof(NoiseLayerSO), false);
            
            
            if (!_noiseLayerSo) return;

            // Create or update the serialized object
            if (_serializedSettings == null || _serializedSettings.targetObject != _noiseLayerSo)
            {
                _serializedSettings = new SerializedObject(_noiseLayerSo);
                _previewTexture = GenerateNoisePreview(_noiseLayerSo, _previewResolution.x, _previewResolution.y);
            }

            // Dynamically draw fields
            _serializedSettings.Update();
            var property = _serializedSettings.GetIterator();
            property.NextVisible(true); // Skip script
            while (property.NextVisible(false))
            {
                EditorGUILayout.PropertyField(property, true);
            }

            if (_serializedSettings.hasModifiedProperties)
            {
                _serializedSettings.ApplyModifiedProperties();
                _previewTexture = GenerateNoisePreview(_noiseLayerSo, _previewResolution.x, _previewResolution.y);
                Repaint();
            }
            else
            {
                _serializedSettings.ApplyModifiedProperties();
            }

            if (_previewTexture)
            {
                GUILayout.Label(_previewTexture, GUILayout.Width(_previewResolution.x), GUILayout.Height(_previewResolution.y));
            }
        }
        private Texture2D GenerateNoisePreview(NoiseLayerSO layerSo, int width, int height)
        {
            var tex = new Texture2D(width, height);
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var point = new Vector2(x, y);
                    var value = layerSo.Evaluate(point);
                    tex.SetPixel(x, y, new Color(value, value, value));
                }
            }
            tex.Apply();
            return tex;
        }
    }
}