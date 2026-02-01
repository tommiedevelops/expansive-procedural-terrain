using System;
using System.Collections.Generic;
using UnityEngine;

namespace TerrainGeneratorAsset
{

    [CreateAssetMenu(menuName = "ScriptableObjects/NoiseGenerator")]
    [Serializable]
    public class NoiseGeneratorSO : ScriptableObject
    {
        [SerializeField] private List<NoiseLayerSO> _noiseLayers;
        private void OnEnable() {
            _noiseLayers = new List<NoiseLayerSO>();    
        }
        public Func<float, float, float> GetHeightFunc() {
            return SampleNoise;
        }
        private float SampleNoise(float x, float y)
        {
            float result = 0.0f;
            foreach (var layer in _noiseLayers)
            {
                if (layer == null) continue;
                result += layer.Evaluate(x,y);
            }
            // Temporary
            return result;
        }
        public void AddLayer(NoiseLayerSO layer)
        {
            _noiseLayers.Add(layer);
        }
        public void RemoveLayer<TLayerType>() where TLayerType : NoiseLayerSO
        {
            _noiseLayers.RemoveAll(layer => layer is TLayerType);
        }
        public List<NoiseLayerSO> GetLayers() { return _noiseLayers; }

    }
}
