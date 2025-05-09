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

