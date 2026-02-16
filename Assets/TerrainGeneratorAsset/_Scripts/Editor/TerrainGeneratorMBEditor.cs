
using UnityEditor;

namespace TerrainGeneratorAsset
{

    // Main Partial Class for TerrainGeneratorMBEditor that containsd data fields and common functions

    [CustomEditor(typeof(TerrainGeneratorMB))]
    public partial class TerrainGeneratorMBEditor : Editor
    {
        private enum PreviewType { Wireframe, Mesh };
        private PreviewType m_previewType; 

        private TerrainConfigSOEditor m_terrainConfigSOEditor;
        private NoiseGeneratorSOEditor m_noiseGeneratorSOEditor;
        private TerrainConfigSO m_terrainConfig;
        private NoiseGeneratorSO m_noiseGen;
        private void OnEnable()
        {
            var terrainGen = (TerrainGeneratorMB)target;
            m_terrainConfig = terrainGen.GetTerrainConfigSO();
            m_noiseGen = terrainGen.GetNoiseGeneratorSO();

            if (!m_terrainConfig || !m_noiseGen) return;

            if (!m_terrainConfigSOEditor)
                m_terrainConfigSOEditor = (TerrainConfigSOEditor)Editor
                                            .CreateEditor(m_terrainConfig, typeof(TerrainConfigSOEditor));

            if (!m_noiseGeneratorSOEditor)
                m_noiseGeneratorSOEditor = (NoiseGeneratorSOEditor)Editor
                                            .CreateEditor(m_noiseGen, typeof(NoiseGeneratorSOEditor));
        }

    }
}
