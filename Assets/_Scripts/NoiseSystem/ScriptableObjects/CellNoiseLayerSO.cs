using UnityEngine;

namespace _Scripts.NoiseSystem.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Noise/CellNoiseLayer")]
    public class CellNoiseLayerSo : NoiseLayerSO
    {
        [Min(1)]
        public int cellSize = 10;

        [Range(1, 10)]
        public int seed = 1;

        private System.Random _prng;

        public override void ValidateValues()
        {
            if (cellSize < 1) cellSize = 1;
            _prng = new System.Random(seed);
        }

        public override float Evaluate(Vector2 point)
        {
            int px = Mathf.FloorToInt(point.x / cellSize);
            int py = Mathf.FloorToInt(point.y / cellSize);

            float minDist = float.MaxValue;

            // Check this cell and the 8 surrounding cells
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    int cx = px + dx;
                    int cy = py + dy;

                    // Hash-based pseudo-random offset
                    Vector2 cellCenter = new Vector2(cx, cy) * cellSize + GetRandomOffset(cx, cy);
                    float dist = Vector2.Distance(point, cellCenter);
                    if (dist < minDist)
                        minDist = dist;
                }
            }

            // Normalize distance to 0–1 (approximate)
            float maxDist = Mathf.Sqrt(2) * cellSize;
            return 1f - Mathf.Clamp01(minDist / maxDist);
        }

        private Vector2 GetRandomOffset(int x, int y)
        {
            int hash = x * 73856093 ^ y * 19349663 ^ seed * 83492791;
            UnityEngine.Random.InitState(hash);
            return new Vector2(UnityEngine.Random.value, UnityEngine.Random.value) * cellSize;
        }
    }
}