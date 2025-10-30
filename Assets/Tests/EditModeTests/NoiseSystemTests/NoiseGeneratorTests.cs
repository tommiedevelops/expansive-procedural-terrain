using System.Collections;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using _Scripts.NoiseSystem;
using _Scripts.NoiseSystem.ScriptableObjects;

namespace EditModeTests
{
    public class NoiseGeneratorTests
    {
        private class ExampleNoise1 : NoiseLayerSO
        {

            public override void ValidateValues()
            {
                throw new System.NotImplementedException();
            }

            public override float Evaluate(Vector2 point)
            {
                return 1f;
            }

            public override float Compose(float currentValue, Vector2 point)
            {
                throw new System.NotImplementedException();
            }
        }

        private class ExampleNoise2 : NoiseLayerSO
        {

            public override void ValidateValues()
            {
                throw new System.NotImplementedException();
            }

            public override float Evaluate(Vector2 point)
            {
                return Mathf.Approximately((point.x+point.y), 2f) ? 1 : 0;
            }

            public override float Compose(float currentValue, Vector2 point)
            {
                throw new System.NotImplementedException();
            }
        }

        private class ExampleNoise3 : NoiseLayerSO
        {

            public override void ValidateValues()
            {
                throw new System.NotImplementedException();
            }

            public override float Evaluate(Vector2 point)
            {
                return (point.x == 0 && point.y == 0) ? 1 : 0;
            }

            public override float Compose(float currentValue, Vector2 point)
            {
                throw new System.NotImplementedException();
            }
        }

        [Test]
        public void Can_Instantiate_A_NoiseGenerator()
        {
            var noiseGeneratorUnderTest = new NoiseGenerator();
            Assert.IsNotNull(noiseGeneratorUnderTest);
        }

        [Test]
        public void Can_Remove_Specified_Layer()
        {
            var noiseGeneratorUnderTest = new NoiseGenerator();
            noiseGeneratorUnderTest.AddLayer(new ExampleNoise1());
            noiseGeneratorUnderTest.AddLayer(new ExampleNoise2());
            noiseGeneratorUnderTest.AddLayer(new ExampleNoise3());

            noiseGeneratorUnderTest.RemoveLayer<ExampleNoise1>();
            
            var layers = noiseGeneratorUnderTest.GetLayers();
            foreach(var layer in layers) {Assert.That(layer.GetType(), Is.Not.EqualTo(typeof(ExampleNoise1)));}
        }
    }
}
