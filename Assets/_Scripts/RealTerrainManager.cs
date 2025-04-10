using UnityEngine;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using System;

public class RealTerrainManager : MonoBehaviour {
    public static RealTerrainManager Instance { get; private set; }

    // PRE-CALCULATED
    public const int MAX_NUM_VERTICES_PER_SIDE = 120;
    public static readonly int[] FACTORS_OF_MAX_NUM_VERTICES_PER_SIDE = { 1, 2, 3, 4, 6, 8, 10, 12 };

    /* For any Mesh generated, the number of vertices per side is required to be a factor of 
     * MAX_NUM_VERTICES_PER_SIDE
     */

    [SerializeField] QTViewer viewer;
    [SerializeField] NoiseSettings noiseSettings;

    QuadTree quadTree; // Generalise this to a collection in the future
    Dictionary<uint, QuadChunk> chunks = new();

    #region Unity Functions
    // Unity Functions
    private void Awake() {
        if (Instance == null) Instance = this;
        ValidateNoiseSettings(); 
    }
    private void Update() {
        viewer.UpdateViewTriangle();
    }
    private void OnValidate() {
        // I should really be configuring the noise separately then 
        // running it on here.
        ValidateNoiseSettings();
        //UpdateNoise();
    }
    #endregion

    #region Helper Functions
    // HELPERS
    private void ValidateNoiseSettings() {
        noiseSettings.width = MAX_NUM_VERTICES_PER_SIDE;
        noiseSettings.length = MAX_NUM_VERTICES_PER_SIDE;
        if (noiseSettings.persistance > 1) noiseSettings.persistance = 0.99f;
        if (noiseSettings.persistance < 0) noiseSettings.persistance = 0.01f;
        if (noiseSettings.octaves < 0) noiseSettings.octaves = 0;
        if (noiseSettings.octaves > 6) noiseSettings.octaves = 6;
        if (noiseSettings.lacunarity < 0) noiseSettings.lacunarity = 0.01f;
    }
    #endregion

    #region Getter and Setter Functions
    #endregion
}
