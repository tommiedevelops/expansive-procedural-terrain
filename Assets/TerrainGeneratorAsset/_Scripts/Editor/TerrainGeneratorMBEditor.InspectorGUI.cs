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
            serializedObject.Update();
            // Create, Save, Load, Modify TerrainConfigs
            EditorGUILayout.LabelField("Terrain Configuration Settings", EditorStyles.boldLabel);

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

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(noiseGenerator_Prop, buttonHeight);
            GUILayout.Button("Create new Noise Generator", buttonWidth, buttonHeight);
            EditorGUILayout.EndHorizontal();

            EditorGUIUtility.labelWidth = prev;

            m_previewType = (PreviewType)EditorGUILayout.EnumPopup("Preview Type", m_previewType);

            TerrainConfigSOModification();
            NoiseGeneratorSOModification();

            serializedObject.ApplyModifiedProperties();
        }
        void NoiseGeneratorSOModification()
        {
            // Drop Down for Noise Generator Settings
            _showNoiseGeneratorSettings = EditorGUILayout.BeginFoldoutHeaderGroup(
                    _showNoiseGeneratorSettings,
                    "Noise Generator Settings"
                );

            if (_showNoiseGeneratorSettings)
            {
                m_noiseGeneratorSOEditor.SetTerrainConfigSO(m_terrainConfig);
                m_noiseGeneratorSOEditor.OnInspectorGUI();
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }
        void TerrainConfigSOModification()
        {
            // Modifying Selected Terrain Config
            _showTerrainConfigSettings = EditorGUILayout.BeginFoldoutHeaderGroup(
                    _showTerrainConfigSettings,
                    "Terrain Configuration"
                );

            if (_showTerrainConfigSettings) m_terrainConfigSOEditor.OnInspectorGUI();

            EditorGUILayout.EndFoldoutHeaderGroup();
        }
    }
}
