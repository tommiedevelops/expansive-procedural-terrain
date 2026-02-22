
namespace TerrainGeneratorAsset
{
    public interface INoiseLayer
    {
        void Validate();
        float Sample(float x, float y);

    }
}
