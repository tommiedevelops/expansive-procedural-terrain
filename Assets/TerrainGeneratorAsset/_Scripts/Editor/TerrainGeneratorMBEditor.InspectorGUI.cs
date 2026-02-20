using UnityEditor;
using UnityEngine;
using System;

namespace TerrainGeneratorAsset
{
    // TerrainGeneratorMBEditor partial class that only contains OnInpsectorGUI logic
    public partial class TerrainGeneratorMBEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (m_noiseGeneratorSOEditor == null || m_terrainConfigSOEditor == null) return; // currently not handling this case

            serializedObject.Update();

            SerializedProperty terrainConfig_Prop = serializedObject.FindProperty("m_terrainConfig");
            SerializedProperty noiseGenerator_Prop = serializedObject.FindProperty("m_noiseGenerator");
            SerializedProperty terrainViewer_Prop = serializedObject.FindProperty("m_terrainViewer");


            float prev = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 110f;
            var minButtonWidth = GUILayout.MinWidth(50f);
            var buttonHeight = GUILayout.Height(40f);
            var maxButtonWidth = GUILayout.MaxWidth(100f);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(terrainViewer_Prop, buttonHeight);
            GUILayout.Button("Create New", minButtonWidth, buttonHeight, maxButtonWidth); // creates new gameobject in the scene and assigns it automatically
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(terrainConfig_Prop, buttonHeight);
            GUILayout.Button("Create New", minButtonWidth, buttonHeight, maxButtonWidth); // creates new terrain config in files and assigns it
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(noiseGenerator_Prop, buttonHeight);
            GUILayout.Button("Create New", minButtonWidth, buttonHeight, maxButtonWidth); // creates new noisegenerator in files and assigns it
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            m_previewType = (PreviewType)EditorGUILayout.EnumPopup("Preview Type", m_previewType, buttonHeight);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Terrain Config SO Editor", EditorStyles.boldLabel);

            m_terrainConfigSOEditor.OnInspectorGUI();

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Noise Generator SO Editor", EditorStyles.boldLabel);
            m_noiseGeneratorSOEditor.SetTerrainConfigSO(m_terrainConfig);
            m_noiseGeneratorSOEditor.OnInspectorGUI();

            EditorGUIUtility.labelWidth = prev;

            serializedObject.ApplyModifiedProperties();

        }
    }
}
