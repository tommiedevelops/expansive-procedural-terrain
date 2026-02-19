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

            float prev = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 110f;
            var buttonWidth = GUILayout.Width(200f);
            var buttonHeight = GUILayout.Height(40f);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(terrainConfig_Prop, buttonHeight);
            GUILayout.Button("Create new Terrain Config", buttonWidth, buttonHeight);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(noiseGenerator_Prop, buttonHeight);
            GUILayout.Button("Create new Noise Generator", buttonWidth, buttonHeight);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            m_previewType = (PreviewType)EditorGUILayout.EnumPopup("Preview Type", m_previewType);

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
