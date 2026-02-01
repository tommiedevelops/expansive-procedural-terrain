using UnityEngine;

namespace TerrainGenerator.NoiseSystem {
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
    
    }
}