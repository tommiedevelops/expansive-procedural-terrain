using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.NoiseSystem.ScriptableObjects;
using NUnit.Framework.Constraints;
using UnityEngine;

namespace _Scripts.NoiseSystem
{
    // responsible for adding noise layers together and producing 
    // a final height map
    public class NoiseGenerator
    {
        private readonly List<NoiseLayerSO> _noiseLayers = new();
        private int _gridWidth = 0;
        private int _gridHeight = 0;
        private Vector3 _worldSpaceOrigin = new(0, 0, 0);
        public void ApplyNoise(Mesh mesh, Vector2 offset, float multiplier)
        {
            var newVertices = new Vector3[mesh.vertexCount];
            var i = 0;
            for(var y = 0; y < _gridWidth; y++)
            for (var x = 0; x < _gridHeight; x++)
            {
                var yValAdjustment = new Vector3(0f, SampleNoise(offset.x + x, offset.y + y), 0f);
                newVertices[i] = mesh.vertices[i] + multiplier * yValAdjustment;
                i++;
            }
            RenormalizeMeshVertices(newVertices);  
            mesh.vertices = newVertices;
            mesh.RecalculateNormals();
              
        }
        private float SampleNoise(float x, float y)
        {
            var unnormalizedSum = _noiseLayers.Sum(layer => layer.Evaluate(new Vector2(x,y)));
            return unnormalizedSum;
        }
        private static void RenormalizeMeshVertices(Vector3[] meshVertices)
        {

            // 1. Find min and max Y
            float minY = float.MaxValue;
            float maxY = float.MinValue;

            foreach (var v in meshVertices)
            {
                if (v.y < minY) minY = v.y;
                if (v.y > maxY) maxY = v.y;
            }

            float range = maxY - minY;
            if (range == 0f) return; // flat mesh, nothing to normalize

            // 2. Normalize Y values to [-1 1]
            for (int i = 0; i < meshVertices.Length; i++)
            {
                float normalizedY = 2f * (meshVertices[i].y - minY) / range - 1f;
                meshVertices[i].y = normalizedY;
            }

        }

        public void AddLayer(NoiseLayerSO layer)
        {
            _noiseLayers.Add(layer);
        }

        public void RemoveLayer<TLayerType>() where TLayerType : NoiseLayerSO
        {
            _noiseLayers.RemoveAll(layer => layer is TLayerType);
        }

        #region Setters & Getters
        public void SetGridWidth(int gridWidth)
        {
            if(gridWidth < 0) throw new ArgumentException("gridWidth must be greater than or equal to 0");
            _gridWidth = gridWidth;
        }

        public void SetGridHeight(int gridHeight)
        {
            if(gridHeight < 0) throw new ArgumentException("gridHeight must be greater than or equal to 0");
            _gridHeight = gridHeight;
        }

        public int GetGridWidth()
        {
            return _gridWidth;
        }

        public int GetGridHeight()
        {
            return _gridHeight;
        }
        
        public void SetGridDimensions(int gridWidth, int gridHeight)
        {
            SetGridWidth(gridWidth);
            SetGridHeight(gridHeight);
        }
        public List<NoiseLayerSO> GetLayers() { return _noiseLayers; }
        
        public Vector3 GetWorldSpaceOrigin() { return _worldSpaceOrigin; }
        public void SetWorldSpaceOrigin(Vector3 origin) { _worldSpaceOrigin = origin; }

        
        #endregion
    }
}
