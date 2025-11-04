using UnityEngine;
using UnityEditor;

using TerrainGenerator;

[CustomEditor(typeof(TerrainGeneratorMB))]
public class TerrainEditor : Editor
{
    private void OnSceneGUI() {
        TerrainGeneratorMB terrainGenerator = (TerrainGeneratorMB)target; 
        Handles.DrawWireCube(terrainGenerator.transform.position, terrainGenerator.GetTerrainDimensions());   
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
    }
}
