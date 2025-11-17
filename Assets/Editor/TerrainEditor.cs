using UnityEngine;
using UnityEditor;

using TerrainGenerator;
using TerrainGenerator.MeshGenerators;
using static TerrainGenerator.MeshGenerators.PlaneMeshGenerator;

[CustomEditor(typeof(TerrainGeneratorMB))]
public class TerrainEditor : Editor
{

    public enum PreviewType { Texture, Wireframe, Mesh }
    Vector3 terrainDimensions = Vector3.zero;
    Vector2 terrainResolution = Vector2.zero;
    PreviewType previewType = PreviewType.Wireframe;

    void DrawTexturePreview() {
        //TODO
    }

    void DrawWireframePreview() {
        // TODO
    }

    void DrawMeshPreview() {
        // Generate Plane Mesh with specified parameters
        // Mesh mesh = GeneratePlaneMesh(float lengthX, float lengthZ, float resolutionX, float resolutionZ, HeightMap heightMap);
        //Graphics.DrawMeshNow()
    }

    private void OnSceneGUI() {
        TerrainGeneratorMB tg = (TerrainGeneratorMB)target;

        terrainDimensions = tg.GetTerrainDimensions();

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
