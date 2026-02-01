using System.Collections.Generic;
using System.Linq;
using TerrainGenerator.ChunkingSystem;
using TerrainGenerator.NoiseSystem;
using TerrainGenerator.NoiseLayers;
using UnityEngine;
using System.Data;


namespace TerrainGeneratorAsset {
        
    public class TerrainGeneratorMB : MonoBehaviour
    {
        [SerializeField] private Transform terrainViewer;
        [SerializeField] private TerrainConfigSO terrainConfig;
        [SerializeField] private NoiseGeneratorSO noiseGen;

        private List<Chunk> activeChunks = new();
        private void Awake()
        {
            if (!terrainViewer || !terrainConfig || !noiseGen)
                Debug.Assert(false, "Terrain Viewer, Terrain Config and Noise Generator are required. Exiting...");
                
        }
        private void Start()
        {
        }
        private void Update()
        {
            
        }
     }
}