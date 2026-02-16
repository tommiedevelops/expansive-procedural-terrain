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
            // Create, Save, Load, Modify TerrainConfigs
            EditorGUILayout.LabelField("Terrain Configuration Settings", EditorStyles.boldLabel);

            serializedObject.Update();

            var terrainGen = (TerrainGeneratorMB)target;

            SerializedProperty terrainConfig_Prop = serializedObject.FindProperty("m_terrainConfig");
            SerializedProperty noiseGenerator_Prop = serializedObject.FindProperty("m_noiseGenerator");

            serializedObject.Update();
            EditorGUILayout.PropertyField(terrainConfig_Prop);
            EditorGUILayout.PropertyField(noiseGenerator_Prop);
            serializedObject.ApplyModifiedProperties();

            m_previewType = (PreviewType)EditorGUILayout.EnumPopup("Preview Type", m_previewType);

            TerrainConfigSOModification();
            NoiseGeneratorSOModification();
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
