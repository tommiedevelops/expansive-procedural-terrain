using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using _Scripts.NoiseSystem;
using _Scripts.NoiseSystem.ScriptableObjects;

namespace EditModeTests
{
    public class SomeNoise : NoiseLayerSO
    {

        public override void ValidateValues()
        {
            throw new NotImplementedException();
        }

        public override float Evaluate(Vector2 point)
        {
            return 0f;
        }

        public override float Compose(float currentValue, Vector2 point)
        {
            throw new NotImplementedException();
        }
    }
    public class NoiseLayerSoTests
    {
        [Test]
        public void Can_Instantiate_A_NoiseLayer()
        {
            var noiseLayerUnderTest = new SomeNoise();
            Assert.That(noiseLayerUnderTest, Is.Not.Null);
        }
    }

}

