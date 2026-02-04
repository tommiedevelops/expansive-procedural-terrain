using UnityEngine;
using UnityEditor;
using System;
using TerrainGeneratorAsset;
using static TerrainGeneratorAsset.PlaneMeshGenerator;

[CustomEditor(typeof(TerrainGeneratorMB))]
public class TerrainGeneratorMBEditor : Editor
{
    public enum PreviewType { None, Texture, Wireframe, Vertices, Mesh }
    private PreviewType m_previewType = PreviewType.Wireframe;

    private Editor m_terrainConfigSOEditor;
    private Editor m_noiseGeneratorSOEditor;
    private TerrainConfigSO m_terrainConfig;
    private NoiseGeneratorSO m_noiseGen;

    private bool _showNoiseGeneratorSettings = false;
    private bool _showTerrainConfigSettings = false;
    private bool _showTerrainPreviewSettings = false;
    private void OnEnable()
    {
        var terrainGen = (TerrainGeneratorMB)target;
        m_terrainConfig = terrainGen.GetTerrainConfigSO();
        m_noiseGen = terrainGen.GetNoiseGeneratorSO();

        if(!m_terrainConfig || !m_noiseGen) return;

        m_terrainConfigSOEditor = Editor.CreateEditor(m_terrainConfig);
        m_noiseGeneratorSOEditor = Editor.CreateEditor(m_noiseGen);
    }
    private void OnSceneGUI()
    {

        TerrainGeneratorMB tg = (TerrainGeneratorMB)target;

        var terrainConfig = tg.GetTerrainConfigSO();
        if (terrainConfig == null) return;

        var noiseGen = tg.GetNoiseGeneratorSO();
        if (noiseGen == null) return;

        var heightFunc = noiseGen.GetHeightFunc();

        // Draw Terrain Boundary
        OnSceneGUI_DrawBoundaries(tg.transform, terrainConfig.terrainDimensions);

        // Draw Preview
        switch (m_previewType)
        {
            case PreviewType.None:
                return;
            case PreviewType.Texture:
                OnSceneGUI_DrawTexturePreview();
                break;
            case PreviewType.Wireframe:
                OnSceneGUI_DrawWireframePreview(terrainConfig, tg.transform, heightFunc);
                break;
            case PreviewType.Mesh:
                //DrawMeshPreview(terrainConfig);
                break;
            case PreviewType.Vertices:
                //DrawVerticesPreview(terrainConfig);
                break;
            default:
                OnSceneGUI_DrawVerticesPreview();
                break;
        }

    }
    public override void OnInspectorGUI()
    {
        OnInspectorGUI_TerrainConfigSOSelection();
        OnInspectorGUI_TerrainConfigSOModification();
        OnInspectorGUI_TerrainConfigPreviewSettings();
        OnInspectorGUI_NoiseGeneratorSOSelection();
        OnInspectorGUI_NoiseGeneratorSOModification();
    }
    void OnSceneGUI_DrawTexturePreview()
    {
        //TODO
    }
    void OnSceneGUI_DrawWireframePreview(TerrainConfigSO terrainConfig, Transform terrainTr, Func<float,float,float> heightFunc)
    {
        Vector3 startCoord = terrainTr.position - (0.5f * terrainConfig.terrainDimensions);
        startCoord.y += 0.5f * terrainConfig.terrainDimensions.y;

        float xDist = terrainConfig.terrainDimensions.x / terrainConfig.resolutionX;
        float zDist = terrainConfig.terrainDimensions.z / terrainConfig.resolutionY;

        for (int x = 0; x < terrainConfig.resolutionX; x++)
            for (int z = 0; z < terrainConfig.resolutionY; z++)
            {
                float currX = x * xDist;
                float currZ = z * zDist;
                float nextX = (x + 1) * xDist;
                float nextZ = (z + 1) * zDist;

                var a = new Vector3(currX, heightFunc(currX, currZ), currZ);
                var b = new Vector3(currX, heightFunc(currX, nextZ), nextZ);
                var c = new Vector3(nextX, heightFunc(nextX, currZ), currZ);
                var d = new Vector3(nextX, heightFunc(nextX, nextZ), nextZ);

                a = terrainTr.TransformPoint((startCoord + a));
                b = terrainTr.TransformPoint((startCoord + b));
                c = terrainTr.TransformPoint((startCoord + c));
                d = terrainTr.TransformPoint((startCoord + d));
                
                Handles.DrawLine(a, b);
                Handles.DrawLine(b, c);
                Handles.DrawLine(c, d);
                Handles.DrawLine(a, d);
                Handles.DrawLine(a, c);
            }

    }
    void OnSceneGUI_DrawMeshPreview()
    {
        // Generate Plane Mesh with specified parameters
        //Mesh mesh = GeneratePlaneMesh(m_terrainDimensions.x, m_terrainDimensions.y, m_terrainResolution.x, m_terrainResolution.y, m_terrainOrigin, m_heightFunc);
        //Graphics.DrawMeshNow(mesh, Matrix4x4.identity);
    }
    private void OnSceneGUI_DrawVerticesPreview()
    {
        throw new NotImplementedException();
    }
    private void OnSceneGUI_DrawBoundaries(Transform terrainTransform, Vector3 terrainDimensions)
    {
        Handles.color = Color.red;
        Handles.matrix = terrainTransform.localToWorldMatrix;
        Handles.DrawWireCube(Vector3.zero, terrainDimensions);
    }
    void OnInspectorGUI_NoiseGeneratorSOSelection()
    {
        SerializedProperty noiseGen_Prop = serializedObject.FindProperty("noiseGen");
        serializedObject.Update();
        EditorGUILayout.PropertyField(noiseGen_Prop);
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndFoldoutHeaderGroup();
    }
    void OnInspectorGUI_NoiseGeneratorSOModification()
    {
        // Drop Down for Noise Generator Settings
        _showNoiseGeneratorSettings = EditorGUILayout.BeginFoldoutHeaderGroup(
                _showNoiseGeneratorSettings,
                "Noise Generator Settings"
            );

        if (_showNoiseGeneratorSettings) m_noiseGeneratorSOEditor.OnInspectorGUI();

        EditorGUILayout.EndFoldoutHeaderGroup();
    }
    void OnInspectorGUI_TerrainConfigSOSelection()
    {
        SerializedProperty terrainConfig_Prop = serializedObject.FindProperty("terrainConfig");
        serializedObject.Update();
        EditorGUILayout.PropertyField(terrainConfig_Prop);
        serializedObject.ApplyModifiedProperties();
    }
    void OnInspectorGUI_TerrainConfigSOModification()
    {
        // Modifying Selected Terrain Config
        _showTerrainConfigSettings = EditorGUILayout.BeginFoldoutHeaderGroup(
                _showTerrainConfigSettings,
                "Terrain Configuration"
            );

        if (_showTerrainConfigSettings) m_terrainConfigSOEditor.OnInspectorGUI();

        EditorGUILayout.EndFoldoutHeaderGroup();
    }
    void OnInspectorGUI_TerrainConfigPreviewSettings()
    {
        _showTerrainPreviewSettings = EditorGUILayout.BeginFoldoutHeaderGroup(
                _showTerrainPreviewSettings,
                "Terrain Preview Settings"
            );

        EditorGUILayout.EndFoldoutHeaderGroup();

        if (_showTerrainPreviewSettings)
        {
            m_previewType = (PreviewType)EditorGUILayout.EnumPopup("Preview Type", m_previewType);
        }

    }
}
