using System.Collections;
using _Scripts.QuadTreeSystem;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using static _Scripts.QuadTreeSystem.QuadTree;

namespace EditModeTests
{
    public class QuadTreeTests
    {

        [Test]
        public void Can_Create_Empty_QuadTree() {
            const float rootNodeSideLength = 1024;
            const int minChunkSize = 120;
            var botLeftPoint = new Vector2(-rootNodeSideLength / 2f, -rootNodeSideLength / 2f);
            var rootNode = new QuadNode(null, botLeftPoint, rootNodeSideLength);
            var tree = new QuadTree(rootNode, minChunkSize);

            Assert.That(tree != null);
            Assert.That(tree.GetRootNode(), Is.EqualTo(rootNode));
            Assert.That(tree.GetRootNode().GetChildren(), Is.All.Null);
            Assert.That(tree.GetViewer() == null);
        }

        [Test]
        public void Can_Create_QuadTree_And_Set_Viewer()
        {
            const float sideLength = 8f;
            const int minChunkSize = 2;
            var botLeftPoint = new Vector2(-sideLength / 2f, -sideLength / 2f);
            var rootNode = new QuadNode(null, botLeftPoint, 8f);
            var quadTree = new QuadTree(rootNode, minChunkSize);
            var cameraTransform = new GameObject().transform;
            var viewer = new QTViewer(cameraTransform, 30f, 1f);
            
            quadTree.SetViewer(viewer);
            quadTree.Update();
            
            Assert.That(quadTree, Is.Not.Null);
            Assert.That(quadTree.GetViewer(), Is.EqualTo(viewer));
            Assert.That(quadTree.GetTreeHeight(), Is.EqualTo(3));
            
        }
        
        //public void Can_Compute_Correct_Tree_Heights_When_Viewer_At_Zero_And_RD_Is_One(float rootNodeSideLength, int minChunkSize, int expectedHeight)
    
    }
}
