using UnityEngine;

namespace _Scripts.NoiseSystem.ScriptableObjects
{
    public abstract class NoiseLayerSO : ScriptableObject
    {
        public abstract void ValidateValues();
        public abstract float Evaluate(Vector2 point);

        private void OnValidate()
        {
            ValidateValues();
        }
    }
}