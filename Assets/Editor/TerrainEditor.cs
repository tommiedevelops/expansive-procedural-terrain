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
    Vector3 terrainDimensions  = Vector3.zero;
    Vector2Int terrainResolution = Vector2Int.zero;
    Vector3 terrainOrigin = Vector2.zero;
    Func<float, float, float> heightFunc = null;
    PreviewType previewType = PreviewType.Wireframe;

    void DrawTexturePreview() {
        //TODO
    }

    void DrawWireframePreview() {

        Vector3 startCoord = terrainOrigin - (0.5f * terrainDimensions);
        startCoord.y = 0;

        float distanceBetweenXPoints = terrainDimensions.x / terrainResolution.x;
        float distanceBetweenZPoints = terrainDimensions.z / terrainResolution.y;

        for(int x = 0; x < terrainResolution.x - 1; x++) {
            for(int z = 0; z < terrainResolution.y - 1; z++) {
                float xDist = distanceBetweenXPoints * x;
                float zDist = distanceBetweenZPoints * z;

                Vector3 a = new Vector3(x*xDist, 0, z*zDist);
                Vector3 b = new Vector3(x*xDist, 0, (z+1)*zDist);
                Vector3 c = new Vector3((x + 1) * xDist, 0, (z + 1) * zDist);
                Vector3 d = new Vector3(x*xDist, 0, (z+1)*zDist);

                Handles.DrawLine(a, b);
                Handles.DrawLine(b, c);
                Handles.DrawLine(c, d);
                Handles.DrawLine(a, d);
                Handles.DrawLine(b, d);
            }
        }

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
