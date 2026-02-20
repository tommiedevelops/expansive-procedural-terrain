using System;
using System.Collections.Generic;
using UnityEngine;


namespace TerrainGeneratorAsset {
        
    public class TerrainGeneratorMB : MonoBehaviour
    {
        // Warning: If you change the variable names, you will need to update the relevant FindProperty() arguments
        // in TerrainGeneratorMBEditor.cs

        [SerializeField] private Transform m_terrainViewer;
        [SerializeField] private TerrainConfigSO m_terrainConfig;
        [SerializeField] private NoiseGeneratorSO m_noiseGenerator;

        private List<Chunk> activeChunks = new();
        private void Awake()
        {
            if (!m_terrainViewer || !m_terrainConfig || !m_noiseGenerator)
                Debug.Assert(false, "Terrain Viewer, Terrain Config and Noise Generator are required. Exiting...");

        }
        private void Start()
        {
        }
        private void Update()
        {
        }
        public NoiseGeneratorSO GetNoiseGeneratorSO()
        {
            return m_noiseGenerator;
        }
        public TerrainConfigSO GetTerrainConfigSO()
        {
            return m_terrainConfig;
        }

     }
}