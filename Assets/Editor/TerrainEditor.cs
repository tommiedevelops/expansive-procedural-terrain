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
    Matrix4x4 model = Matrix4x4.identity;

    void DrawTexturePreview() {
        //TODO
    }

    void DrawWireframePreview() {

        // NEED TO SAMPLE HEIGHTS
        Vector3 startCoord = terrainOrigin - (0.5f * terrainDimensions);
        startCoord.y += 0.5f * terrainDimensions.y;

        float xDist = terrainDimensions.x / terrainResolution.x;
        float zDist = terrainDimensions.z / terrainResolution.y;

        for(int x = 0; x < terrainResolution.x; x++) {
            for(int z = 0; z < terrainResolution.y; z++) {

                Vector3 a = new Vector3(x*xDist, 0, z*zDist);
                Vector3 b = new Vector3(x*xDist, 0, (z+1)*zDist);
                Vector3 c = new Vector3((x + 1) * xDist, 0, (z + 1) * zDist);
                Vector3 d = new Vector3(x*xDist, 0, (z+1)*zDist);

                a = model * (startCoord + a);
                b = model * (startCoord + b);
                c = model * (startCoord + c);
                d = model * (startCoord + d);

                Handles.DrawLine(a, b);
                Handles.DrawLine(b, c);
                Handles.DrawLine(c, d);
                Handles.DrawLine(a, d);
                Handles.DrawLine(a, c);
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
        model = tg.transform.localToWorldMatrix;

        // Draw Terrain Boundary
        DrawBoundaries(tg.transform.position, terrainDimensions);

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

    private void DrawBoundaries(Vector3 origin, Vector3 terrainDimensions) {
        Handles.DrawWireCube(origin, terrainDimensions);
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        previewType = (PreviewType)EditorGUILayout.EnumPopup("Preview Type", previewType);
    }
}
