using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor
{
    
    public class TestEditor : EditorWindow
    {
        [MenuItem("Tools/Test")]
        public static void ShowWindow()
        {
            var window = GetWindow<TestEditor>();
            window.titleContent = new GUIContent("Height map editor");
        }

        public void OnEnable()
        {
            // load UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UIToolkit/TestUXML.uxml");
            VisualElement root = visualTree.Instantiate();
            rootVisualElement.Add(root);
            
            var visualElement = root.Q<VisualElement>("heightMapDisplay");
            var slider = root.Q<Slider>();


            slider?.RegisterValueChangedCallback(evt =>
            {
                visualElement.style.backgroundImage = new StyleBackground(GenerateHeightmapTexture(slider.value));
            });



        }
        
        private Texture2D GenerateHeightmapTexture(float num)
        {
            int width = 128;
            int height = 128;
            Texture2D tex = new Texture2D(width, height);

            for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                float value = Mathf.PerlinNoise(x * num, y * num);
                tex.SetPixel(x, y, new Color(value, value, value));
            }

            tex.Apply();
            return tex;
        }
    }
}
