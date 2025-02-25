using System;
using UnityEngine;

public static class PerlinNoiseMapGenerator {

    /* This class is for outputting Perlin noise in the form of a normalized 2D float array */
    /* FUTURE: Generalise to 3D. Add other forms of noise. */
    #region Fields
    [Header("Constants")]
    private const int MAP_CHUNK_SIZE = 240; // Default value for width and height
    private const int INITIAL_FREQUENCY = 1;
    private const int INITIAL_AMPLTIUDE = 1;
    private const int INITIAL_NOISE_HEIGHT = 1;
    #endregion
    public static float[,] GenerateNoiseMap(NoiseSettings settings) {
        /* Algorithm for computing the Perlin Noise Map */

        Vector2[] octaveOffsets = GenerateOctaveOffsets(settings);
        float[,] noiseMap = new float[settings.width, settings.length];

        // Variables for state
        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;
        float halfWidth = settings.width / 2;
        float halfHeight = settings.length / 2;

        // Loop through each pixel
        for (int y = 0; y < settings.length; y++) {
            for (int x = 0; x < settings.width; x++) {

                float amplitude = INITIAL_AMPLTIUDE;
                float frequency = INITIAL_FREQUENCY;
                float noiseHeight = INITIAL_NOISE_HEIGHT;

                // loop through octaves
                for (int i = 0; i < settings.octaves; i++) {

                    float sampleX = ((x - halfWidth) / settings.noiseScale + octaveOffsets[i].x) * frequency;
                    float sampleY = ((y - halfHeight) / settings.noiseScale - octaveOffsets[i].y) * frequency;

                    // Shift and scale to be in range [-1,1]
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

                    noiseHeight += perlinValue * amplitude;
                    amplitude *= settings.persistance;
                    frequency *= settings.lacunarity;

                    noiseMap[x, y] = noiseHeight;
                }

                // Update state variables 
                if (noiseHeight > maxNoiseHeight) maxNoiseHeight = noiseHeight;
                else if (noiseHeight < minNoiseHeight) minNoiseHeight = noiseHeight;
            }
        }

        RenormalizeNoiseMap(noiseMap,maxNoiseHeight, minNoiseHeight);
        return noiseMap;

    }
    private static void RenormalizeNoiseMap(float[,] noiseMap, float maxNoiseHeight, float minNoiseHeight) {
        /* 2D Perlin Noise Helper Function
           Re-normalizes noise map to be between 0 and 1.
        */

        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);
        for (int y = 0; y < height; y++) 
            for (int x = 0; x < width; x++) 
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);         
    }
    private static Vector2[] GenerateOctaveOffsets(NoiseSettings settings) {
        /* Generates offsets for each octave based on the provided offset value */
        const int MIN = -100000;
        const int MAX = 100000;
        var prng = new System.Random(settings.seed);
        var octaveOffsets = new Vector2[settings.octaves];

        for (int i = 0; i < settings.octaves; i++) {
            float offsetX = prng.Next(MIN, MAX) + settings.offsetV2.x;
            float offsetY = prng.Next(MIN, MAX) + settings.offsetV2.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        return octaveOffsets;
    }

}

[Serializable]
public class NoiseSettings {
    /* An instance of this class can be used to configure the static NoiseMapGenerator */
    public int width;
    public int length;
    public int seed;
    public int octaves;
    public float persistance;
    public float lacunarity = 1f;
    public float noiseScale;
    public AnimationCurve amplitudeEnvelope;
    public int previewLOD;
    public Vector2 offsetV2;
    public float heightMultiplier;
}