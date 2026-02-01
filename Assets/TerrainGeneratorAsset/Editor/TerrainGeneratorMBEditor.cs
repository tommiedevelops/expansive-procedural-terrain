using UnityEngine;
using UnityEditor;
using System;
using TerrainGeneratorAsset;
using static TerrainGeneratorAsset.PlaneMeshGenerator;

[CustomEditor(typeof(TerrainGeneratorMB))]
public class TerrainGeneratorMBEditor : Editor
{
    public enum PreviewType { Texture, Wireframe, Vertices, Mesh }
    Vector3 m_terrainDimensions = Vector3.zero;
    Vector2Int m_terrainResolution = Vector2Int.zero;
    Vector3 m_terrainOrigin = Vector2.zero;
    Func<float, float, float> m_heightFunc = null;
    PreviewType m_previewType = PreviewType.Wireframe;
    Matrix4x4 modelMatrix = Matrix4x4.identity;
    private Editor m_terrainConfigSOEditor;
    private Editor m_noiseGeneratorSOEditor;
    private bool _showNoiseGeneratorSettings = false;
    private bool _showTerrainConfigSettings = false;
    private bool _showTerrainPreviewSettings = false;
    private void OnSceneGUI()
    {
        TerrainGeneratorMB tg = (TerrainGeneratorMB)target;

        var terrainConfig = tg.GetTerrainConfigSO();
        if (terrainConfig == null) return;

        var noiseGen = tg.GetNoiseGeneratorSO();
        if (noiseGen == null) return;

        m_terrainDimensions = terrainConfig.terrainDimensions;
        m_terrainResolution = new Vector2Int(terrainConfig.resolutionX, terrainConfig.resolutionY);
        m_terrainOrigin = tg.transform.position;
        m_heightFunc = tg.GetNoiseGeneratorSO().GetHeightFunc();
        modelMatrix = tg.transform.localToWorldMatrix;

        // Draw Terrain Boundary
        DrawBoundaries(tg.transform.position, m_terrainDimensions);

        // Draw Preview
        switch (m_previewType)
        {
            case PreviewType.Texture:
                DrawTexturePreview();
                break;
            case PreviewType.Wireframe:
                DrawWireframePreview();
                break;
            case PreviewType.Mesh:
                DrawMeshPreview();
                break;
            case PreviewType.Vertices:
                DrawVerticesPreview();
                break;
            default:
                DrawVerticesPreview();
                break;
        }

    }
    private void OnEnable()
    {
        m_terrainConfigSOEditor = Editor.CreateEditor(((TerrainGeneratorMB)target).GetTerrainConfigSO());
        m_noiseGeneratorSOEditor = Editor.CreateEditor(((TerrainGeneratorMB)target).GetNoiseGeneratorSO());
    }
    public override void OnInspectorGUI()
    {
        TerrainConfigSOSelection();
        TerrainConfigSOModification();
        TerrainConfigPreviewSettings();
        NoiseGeneratorSOSelection();
        NoiseGeneratorSOModification();
    }
    void DrawTexturePreview()
    {
        //TODO
    }
    void DrawWireframePreview()
    {

        Vector3 startCoord = m_terrainOrigin - (0.5f * m_terrainDimensions);
        startCoord.y += 0.5f * m_terrainDimensions.y;

        float xDist = m_terrainDimensions.x / m_terrainResolution.x;
        float zDist = m_terrainDimensions.z / m_terrainResolution.y;

        for (int x = 0; x < m_terrainResolution.x; x++)
            for (int z = 0; z < m_terrainResolution.y; z++)
            {
                float currX = x * xDist;
                float currZ = z * zDist;
                float nextX = (x + 1) * xDist;
                float nextZ = (z + 1) * zDist;

                var a = new Vector3(currX, m_heightFunc(currX, currZ), currZ);
                var b = new Vector3(currX, m_heightFunc(currX, nextZ), nextZ);
                var c = new Vector3(nextX, m_heightFunc(nextX, currZ), currZ);
                var d = new Vector3(nextX, m_heightFunc(nextX, nextZ), nextZ);

                a = modelMatrix * (startCoord + a);
                b = modelMatrix * (startCoord + b);
                c = modelMatrix * (startCoord + c);
                d = modelMatrix * (startCoord + d);

                Handles.DrawLine(a, b);
                Handles.DrawLine(b, c);
                Handles.DrawLine(c, d);
                Handles.DrawLine(a, d);
                Handles.DrawLine(a, c);
            }

    }
    void DrawMeshPreview()
    {
        // Generate Plane Mesh with specified parameters
        Mesh mesh = GeneratePlaneMesh(m_terrainDimensions.x, m_terrainDimensions.y, m_terrainResolution.x, m_terrainResolution.y, m_terrainOrigin, m_heightFunc);
        Graphics.DrawMeshNow(mesh, Matrix4x4.identity);
    }
    private void DrawVerticesPreview()
    {
        throw new NotImplementedException();
    }
    private void DrawBoundaries(Vector3 origin, Vector3 terrainDimensions)
    {
        Handles.DrawWireCube(origin, terrainDimensions);
    }
    void NoiseGeneratorSOSelection()
    {
        var noiseGen = serializedObject.FindProperty("noiseGen");
        serializedObject.Update();
        EditorGUILayout.PropertyField(noiseGen);
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndFoldoutHeaderGroup();
    }
    void NoiseGeneratorSOModification()
    {
        // Modifying Selected Terrain Config
        _showNoiseGeneratorSettings = EditorGUILayout.BeginFoldoutHeaderGroup(
                _showNoiseGeneratorSettings,
                "Noise Generator Settings"
            );

        if (_showNoiseGeneratorSettings) m_noiseGeneratorSOEditor.OnInspectorGUI();

        EditorGUILayout.EndFoldoutHeaderGroup();
    }
    void TerrainConfigSOSelection()
    {
        var terrainConfig = serializedObject.FindProperty("terrainConfig");
        serializedObject.Update();
        EditorGUILayout.PropertyField(terrainConfig);
        serializedObject.ApplyModifiedProperties();
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
    void TerrainConfigPreviewSettings()
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
