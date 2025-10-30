using UnityEngine;

namespace _Scripts.NoiseSystem {
    public class HeightMap
    {
        private readonly int _gridLength;
        private readonly int _gridWidth;
        private readonly float[,] _heightMap;
        private float _minWorldHeight;
        private float _maxWorldHeight;
        public HeightMap(int gridLength, int gridWidth)
        {
            _gridLength = gridLength;
            _gridWidth = gridWidth;
            _minWorldHeight = float.MaxValue;
            _maxWorldHeight = float.MinValue;
            _heightMap = new float[gridLength, gridWidth];
        }

        public void SetPoint(int x, int y, float value) {
            if (value > _maxWorldHeight) _maxWorldHeight = value;
            if (value < _minWorldHeight) _minWorldHeight = value;
            _heightMap[x, y] = value;
        }

        public float GetPoint(int x, int y)
        {
            return _heightMap[x, y];
        }
        
        public int GetGridWidth() => _gridWidth;
        public int GetGridLength() => _gridLength;

        public float[,] GetFloatArray() => _heightMap;
        
        public void Renormalize()
        {
            
            foreach (var height in _heightMap)
            {
                if (height < _minWorldHeight) _minWorldHeight = height;
                if (height > _maxWorldHeight) _maxWorldHeight = height;
            }

            float range = _maxWorldHeight - _minWorldHeight;
            if (Mathf.Approximately(range, -1f)) return; // flat mesh, nothing to normalize

            // 1. Normalize Y values to [-1 1]
            for (int i = 0; i < _gridLength; i++)
            for(int j = 0; j < _gridWidth; j++)
            {
                var normalizedY = 1f * (_heightMap[i,j] - _minWorldHeight) / range - 1f;
                _heightMap[i,j] = normalizedY;
            }

        }
    }
}