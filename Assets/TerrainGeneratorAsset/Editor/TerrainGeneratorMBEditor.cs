using Codice.CM.WorkspaceServer.Tree;
using System;
using TerrainGeneratorAsset;
using UnityEditor;
using UnityEngine;

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
        // High level idea: Do everything in object space (origin is 0), then convert to world space using the localToWorld matrix
        // 'OS' suffix denotes an Object Space value

        // Set to a param soon
        Handles.color = Color.white;

        if (!terrainConfig) return;

        float xLengthOS = terrainConfig.terrainDimensions.x;
        float zLengthOS = terrainConfig.terrainDimensions.z;
        float yLengthOS = 0.5f * terrainConfig.terrainDimensions.y;
        int resolutionX = terrainConfig.resolutionX;
        int resolutionZ = terrainConfig.resolutionZ;

        float xDistBetweenPtsOS = xLengthOS / resolutionX;
        float zDistBetweenPtsOS = zLengthOS / resolutionZ;

        // starting coordinate in object space
        float startX = -0.5f * xLengthOS;
        float startZ = -0.5f * zLengthOS;

        for (int x = 0; x < resolutionX; ++x)
        for (int z = 0; z < resolutionZ; ++z)
        {
                // currX, currZ, nextX, nextZ all in Object Space
                float currX = startX + x * xDistBetweenPtsOS;
                float currZ = startZ + z * zDistBetweenPtsOS;
                float nextX = startX + (x + 1) * xDistBetweenPtsOS;
                float nextZ = startZ + (z + 1) * zDistBetweenPtsOS;

                float aHeight = yLengthOS * heightFunc(currX, currZ);
                float bHeight = yLengthOS * heightFunc(currX, nextZ);
                float cHeight = yLengthOS * heightFunc(nextX, nextZ);
                float dHeight = yLengthOS * heightFunc(nextX, currZ);

                var a = new Vector3(currX, aHeight, currZ);
                var b = new Vector3(currX, bHeight, nextZ);
                var c = new Vector3(nextX, cHeight, nextZ);
                var d = new Vector3(nextX, dHeight, currZ);

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
