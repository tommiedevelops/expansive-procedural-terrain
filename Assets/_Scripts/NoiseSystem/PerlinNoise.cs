using UnityEngine;

namespace _Scripts.NoiseSystem
{
    public class PerlinNoise : NoiseLayer {

        public static void ApplyPerlinNoise(Vector2 offset, ref Mesh mesh, float noiseScale, int octaves, int seed, float persistance, float lacunarity, Vector2 offsetV2, float heightMultiplier ) {
            /*
         * offset: the global offset in position
         * mesh: Mesh we are applying noise to  
         * noiseSettings: self explanatory
         */

            if (mesh == null) {
                Debug.LogError("No mesh found! Make sure a mesh exists before applying noise.");
                return;
            }
        

            // No noise required
            if (noiseScale == 0) return;
            Vector3[] vertices = mesh.vertices;
            Vector2[] octaveOffsets = GenerateOctaveOffsets(octaves, seed, offsetV2);

            float maxNoiseHeight = float.MinValue;
            float minNoiseHeight = float.MaxValue;
        
            for (int i = 0; i < vertices.Length; i++) {
                float x = vertices[i].x;
                float z = vertices[i].z;

                // Add Perlin noise to the existing Y position
                vertices[i].y += Mathf.PerlinNoise((x + offset.x) * noiseScale, (z - offset.y) * noiseScale) * 2 - 1;

                float amplitude = 1f;
                float frequency = 1f;
                float noiseHeight = 1f;

                // loop through octaves
                for (int j = 0; j < octaves; j++) {

                    float sampleX = ((x + offset.x) / noiseScale + octaveOffsets[j].x) * frequency;
                    float sampleY = ((z + offset.y) / noiseScale - octaveOffsets[j].y) * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

                    noiseHeight += perlinValue * amplitude;
                    amplitude *= persistance;
                    frequency *= lacunarity;

                    vertices[i].y = noiseHeight;
                }
                
                // Handle height multiplier
                vertices[i].y *= heightMultiplier;


                if (noiseHeight > maxNoiseHeight) maxNoiseHeight = noiseHeight;
                else if (noiseHeight < minNoiseHeight) minNoiseHeight = noiseHeight;
            }

            mesh.vertices = vertices;
            mesh.RecalculateNormals(); // Update normals for correct lighting
            RenormalizeHeightMap(mesh, maxNoiseHeight, minNoiseHeight);
        }

        // Helpers
        private static void RenormalizeHeightMap(Mesh mesh, float maxNoiseHeight, float minNoiseHeight) {
            /* 2D Perlin Noise Helper Function
           Re-normalizes noise map to be between 0 and 1.
        */
            for(int i = 0; i < mesh.vertices.Length; i++)
                mesh.vertices[i].y = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, mesh.vertices[i].y);
        }
        private static Vector2[] GenerateOctaveOffsets(int octaves, int seed, Vector2 offsetV2) {
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
        
        public override float Evaluate(float x, float y)
        {
            throw new System.NotImplementedException();
        }
    }
}