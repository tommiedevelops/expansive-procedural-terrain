using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using _Scripts.NoiseSystem;

namespace EditModeTests
{
    public class SomeNoise : NoiseLayer
    {
        public override float Evaluate(float x, float y)
        {
            return 0f;
        }
    }
    public class NoiseLayerTests
    {
        [Test]
        public void Can_Instantiate_A_NoiseLayer()
        {
            var noiseLayerUnderTest = new SomeNoise();
            Assert.That(noiseLayerUnderTest, Is.Not.Null);
        }
    }

}

