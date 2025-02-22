using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor {
    public override void OnInspectorGUI() {
        MapGenerator mapGen = target as MapGenerator; // object that this custom editor is inspecting
        
        if (DrawDefaultInspector() && mapGen.autoUpdate) {
            mapGen.GenerateMap();
        }

        if (GUILayout.Button("Generate")) {
            mapGen.GenerateMap();
        }

    }

}
