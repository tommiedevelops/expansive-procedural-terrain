using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using _Scripts.NoiseSystem;

namespace EditModeTests
{
    public class NoiseGeneratorTests
    {
        // A Test behaves as an ordinary method
        private class ExampleNoise1 : NoiseLayer
        {
            public override float Evaluate(float x, float y)
            {
                return 1f;
            }
        }

        private class ExampleNoise2 : NoiseLayer
        {
            public override float Evaluate(float x, float y)
            {
                return Mathf.Approximately((x+y), 2f) ? 1 : 0;
            }
        }
        
        [Test]
        public void Can_Instantiate_A_NoiseGenerator()
        {
            var noiseGeneratorUnderTest = new NoiseGenerator();
            Assert.IsNotNull(noiseGeneratorUnderTest);
        }

        [Test]
        public void Test_Origin_Is_Vector3_Zero_By_Default()
        {
            var noiseGeneratorUnderTest = new NoiseGenerator();
            var expectedOrigin = new Vector3(0, 0, 0);
            var origin = noiseGeneratorUnderTest.GetWorldSpaceOrigin();
            Assert.AreEqual(expectedOrigin, origin);
        }

        [Test]
        public void Test_Can_Set_Custom_Origin()
        {
            var noiseGeneratorUnderTest = new NoiseGenerator();
            var customOrigin = new Vector3(1, 1, 1);
            noiseGeneratorUnderTest.SetWorldSpaceOrigin(customOrigin);
            Assert.AreEqual(customOrigin, noiseGeneratorUnderTest.GetWorldSpaceOrigin());
        }
        
        [Test]
        public void Test_Can_Set_Grid_Dimensions()
        {
            var noiseGeneratorUnderTest = new NoiseGenerator();
            int gridWidth = 2;
            int gridHeight = 2;
            
            noiseGeneratorUnderTest.SetGridWidth(gridWidth);
            noiseGeneratorUnderTest.SetGridHeight(gridHeight);
            
            Assert.That(noiseGeneratorUnderTest.GetGridWidth(), Is.EqualTo(gridWidth));
            Assert.That(noiseGeneratorUnderTest.GetGridHeight(), Is.EqualTo(gridHeight));
        }
        
        [Test]
        public void Test_Can_Apply_Noise_To_FloatArray()
        {
            const int gridWidth = 3;
            const int gridHeight = 3;
            var noiseGeneratorUnderTest = new NoiseGenerator();
            noiseGeneratorUnderTest.SetGridDimensions(gridWidth, gridHeight);
            noiseGeneratorUnderTest.AddLayer(new ExampleNoise1());
            
            
            var expectedArray = new float[,]
            {
                { 1, 1, 1 },
                { 1, 1, 1 },
                { 1, 1, 1 }
            };
            
            var array = new float[,]
            {
                { 0, 0, 0 },
                { 0, 0, 0 },
                { 0, 0, 0 }
            };

            noiseGeneratorUnderTest.ApplyNoise(array);
            
            Assert.That(array, Is.EqualTo(expectedArray));
            
        }

        [Test]
        public void Test_Can_Apply_Noise_To_Mesh()
        {
            const int gridWidth = 2;
            const int gridHeight = 2;
            var noiseGeneratorUnderTest = new NoiseGenerator();
            noiseGeneratorUnderTest.SetGridDimensions(gridWidth, gridHeight);
            noiseGeneratorUnderTest.AddLayer(new ExampleNoise2());
            
            var mesh = new Mesh()
            {
                vertices = new Vector3[]
                {
                    new Vector3(0f, 0f, 0f),
                    new Vector3(1f, 0f, 0f),
                    new Vector3(0f, 0f, 1f),
                    new Vector3(1f, 0f, 1f)
                },
                
                triangles = new int[] {0,2,1,1,2,3}
            };
            
            
            var expectedMesh = new Mesh()
            {
                vertices = new Vector3[]
                {
                    new Vector3(0f, 0f, 0f),
                    new Vector3(1f, 0f, 0f),
                    new Vector3(0f, 0f, 1f),
                    new Vector3(1f, 1f, 1f)
                },
                
                triangles = new int[] {0,2,1,1,2,3}
            };
            
            noiseGeneratorUnderTest.ApplyNoise(mesh);
            Assert.That(mesh.vertices, Is.EqualTo(expectedMesh.vertices));
            
        }
    }
}
