using UnityEngine;
using UnityEditor;

using TerrainGenerator;

[CustomEditor(typeof(TerrainGeneratorMB))]
public class TerrainEditor : Editor
{
    private void OnSceneGUI() {
        TerrainGeneratorMB tg = (TerrainGeneratorMB)target;
        Vector3 dimensions = new(tg.GetTerrainSideLength(), tg.GetTerrainHeight(), tg.GetTerrainSideLength());
        Handles.DrawWireCube(tg.transform.position, dimensions);   
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
    }
}
