using UnityEngine;

namespace TerrainGenerator.NoiseLayers
{
    public abstract class NoiseLayerSO : ScriptableObject
    {
        public abstract void ValidateValues();
        public abstract float Evaluate(float x, float y);
        private void OnValidate()
        {
            ValidateValues();
        }
        public abstract float Compose(float currentValue, Vector2 point);
    }
}