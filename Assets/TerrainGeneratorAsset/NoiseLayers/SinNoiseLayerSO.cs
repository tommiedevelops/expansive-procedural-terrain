using TerrainGeneratorAsset;
using UnityEngine;

[CreateAssetMenu(menuName = "Noise/Sin")]
public class SinNoiseLayerSO : NoiseLayerSO
{
    [SerializeField] float xFreq = 1.0f;
    [SerializeField] float yFreq = 1.0f;
    public override float Evaluate(float x, float y)
    {
        return Mathf.Sin(xFreq * x);
    }

    public override void ValidateValues()
    {
        return;
    }
}
