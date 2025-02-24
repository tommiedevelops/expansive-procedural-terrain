using NUnit.Framework;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Threading;

public class MapGenerator : MonoBehaviour {
    #region Fields&Consts
    [Header("Values")]
    [SerializeField] GenerateNoiseMapParams parameters;
    [SerializeField] List<TerrainType> regions;

    public static MapGenerator Instance { get; private set; }

    Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new();
    Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new();

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
        float[,] noiseMap = Noise.GenerateNoiseMap(parameters);
        Color[] colorMap = GenerateColorMap(noiseMap);
        return new MapData(noiseMap, colorMap);
    }

    public void DrawMapInEditor() {
        MapData mapData = GenerateMapData();
        MapDisplay display = GetComponent<MapDisplay>();

        if (drawMode == DrawMode.NoiseMap) {
            display.DrawTexture(TextureGenerator.GetTextureFromHeightMap(mapData.heightMap));
        } else if (drawMode == DrawMode.ColorMap) {
            display.DrawTexture(TextureGenerator.GetTextureFromColorMap(mapData.colorMap, parameters.mapChunkSize, parameters.mapChunkSize));
        } else if (drawMode == DrawMode.Mesh) {
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(mapData.heightMap, parameters.meshHeightMultipler, parameters.meshHeightCurve, parameters.levelOfDetail),
                TextureGenerator.GetTextureFromColorMap(mapData.colorMap, parameters.mapChunkSize, parameters.mapChunkSize)
            );
        }
    }

    //wtf
    public void RequestMapData(Action<MapData> callback) {
        ThreadStart threadStart = delegate {
            MapDataThread(callback);
        };

        new Thread(threadStart).Start();
    }

    public void RequestMeshData(MapData mapData, Action<MeshData> callback) {
        ThreadStart threadStart = delegate {
            MeshDataThread(mapData, callback);
        };

        new Thread(threadStart).Start();
    }

    void MeshDataThread(MapData mapData, Action<MeshData> callback) {
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.heightMap, parameters.meshHeightMultipler, parameters.meshHeightCurve, parameters.levelOfDetail);
        lock(meshDataThreadInfoQueue) {
            meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));
        }
    }

    void MapDataThread(Action<MapData> callback) {
        // Not running on the main thread
        MapData mapData = GenerateMapData(); // executed inside this thread
        lock (mapDataThreadInfoQueue) {
            mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
        }
    }

    private void Update() {
        if (mapDataThreadInfoQueue.Count > 0) {
            for (int i = 0; i < mapDataThreadInfoQueue.Count; i++) {
                MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }

        if (meshDataThreadInfoQueue.Count > 0) {
            for (int i = 0; i < meshDataThreadInfoQueue.Count; i++) {
                MapThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }

    }

    struct MapThreadInfo<T> {
        public readonly Action<T> callback;
        public readonly T parameter;

        public MapThreadInfo(Action<T> callback, T parameter) {
            this.callback = callback;
            this.parameter = parameter;
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
public readonly struct MapData {
    public readonly float[,] heightMap;
    public readonly Color[] colorMap;

    public MapData(float[,] heightMap, Color[] colorMap) {
        this.heightMap = heightMap;
        this.colorMap = colorMap;
    }
}