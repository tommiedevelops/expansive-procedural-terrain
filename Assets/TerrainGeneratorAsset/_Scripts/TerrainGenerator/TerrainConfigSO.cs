
using System;
using UnityEngine;


namespace TerrainGenerator {
    [CreateAssetMenu(menuName = "ScriptableObjects/TerrainConfig")]
    [Serializable]
    public class TerrainConfigSO : ScriptableObject {
        [SerializeField] private bool isInfinite; 
        [SerializeField] private Material terrainMaterial;
        [SerializeField] private Vector3 terrainDimensions = Vector3.one;
        [SerializeField] private int resolutionX = 1;
        [SerializeField] private int resolutionY = 1;
    }
}