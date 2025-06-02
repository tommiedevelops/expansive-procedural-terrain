using _Scripts.NoiseSystem.ScriptableObjects;
using UnityEngine;

[CreateAssetMenu(fileName = "Sine2DSO", menuName = "Scriptable Objects/Sine2DSO")]
public class Sine2DSO : NoiseLayerSO
{
    public override void ValidateValues()
    {
        Debug.Log("Validated from Sine2DSO");
    }

    public override float Evaluate(Vector2 point)
    {
        return 1;
    }

    public override float Compose(float currentValue, Vector2 point)
    {
        return 1 - currentValue;
    }
}
