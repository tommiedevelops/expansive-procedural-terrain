using TerrainGenerator.NoiseLayers;
using UnityEngine;

[CreateAssetMenu(menuName =  "Noise/FlatNoiseLayer")]
public class FlatLayerSO : NoiseLayerSO
{
    public override void ValidateValues() {
        return;
    }
    public override float Evaluate(Vector2 point) {
        return 0.0f;
    }
    public override float Compose(float currentValue, Vector2 point) {
        throw new System.NotImplementedException();
    }
}
