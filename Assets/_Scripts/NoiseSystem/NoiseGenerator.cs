using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.NoiseSystem.ScriptableObjects;
using NUnit.Framework.Constraints;
using UnityEngine;

namespace _Scripts.NoiseSystem
{
    public class NoiseGenerator : MonoBehaviour
    {
        [SerializeField] private List<NoiseLayerSO> additiveNoiseLayers;
        [SerializeField] private List<NoiseLayerSO> multiplicativeNoiseLayers;
        [SerializeField] private List<NoiseLayerSO> compositionalNoiseLayers;
        
        private readonly List<NoiseLayerSO> _noiseLayers = new();
        public HeightMap GenerateNoiseMap(Vector2 botLeftPointInGlobalCoords, float distanceBetweenPoints, float heightMultiplier, int gridWidth, int gridHeight)
        {
            var heightMap = new HeightMap(gridHeight, gridWidth);
            
            for(var y = 0; y < gridWidth; y++)
            for (var x = 0; x < gridHeight; x++)
                heightMap.SetPoint(x, y, SampleNoise(botLeftPointInGlobalCoords.x + x * distanceBetweenPoints, botLeftPointInGlobalCoords.y + y * distanceBetweenPoints));

            heightMap.Renormalize();
            
            for(var y = 0; y < gridWidth; y++)
            for (var x = 0; x < gridHeight; x++)
                heightMap.SetPoint(x, y, heightMultiplier*heightMap.GetPoint(x,y));
            
            return heightMap;
        }
        private float SampleNoise(float x, float y)
        {
            var result = additiveNoiseLayers.Sum(layer => layer.Evaluate(new Vector2(x,y)));

            foreach (var layer in compositionalNoiseLayers)
            {
                result = layer.Compose(result, new Vector2(x, y));
            }

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
