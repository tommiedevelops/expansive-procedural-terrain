using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TerrainGenerator))]
public class MapGeneratorEditor : Editor {
    public override void OnInspectorGUI() {
        TerrainGenerator mapGen = target as TerrainGenerator; // object that this custom editor is inspecting
        
        if (DrawDefaultInspector() && mapGen.autoUpdate) {
            mapGen.DrawMapInEditor();
        }

        if (GUILayout.Button("Generate")) {
            mapGen.DrawMapInEditor();
        }

    }

}
