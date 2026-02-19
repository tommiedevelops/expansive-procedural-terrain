using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

namespace TerrainGeneratorAsset
{
    public abstract class NoiseCombinerSO : ScriptableObject
    {
        private List<NoiseLayerSO> m_noiseLayers;
        public NoiseCombinerSO(List<NoiseLayerSO> noiseLayers) { m_noiseLayers = noiseLayers; }
        public abstract float Combine();
    }

}