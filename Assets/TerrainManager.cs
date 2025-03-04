using System;
using UnityEditor;
using UnityEngine;

public class TerrainManager : MonoBehaviour {
    [Header("Plane Mesh Properties")]
    [SerializeField] private int width = 10;
    [SerializeField] private int length = 10;
    [SerializeField] private float scale = 1f;

    [Header("Perlin Noise Properties")]
    [SerializeField] private float noiseScale = 0.1f;  // Controls the frequency of the noise
    [SerializeField] private float heightMultiplier = 2f; // Controls the height variation
    [SerializeField] private float persistance = 0.5f;
    [SerializeField] private float lacunarity = 1f;
    [SerializeField] private int octaves = 1;
    [SerializeField] private int seed = 1;
    [SerializeField] private Vector2 offsetV2 = Vector2.zero;
    [SerializeField] private AnimationCurve heightCurve;

    private void OnValidate() {
        ValidateFields();
    }

    private void ValidateFields() {
        if (width < 0) width = 0;
        if (length < 0) length = 0;
        if (noiseScale < 0) noiseScale = 0;
        if (persistance < 0) persistance = 0.01f;
        if (persistance >= 1) persistance = 0.99f;
        if (octaves < 0) octaves = 0;
    }

    public void Generate() {
        PerlinNoise perlinNoise = GetComponent<PerlinNoise>();

        perlinNoise.SetNoiseScale(noiseScale);
        perlinNoise.SetHeightMultiplier(heightMultiplier);
        perlinNoise.SetLacunarity(lacunarity);
        perlinNoise.SetOctaves(octaves);
        perlinNoise.SetPersistance(persistance);
        perlinNoise.SetOffsetV2(offsetV2);
        perlinNoise.SetSeed(seed);
        perlinNoise.SetHeightCurve(heightCurve);
        perlinNoise.ApplyPerlinNoise();
    }
}

[CustomEditor(typeof(TerrainManager))]
public class TerrainManagerEditor : Editor {
    public override void OnInspectorGUI() {
        TerrainManager manager = (TerrainManager)target;

        // Draw default inspector properties
        bool changed = DrawDefaultInspector(); // Only detects changes in TerrainManager

        // Check if values in the child components change
        if (GUI.changed) {
            changed = true;
        }

        if (changed) {
            Undo.RecordObject(manager, "Modify Terrain Manager");
            EditorUtility.SetDirty(manager); // Mark the object as changed
            manager.Generate();
        }

        if (GUILayout.Button("Generate")) {
            Undo.RecordObject(manager, "Generate Terrain");
            EditorUtility.SetDirty(manager);
            manager.Generate();
        }
    }
}

