using UnityEngine;


public class MapGenerator : MonoBehaviour {
    #region Fields&Consts
    [Header("Values")]
    [SerializeField] GenerateNoiseMapParams parameters;
    
    public bool autoUpdate;

    [Header("Constants")]
    private const float SMALL_NUMBER = 0.001f;
    private const int ONE = 1;

    
    #endregion
    public void GenerateMap() {
        float[,] noiseMap = Noise.GenerateNoiseMap(parameters);
        MapDisplay display = GetComponent<MapDisplay>();
        display.DrawNoiseMap(noiseMap);
    }

    void OnValidate() {
        ValidateFields();
    }

    void ValidateFields() {
        if (parameters.mapWidth < 1) parameters.mapWidth = ONE;
        if (parameters.mapHeight < 1) parameters.mapHeight = ONE;
        if (parameters.noiseScale <= 0) parameters.noiseScale = SMALL_NUMBER;
        if (parameters.persistance <= 0) parameters.persistance = SMALL_NUMBER;
        if (parameters.persistance > 1) parameters.persistance = ONE;
        if (parameters.lacunarity <= 0) parameters.lacunarity = SMALL_NUMBER;
        if (parameters.octaves <= 0) parameters.octaves = ONE;
    }

}
