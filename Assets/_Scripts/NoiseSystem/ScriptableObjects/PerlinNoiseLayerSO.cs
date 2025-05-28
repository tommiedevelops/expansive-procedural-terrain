using _Scripts.NoiseSystem;
using _Scripts.NoiseSystem.ScriptableObjects;
using UnityEngine;

[CreateAssetMenu(menuName = "Noise/PerlinNoiseSettings")]
public class PerlinNoiseLayerSo : NoiseLayerSO
{
    public float scale = 10f;
    public int octaves = 4;
    public float persistence = 0.5f;
    public float lacunarity = 2.0f;
    public Vector2 offset;

    public override void ValidateValues()
    {
        if(lacunarity < 0f) lacunarity = 0f;
        if(octaves < 0) octaves = 0;
        if (octaves > 6) octaves = 6;
        if(persistence < 0f) persistence = 0f;
        if(persistence > 1f) persistence = 1f;
        if(scale < 0f) scale = 0f;
        
    }

    public override float Evaluate(Vector2 point)
    {
        float amplitude = 1;
        float frequency = 1;
        float noiseHeight = 0;

        for (int i = 0; i < octaves; i++)
        {
            float sampleX = (point.x + offset.x) / scale * frequency;
            float sampleY = (point.y + offset.y) / scale * frequency;

            float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
            noiseHeight += perlinValue * amplitude;

            amplitude *= persistence;
            frequency *= lacunarity;
        }

        return noiseHeight;
    }
}