using System;
using System.Collections.Generic;
using UnityEngine;

namespace TerrainGeneratorAsset
{

    [CreateAssetMenu(menuName = "TerrainGenerator/NoiseGenerator")]
    [Serializable]
    public class NoiseGeneratorSO : ScriptableObject
    {
        [SerializeField] private List<NoiseLayerSO> _noiseLayers;
        private NoiseCombinerSO _noiseCombiner;
        private void OnEnable() {
            // only initialize if its null
            _noiseLayers ??= new List<NoiseLayerSO>();    
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
        public List<NoiseLayerSO> GetNoiseLayers() { return _noiseLayers; }

    }
}
