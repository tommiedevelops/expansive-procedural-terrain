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
