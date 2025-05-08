using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Constraints;
using UnityEngine;

namespace _Scripts.NoiseSystem
{
    public class NoiseGenerator
    {
        private int _gridWidth = 0;
        private int _gridHeight = 0;
        private Vector3 _worldSpaceOrigin = new(0, 0, 0);
        
        public Vector3 GetWorldSpaceOrigin() { return _worldSpaceOrigin; }
        public void SetWorldSpaceOrigin(Vector3 origin) { _worldSpaceOrigin = origin; }

        private readonly List<NoiseLayer> _noiseLayers = new();

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

        public void ApplyNoise(float[,] array, Vector2 offset)
        {
            for(var x = 0; x < _gridWidth; x++)
            for (var y = 0; y < _gridHeight; y++)
                array[x, y] += SampleNoise(offset.x + x, offset.y + y);
            
            RenormalizeFloatArray(array);
        }

        public void ApplyNoise(Mesh mesh, Vector2 offset, float multiplier)
        {
            var newVertices = new Vector3[mesh.vertexCount];
            var i = 0;
            for(var y = 0; y < _gridWidth; y++)
            for (var x = 0; x < _gridHeight; x++)
            {
                var yValAdjustment = new Vector3(0f, 2*SampleNoise(offset.x + x, offset.y + y)-1, 0f);
                newVertices[i] = mesh.vertices[i] + multiplier * yValAdjustment;
                i++;
            }
            RenormalizeMeshVertices(newVertices);  
            mesh.vertices = newVertices;
            mesh.RecalculateNormals();
              
        }

        public float SampleNoise(float x, float y)
        {
            var numLayers = _noiseLayers.Count;
            var normalizedSum = _noiseLayers.Sum(layer => layer.Evaluate(x, y));
            return normalizedSum;
        }

        public void AddLayer(NoiseLayer layer)
        {
            _noiseLayers.Add(layer);
        }

        public void RemoveLayer<TLayerType>() where TLayerType : NoiseLayer
        {
            _noiseLayers.RemoveAll(layer => layer is TLayerType);
        }
        
        public List<NoiseLayer> GetLayers() { return _noiseLayers; }
        public void SetGridDimensions(int gridWidth, int gridHeight)
        {
            SetGridWidth(gridWidth);
            SetGridHeight(gridHeight);
        }
        
        private static void RenormalizeFloatArray(float[,] array)
        {
            int width = array.GetLength(0);
            int height = array.GetLength(1);

            float min = float.MaxValue;
            float max = float.MinValue;

            // 1. Find min and max
            for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                float val = array[x, y];
                if (val < min) min = val;
                if (val > max) max = val;
            }

            float range = max - min;
            if (range == 0f) return; // All values are the same — nothing to normalize

            // 2. Renormalize
            for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                array[x, y] = (array[x, y] - min) / range;
            }
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

            // 2. Normalize Y values to [0, 1]
            for (int i = 0; i < meshVertices.Length; i++)
            {
                float normalizedY = (meshVertices[i].y - minY) / range;
                meshVertices[i].y = normalizedY;
            }
            
        }


    }
}
