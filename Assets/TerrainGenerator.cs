using NUnit.Framework;
using System;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Security.Cryptography;

[RequireComponent(typeof(MapDisplay))]
public class TerrainGenerator : MonoBehaviour {
    #region Declarations
    public readonly struct MapData {
        /* Data required for constructing a Terrain Map */
        public readonly float[,] heightMap;
        public readonly Color[] colorMap;
        public readonly int width;
        public readonly int height;

        public MapData(float[,] heightMap, Color[] colorMap) {
            this.heightMap = heightMap;
            this.colorMap = colorMap;
            this.width = heightMap.GetLength(0);
            this.height = heightMap.GetLength(1);
        }
    }

    [Serializable]
    public struct TerrainType {
        public string name;
        public float height;
        public Color color;
    }


    #endregion
    #region Fields&Consts

    [Header("Values")]
    [SerializeField] List<TerrainType> regions;
    [SerializeField] PerlinNoiseMapGeneratorSettings settings; // Set in editor

    public static TerrainGenerator Instance { get; private set; }

    //Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new();
    //Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new();

    public const int MAP_CHUNK_SIZE_PLUSONE = 241; // 240 divisible by 1,2,4,8,6,12

    public bool autoUpdate;

    [Header("Constants")]
    private const float SMALL_NUMBER = 0.001f;
    private const int ONE = 1;

    #endregion

    private void Awake() {
        Instance = this;
    }
    public enum DrawMode {
        NoiseMap,
        ColorMap,
        Mesh
    }

    public DrawMode drawMode;
    MapData GenerateMapData() {
        float[,] noiseMap = PerlinNoiseMapGenerator.GenerateNoiseMap(settings);
        Color[] colorMap = GenerateColorMapFromNoiseMap(noiseMap);
        return new MapData(noiseMap, colorMap);
    }
    public void DrawMapInEditor() {
        MapData mapData = GenerateMapData();
        MapDisplay display = GetComponent<MapDisplay>();

        if (drawMode == DrawMode.NoiseMap) {
            display.DrawTexture(TextureGenerator.GetTextureFromHeightMap(mapData.heightMap));
        } else if (drawMode == DrawMode.ColorMap) {
            display.DrawTexture(TextureGenerator.GetTextureFromColorMap(mapData.colorMap, mapData.width, mapData.height));
        } else if (drawMode == DrawMode.Mesh) {
            MeshData meshData = MeshGenerator.GenerateTerrainMeshDataFromHeightMap(mapData.heightMap, settings.amplitudeMultipler, settings.amplitudeEnvelope, settings.previewLOD);
            Texture2D texture = TextureGenerator.GetTextureFromColorMap(mapData.colorMap, settings.width, settings.height);
            display.DrawMesh(meshData, texture);
        }
    }

    
    //wtf
    //public void RequestMapData(Action<MapData> callback) {
    //    ThreadStart threadStart = delegate {
    //        MapDataThread(callback);
    //    };

    //    new Thread(threadStart).Start();
    //}

    //public void RequestMeshData(MapData mapData, int levelOfDetail, Action<MeshData> callback) {
    //    ThreadStart threadStart = delegate {
    //        MeshDataThread(mapData, levelOfDetail, callback);
    //    };

    //    new Thread(threadStart).Start();
    //}

    //void MeshDataThread(MapData mapData, int levelOfDetail, Action<MeshData> callback) {
    //    MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.heightMap, parameters.meshHeightMultipler, parameters.meshHeightCurve, levelOfDetail);
    //    lock(meshDataThreadInfoQueue) {
    //        meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));
    //    }
    //}

    //void MapDataThread(Action<MapData> callback) {
    //    // Not running on the main thread
    //    MapData mapData = GenerateMapData(); // executed inside this thread
    //    lock (mapDataThreadInfoQueue) {
    //        mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
    //    }
    //}

    //private void Update() {
    //    if (mapDataThreadInfoQueue.Count > 0) {
    //        for (int i = 0; i < mapDataThreadInfoQueue.Count; i++) {
    //            MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
    //            threadInfo.callback(threadInfo.parameter);
    //        }
    //    }

    //    if (meshDataThreadInfoQueue.Count > 0) {
    //        for (int i = 0; i < meshDataThreadInfoQueue.Count; i++) {
    //            MapThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
    //            threadInfo.callback(threadInfo.parameter);
    //        }
    //    }

    //}

    //struct MapThreadInfo<T> {
    //    public readonly Action<T> callback;
    //    public readonly T parameter;

    //    public MapThreadInfo(Action<T> callback, T parameter) {
    //        this.callback = callback;
    //        this.parameter = parameter;
    //    }
    //}

    private Color[] GenerateColorMapFromNoiseMap(float[,] noiseMap) {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        Color[] colorMap = new Color[width * height];
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Count; i++) {
                    if (currentHeight <= regions[i].height) {
                        colorMap[y * width + x] = regions[i].color;
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
        if (settings.amplitudeMultipler <= 0) settings.amplitudeMultipler = SMALL_NUMBER;
        if (settings.persistance <= 0) settings.persistance = SMALL_NUMBER;
        if (settings.persistance > 1) settings.persistance = ONE;
        if (settings.lacunarity <= 0) settings.lacunarity = SMALL_NUMBER;
        if (settings.octaves <= 0) settings.octaves = ONE;
    }



}
