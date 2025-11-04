using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TerrainEditor))]
public class TerrainEditor : Editor
{
    private void OnSceneGUI() {
        TerrainEditor editor = (TerrainEditor)target; 
        Handles.DrawWireCube(Vector3.zero, Vector3.one);   
    }
}
