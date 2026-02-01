using UnityEngine;

namespace TerrainGeneratorAsset
{
    public abstract class NoiseLayerSO : ScriptableObject
    {
        public abstract void ValidateValues();
        public abstract float Evaluate(float x, float y);
        private void OnValidate()
        {
            ValidateValues();
        }
    }
}