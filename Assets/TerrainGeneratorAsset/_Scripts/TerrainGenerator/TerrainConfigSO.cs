
using System;
using UnityEngine;


namespace TerrainGenerator {
    [CreateAssetMenu(menuName = "ScriptableObjects/TerrainConfig")]
    [Serializable]
    public class TerrainConfigSO : ScriptableObject {
        [SerializeField] public Material terrainMaterial;
        [SerializeField] public Vector3 terrainDimensions = Vector3.one;
        [SerializeField] public int resolutionX = 1;
        [SerializeField] public int resolutionY = 1;
    }
}