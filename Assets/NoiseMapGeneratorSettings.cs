using System;
using UnityEngine;
using static NoiseMapGenerator;

[Serializable]
public class NoiseMapGeneratorSettings {
    /* This class needs to be instantiated and configured before creating a NoiseGeneratorObject*/
    public int width;
    public int height;
    public int seed;
    public int octaves;
    public float persistance;
    public float lacunarity = 1f;
    public float amplitudeMultipler;
    public AnimationCurve amplitudeEnvelope;
    public int previewLOD;
    public Vector2 offsetV2;
    public NoiseType type;
}
