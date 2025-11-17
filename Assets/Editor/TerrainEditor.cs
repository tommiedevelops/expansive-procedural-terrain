using UnityEngine;
using UnityEditor;

using TerrainGenerator;
using TerrainGenerator.MeshGenerators;
using static TerrainGenerator.MeshGenerators.PlaneMeshGenerator;
using System;

[CustomEditor(typeof(TerrainGeneratorMB))]
public class TerrainEditor : Editor
{

    public enum PreviewType { Texture, Wireframe, Mesh }
    Vector3 terrainDimensions = Vector3.zero;
    Vector2Int terrainResolution = Vector2Int.zero;
    Vector3 terrainOrigin = Vector3.zero;
    Func<float, float, float> heightFunc = null;
    PreviewType previewType = PreviewType.Wireframe;

    void DrawTexturePreview() {
        //TODO
    }

    void DrawWireframePreview() {
        // TODO
    }

    void DrawMeshPreview() {
        // Generate Plane Mesh with specified parameters
        Mesh mesh = GeneratePlaneMesh(terrainDimensions.x,terrainDimensions.y,terrainResolution.x,terrainResolution.y, terrainOrigin, heightFunc);
        Graphics.DrawMeshNow(mesh, terrainOrigin, Quaternion.identity);
    }

    private void OnSceneGUI() {
        TerrainGeneratorMB tg = (TerrainGeneratorMB)target;

        terrainDimensions = tg.GetTerrainDimensions();
        terrainResolution = tg.GetTerrainResolution();
        terrainOrigin = tg.transform.position;
        heightFunc = tg.GetNoiseGenerator().GetHeightFunc();

        // Draw Terrain Boundary
        Handles.DrawWireCube(tg.transform.position, terrainDimensions);

        // Draw Preview
        switch (previewType) {
            case PreviewType.Texture:
                DrawTexturePreview();
                break;
            case PreviewType.Wireframe:
                DrawWireframePreview();
                break;
            case PreviewType.Mesh:
                DrawMeshPreview();
                break;
        }

    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        previewType = (PreviewType)EditorGUILayout.EnumPopup("Preview Type", previewType);
    }
}
