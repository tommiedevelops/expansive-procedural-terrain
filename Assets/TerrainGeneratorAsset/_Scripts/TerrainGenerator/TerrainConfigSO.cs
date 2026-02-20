using System;
using UnityEngine;

namespace TerrainGeneratorAsset 
{
    [CreateAssetMenu(menuName = "TerrainGenerator/TerrainConfig")]
    [Serializable]
    public class TerrainConfigSO : ScriptableObject 
    {
        public bool isInfinite;
        public Material terrainMaterial;
        public Vector3 terrainDimensions = Vector3.one;
        public int resolutionX = 1;
        public int resolutionZ = 1;
    }
}