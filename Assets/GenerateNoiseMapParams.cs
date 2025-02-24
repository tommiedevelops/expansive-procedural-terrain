using System;
using UnityEngine;

[Serializable]
public class GenerateNoiseMapParams {
    public int mapChunkSize = MapGenerator.MAP_CHUNK_SIZE_PLUSONE;
    [Range(0,6)]
    public int editorPreviewLOD = 1;
    public float noiseScale = 1;
    public int seed = 0;
    public int octaves = 0;
    public float persistance = 0.5f;
    public float lacunarity = 1;
    public float meshHeightMultipler = 1;
    public Vector2 offset = Vector2.zero;
    public AnimationCurve meshHeightCurve;
}
