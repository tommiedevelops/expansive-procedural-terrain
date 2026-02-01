
using TerrainGeneratorAsset;
using UnityEditor;

[CustomEditor(typeof(TerrainConfigSO))]
public class TerrainConfigSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawDefaultInspector();
        serializedObject.ApplyModifiedProperties();
    }
}