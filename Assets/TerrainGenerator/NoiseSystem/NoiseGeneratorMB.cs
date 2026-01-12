using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Constraints;
using UnityEngine;

using TerrainGenerator.NoiseLayers;
using Unity.VisualScripting;
namespace TerrainGenerator.NoiseSystem
{

    // TODO : re-design to allow for user to express mathematical layers and their compositions
    public class NoiseGenerator
    {
        private readonly List<NoiseLayerSO> _noiseLayers = new();

        public Func<float, float, float> GetHeightFunc() {
            return SampleNoise;
        }

        public HeightMap GenerateNoiseMap(Vector2 offset, float distanceBetweenPoints, float heightRange, int gridWidth, int gridHeight)
        {
            var heightMap = new HeightMap(gridHeight, gridWidth);
            
            for(var y = 0; y < gridWidth; y++)
            for (var x = 0; x < gridHeight; x++) {
                float height = heightRange * SampleNoise(offset.x + x * distanceBetweenPoints, offset.y + y * distanceBetweenPoints);
                heightMap.SetPoint(x, y, height);
            }
           
           return heightMap;
        }
        private float SampleNoise(float x, float y)
        {
            var result = _noiseLayers.Sum(layer => layer.Evaluate(new Vector2(x,y)));
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
