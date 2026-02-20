
using TerrainGeneratorAsset;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TerrainConfigSO))]
public class TerrainConfigSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        var terrainConfig = (TerrainConfigSO)target;

        var isInfinite_Prop = serializedObject.FindProperty("isInfinite");
        var terrainMaterial_Prop = serializedObject.FindProperty("terrainMaterial");
        var terrainDimensions_Prop = serializedObject.FindProperty("terrainDimensions");
        var resolutionX_Prop = serializedObject.FindProperty("resolutionX");
        var resolutionZ_Prop = serializedObject.FindProperty("resolutionZ");

        var buttonWidth = GUILayout.Width(400f);
        var buttonHeight = GUILayout.Height(40f);

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(terrainMaterial_Prop
                                     , new GUIContent("Material")
                                     , buttonHeight
                                     );

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(terrainDimensions_Prop
                                      , new GUIContent("Dimensions")
                                      , buttonHeight
                                      );

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(resolutionX_Prop
                                     , new GUIContent("X Resolution")
                                     );

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(resolutionZ_Prop
                                     , new GUIContent("Z Resolution")
                                     );

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(20);

        serializedObject.ApplyModifiedProperties();
    }
}