using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;

namespace EditModeTests {
    public class PlaneMeshGeneratorTests {

        #region Vertex Grid Tests
        [Test]
        public void CreateEmptyVertexGrid() {
            /* EDGE CASE: Testing creating an empty vertex list 
             */
            float worldXLength = 0f;
            float worldYLength = 0f;
            int numXVerts = 0;
            int numYVerts = 0;


            Vector3[] actualGrid = PlaneMeshGenerator.GenerateVertexGrid(worldXLength,worldYLength,numXVerts,numYVerts);

            Assert.That(actualGrid, Is.Not.Null); // It should return something
            Assert.That(actualGrid.Length, Is.EqualTo(0)); // It should return an empty list
        }

        [Test]
        public void CreateTwoByTwoVertexGrid() {
            // Arrange
            float worldXLength = 1f;
            float worldYLength = 1f;
            int numXVerts = 2;
            int numYVerts = 2;

            // ORDER IS IMPORTANT HERE
            //  3 4
            //  1 2
            // Increasing +x and Increasing +z.

            var point_1 = new Vector3(0f, 0f, 0f);
            var point_2 = new Vector3(1f, 0f, 0f);
            var point_3 = new Vector3(0f, 0f, 1f);
            var point_4 = new Vector3(1f, 0f, 1f);

            // Act
            Vector3[] expectedGrid = { point_1, point_2, point_3, point_4 };
            var actualGrid = PlaneMeshGenerator.GenerateVertexGrid(worldXLength, worldYLength, numXVerts, numYVerts);

            // Assert

            // They should be the same length
            Assert.That(expectedGrid.Length, Is.EqualTo(actualGrid.Length)); 

            // They should be listed in the correct order
            for(int i = 0; i < actualGrid.Length; i++) {
                Assert.That(actualGrid[i], Is.EqualTo(expectedGrid[i]).Using(Vector3EqualityComparer.Instance));
            }
        }

        #endregion

        #region Triangle Array Tests
        [Test]
        public void TestTwoByTwoTriangleArrayGeneration() {
            // Arrange
            var point_1 = new Vector3(0f, 0f, 0f);
            var point_2 = new Vector3(1f, 0f, 0f);
            var point_3 = new Vector3(1f, 0f, 1f);
            var point_4 = new Vector3(0f, 0f, 1f);

            // Act
            Vector3[] vertexArray = { point_1, point_2, point_3, point_4 };
            int[] expectedTriangleArray = { 0, 2, 3, 0, 3, 1 }; // Triangles defined clockwise
            int[] actualTriangleArray = PlaneMeshGenerator.GenerateTriangleArray(vertexArray, 2, 2);

            // Assert
            Assert.That(actualTriangleArray.Length, Is.EqualTo(expectedTriangleArray.Length));
            
            for(int i = 0; i < actualTriangleArray.Length; i++)
                Assert.That(actualTriangleArray[i], Is.EqualTo(expectedTriangleArray[i]));

        }
        #endregion

        #region Mesh Generation Tests
        [Test]
        public void TestTwoByTwoMeshGeneration() {
            PlaneMeshGenerator.MeshData meshData = new(2, 2, 1);
            Mesh mesh = PlaneMeshGenerator.GeneratePlaneMesh(meshData);

            var point_1 = new Vector3(0f, 0f, 0f);
            var point_2 = new Vector3(1f, 0f, 0f);
            var point_3 = new Vector3(0f, 0f, 1f);
            var point_4 = new Vector3(1f, 0f, 1f);
            Vector3[] expectedVertexArray = { point_1, point_2, point_3, point_4 };
            int[] expectedTriangleArray = { 0, 2, 3, 0, 3, 1 }; // Triangles defined clockwise

            Assert.That(mesh.vertices, Is.EqualTo(expectedVertexArray));
            Assert.That(mesh.triangles, Is.EqualTo(expectedTriangleArray));
        }

        #endregion

    }
}
