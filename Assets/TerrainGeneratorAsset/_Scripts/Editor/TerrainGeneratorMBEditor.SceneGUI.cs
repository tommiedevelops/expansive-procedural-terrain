using UnityEditor;
using UnityEngine;
using TerrainGeneratorAsset;
using System;
using static TerrainGeneratorAsset.PlaneMeshGenerator;

namespace TerrainGeneratorAsset
{
    // TerrainGeneratorMBEditor Partial Class that only contains OnSceneGUI Logic
    public partial class TerrainGeneratorMBEditor : Editor
    {
        private void OnSceneGUI()
        {
            TerrainGeneratorMB tg = (TerrainGeneratorMB)target;

            var terrainConfig = tg.GetTerrainConfigSO();
            if (terrainConfig == null) return;

            var noiseGen = tg.GetNoiseGeneratorSO();
            if (noiseGen == null) return;

            var heightFunc = noiseGen.GetHeightFunc();

            // Draw Terrain Boundary
            DrawBoundaries(tg.transform, terrainConfig.terrainDimensions);

            // Draw Preview
            switch (m_previewType)
            {
                case PreviewType.Wireframe:
                    DrawWireframePreview(terrainConfig, tg.transform, heightFunc);
                    break;
                case PreviewType.Mesh:
                    DrawWireframePreview(terrainConfig, tg.transform, heightFunc);
                    DrawMeshPreview(terrainConfig, tg.transform, heightFunc);
                    break;
            }

        }
        private void DrawWireframePreview(TerrainConfigSO terrainConfig, Transform terrainTr, Func<float, float, float> heightFunc)
        {
            Handles.color = Color.red;

            if (!terrainConfig) return;

            float xLengthOS = terrainConfig.terrainDimensions.x;
            float zLengthOS = terrainConfig.terrainDimensions.z;
            float yLengthOS = 0.5f * terrainConfig.terrainDimensions.y;
            int resolutionX = terrainConfig.resolutionX;
            int resolutionZ = terrainConfig.resolutionZ;

            float xDistBetweenPtsOS = xLengthOS / (resolutionX - 1);
            float zDistBetweenPtsOS = zLengthOS / (resolutionZ - 1);

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

                    if (Mathf.Abs(nextX) > terrainConfig.terrainDimensions.x / 2f)
                        nextX = terrainConfig.terrainDimensions.x / 2f;

                    if(Mathf.Abs(nextZ) > terrainConfig.terrainDimensions.z / 2f)
                        nextZ = terrainConfig.terrainDimensions.z / 2f;

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
        private void DrawMeshPreview(TerrainConfigSO cfg, Transform tr, Func<float, float, float> heightFunc)
        {
            // Generate Plane Mesh with specified parameters
            float newHeightFunc(float x, float y)
            {
                return 0.5f * cfg.terrainDimensions.y * heightFunc(x, y);
            }

            var mesh = GeneratePlaneMesh(cfg.terrainDimensions.x, cfg.terrainDimensions.z,
                                         cfg.resolutionX, cfg.resolutionZ, newHeightFunc);

            cfg.terrainMaterial.SetPass(0); // Draw mesh with the provided Material
            Graphics.DrawMeshNow(mesh, tr.localToWorldMatrix);

        }
        private void DrawBoundaries(Transform terrainTransform, Vector3 terrainDimensions)
        {
            Handles.color = Color.red;
            Handles.matrix = terrainTransform.localToWorldMatrix;
            Handles.DrawWireCube(Vector3.zero, terrainDimensions);
        }

    }
}
