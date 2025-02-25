using Unity.VisualScripting;
using UnityEngine;

public class NoiseMapGenerator {

    /* This class is for outputting Perlin noise in the form of a 2D float array */
    /* FUTURE: Generalise to 3D. Add other forms of noise. */

    #region Declarations
    public enum NoiseType {
        Perlin
    }

    #endregion
    #region Fields
    [Header("Variables")]
    private float[,] noiseMap;
    int width;
    int height;
    int seed = 0;
    int octaves;
    float persistance;
    float lacunarity;
    float amplitudeMultipler;
    int previewLOD = 0; // LOD is short for 'Level of Detail'
    Vector2 offsetV2; // For offsetting the noise
    AnimationCurve amplitudeEnvelope;
    NoiseType noiseType;

    [Header("Constants")]
    private const int MAP_CHUNK_SIZE = 240; // Default value for width and height
    private const int INITIAL_FREQUENCY = 1;
    private const int INITIAL_AMPLTIUDE = 1;
    private const int INITIAL_NOISE_HEIGHT = 1;
    #endregion

    // Constructor
    public NoiseMapGenerator(NoiseMapGeneratorSettings settings = null) {
        bool settingsProvided = (settings == null);

        this.width = settingsProvided ? settings.width : MAP_CHUNK_SIZE;
        this.height = settingsProvided ? settings.height : MAP_CHUNK_SIZE;
        this.seed = settingsProvided ? settings.seed : 0;
        this.octaves = settingsProvided ? settings.octaves : 0;
        this.persistance = settingsProvided ? settings.persistance : 0.5f;
        this.lacunarity = settingsProvided ? settings.lacunarity : 1f;
        this.amplitudeMultipler = settingsProvided ? settings.amplitudeMultipler : 1f ;
        this.amplitudeEnvelope = settingsProvided ? settings.amplitudeEnvelope : new AnimationCurve();
        this.previewLOD = settingsProvided ? settings.previewLOD : 0;
        this.offsetV2 = settingsProvided ? settings.offsetV2 : Vector2.zero;
        this.noiseType = settingsProvided ? settings.type : NoiseType.Perlin;
    }

    // FOR 2D PERLIN NOISE
    public float[,] GeneratePerlinNoiseMap(NoiseMapGeneratorSettings settings) {
        /* Algorithm for computing the Perlin Noise Map */

        Vector2[] octaveOffsets = GenerateOctaveOffsets();
        float[,] noiseMap = new float[width, height];

        // Variables for state
        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;
        float halfWidth = width / 2;
        float halfHeight = height / 2;

        // Loop through each pixel
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {

                float amplitude = INITIAL_AMPLTIUDE;
                float frequency = INITIAL_FREQUENCY;
                float noiseHeight = INITIAL_NOISE_HEIGHT;

                // loop through octaves
                for (int i = 0; i < octaves; i++) {

                    float sampleX = ((x - halfWidth) / amplitudeMultipler + octaveOffsets[i].x) * frequency;
                    float sampleY = ((y - halfHeight) / amplitudeMultipler - octaveOffsets[i].y) * frequency;

                    // Shift and scale to be in range [-1,1]
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

                    noiseHeight += perlinValue * amplitude;
                    amplitude *= persistance;
                    frequency *= lacunarity;

                    noiseMap[x, y] = noiseHeight;
                }

                // Update state variables 
                if (noiseHeight > maxNoiseHeight) maxNoiseHeight = noiseHeight;
                else if (noiseHeight < minNoiseHeight) minNoiseHeight = noiseHeight;
            }
        }

        RenormalizeNoiseMap(maxNoiseHeight, minNoiseHeight);
        return noiseMap;

    }
    private void RenormalizeNoiseMap(float maxNoiseHeight, float minNoiseHeight) {
        /* 2D Perlin Noise Helper Function
           Re-normalizes noise map to be between 0 and 1.
        */
        for (int y = 0; y < height; y++) 
            for (int x = 0; x < width; x++) 
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);         
    }
    private Vector2[] GenerateOctaveOffsets() {
        /* Generates offsets for each octave based on the provided offset value */
        const int MIN = -100000;
        const int MAX = 100000;
        var prng = new System.Random(seed);
        var octaveOffsets = new Vector2[octaves];

        for (int i = 0; i < octaves; i++) {
            float offsetX = prng.Next(MIN, MAX) + offsetV2.x;
            float offsetY = prng.Next(MIN, MAX) + offsetV2.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        return octaveOffsets;
    }

}
