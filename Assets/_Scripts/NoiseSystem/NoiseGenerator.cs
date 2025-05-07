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
        
        List<NoiseLayer>  _noiseLayers = new();

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

        public void ApplyNoise(float[,] array)
        {
            for(var x = 0; x < _gridWidth; x++)
            for (var y = 0; y < _gridHeight; y++)
                array[x, y] += SampleNoise(x, y);
        }

        public void ApplyNoise(Mesh mesh)
        {
            var newVertices = new Vector3[mesh.vertexCount];
            var i = 0;
            for(var y = 0; y < _gridWidth; y++)
            for (var x = 0; x < _gridHeight; x++)
            {
                
                newVertices[i] = mesh.vertices[i] + new Vector3(0f, SampleNoise(x, y), 0f);
                i++;
            }
            
            mesh.vertices = newVertices;
                
        }

        private float SampleNoise(int x, int y)
        {
            var numLayers = _noiseLayers.Count;
            var sum = _noiseLayers.Sum(layer => layer.Evaluate(x, y));
            return sum / numLayers; // ensure its normalised
        }

        public void AddLayer(NoiseLayer layer)
        {
            _noiseLayers.Add(layer);
        }

        public void SetGridDimensions(int gridWidth, int gridHeight)
        {
            SetGridWidth(gridWidth);
            SetGridHeight(gridHeight);
        }
    }
}
