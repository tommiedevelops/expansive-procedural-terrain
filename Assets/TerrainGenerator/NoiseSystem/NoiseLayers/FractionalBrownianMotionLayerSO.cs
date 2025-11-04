using UnityEngine;

namespace TerrainGenerator.NoiseLayers
{
    [CreateAssetMenu(menuName = "Noise/FBmNoiseSettings")]
    public class FBmNoiseLayerSo : NoiseLayerSO
    {
        [Range(1, 10)]
        public int octaves = 4;

        [Min(0.0001f)]
        public float frequency = 1f;

        [Range(0f, 1f)]
        public float persistence = 0.5f;

        [Min(1f)]
        public float lacunarity = 2f;

        public Vector2 offset;

        [Tooltip("The base noise to sample from (e.g., Perlin)")]
        public NoiseLayerSO baseNoise;

        public override void ValidateValues()
        {
            if (octaves < 1) octaves = 1;
            if (frequency <= 0f) frequency = 0.001f;
            if (lacunarity < 1f) lacunarity = 1f;
            if (baseNoise != null) baseNoise.ValidateValues();
        }

        public override float Evaluate(Vector2 point)
        {
            if (baseNoise == null)
                return 0f;

            float total = 0f;
            float amplitude = 1f;
            float freq = frequency;
            float maxAmplitude = 0f;

            for (int i = 0; i < octaves; i++)
            {
                Vector2 samplePoint = (point + offset) * freq;
                float value = baseNoise.Evaluate(samplePoint);

                total += value * amplitude;
                maxAmplitude += amplitude;

                amplitude *= persistence;
                freq *= lacunarity;
            }

            return total / maxAmplitude;
        }

        public override float Compose(float currentValue, Vector2 point)
        {
            throw new System.NotImplementedException();
        }
    }
}