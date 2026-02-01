using TerrainGenerator.NoiseLayers;
using UnityEngine;

[CreateAssetMenu(menuName =  "Noise/FlatNoiseLayer")]
public class FlatLayerSO : NoiseLayerSO
{
    public override void ValidateValues() {
        return;
    }
    public override float Evaluate(float x, float y) {
        return 0.0f;
    }
}
