using NUnit.Framework;
using System;
using UnityEngine;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour {
    #region Fields&Consts
    [Header("Values")]
    [SerializeField] GenerateNoiseMapParams parameters;
    [SerializeField] List<TerrainType> regions;
    
    public bool autoUpdate;

    [Header("Constants")]
    private const float SMALL_NUMBER = 0.001f;
    private const int ONE = 1;

    #endregion
    
    public enum DrawMode {
        NoiseMap,
        ColorMap,
        Mesh
    }

    public DrawMode drawMode;
    public void GenerateMap() {
        float[,] noiseMap = Noise.GenerateNoiseMap(parameters);
        MapDisplay display = GetComponent<MapDisplay>();
        Color[] colorMap = GenerateColorMap(noiseMap);

        if (drawMode == DrawMode.NoiseMap) {
            display.DrawTexture(TextureGenerator.GetTextureFromHeightMap(noiseMap));
        } else if(drawMode == DrawMode.ColorMap) {
            display.DrawTexture(TextureGenerator.GetTextureFromColorMap(colorMap, parameters.mapChunkSize, parameters.mapChunkSize));
        } else if(drawMode == DrawMode.Mesh) {
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, parameters.meshHeightMultipler, parameters.meshHeightCurve, parameters.levelOfDetail),
                TextureGenerator.GetTextureFromColorMap(colorMap, parameters.mapChunkSize, parameters.mapChunkSize)
            );
        }

    }
    private Color[] GenerateColorMap(float[,] noiseMap) {
        Color[] colorMap = new Color[parameters.mapChunkSize * parameters.mapChunkSize];
        for (int y = 0; y < parameters.mapChunkSize; y++) {
            for (int x = 0; x < parameters.mapChunkSize; x++) {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Count; i++) { //yo
                    if (currentHeight <= regions[i].height) {
                        colorMap[y * parameters.mapChunkSize + x] = regions[i].color;
                        break;
                    }
                }
            }
        }
        return colorMap;
    }
    void OnValidate() {
        ValidateFields();
    }
    void ValidateFields() {
        if (parameters.noiseScale <= 0) parameters.noiseScale = SMALL_NUMBER;
        if (parameters.persistance <= 0) parameters.persistance = SMALL_NUMBER;
        if (parameters.persistance > 1) parameters.persistance = ONE;
        if (parameters.lacunarity <= 0) parameters.lacunarity = SMALL_NUMBER;
        if (parameters.octaves <= 0) parameters.octaves = ONE;
    }

    [Serializable]
    public struct TerrainType {
        public string name;
        public float height;
        public Color color;
    }
}
