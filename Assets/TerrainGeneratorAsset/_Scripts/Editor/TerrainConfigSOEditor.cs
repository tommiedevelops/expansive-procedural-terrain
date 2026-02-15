
using TerrainGeneratorAsset;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TerrainConfigSO))]
public class TerrainConfigSOEditor : Editor
{
    public Vector3 terrainDimensions = Vector3.zero;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();    
        var tg = (TerrainConfigSO)target;    
        terrainDimensions = tg.terrainDimensions;
    }
}