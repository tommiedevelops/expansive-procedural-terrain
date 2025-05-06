using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Scripts.QuadTreeSystem;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.PlayerLoop;
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

        [Test]
        public void Testing_Update_Method_Returns_Correct_Culled_Nodes_Simple()
        {
            const int minChunkSize = 2;
            var rootNode = new QuadNode(null, Vector2.zero, 8f);
            var quadTreeUnderTest = new QuadTree(rootNode, minChunkSize);
            var go = new GameObject();
            go.transform.position = new Vector3(6f, 0f, 6f);
            var viewer = new QTViewer(go.transform, 30, 1);
            quadTreeUnderTest.SetViewer(viewer);
            
            Debug.Log("========================");
            quadTreeUnderTest.Update();
            quadTreeUnderTest.PrintTree();
            var leafNodes = quadTreeUnderTest.GetRootNode().GetAllLeafNodes();
            
            var targetPoints = new List<Vector2>
            {
                new Vector2(4, 4),
                new Vector2(4, 6),
                new Vector2(6, 4),
                new Vector2(6, 6)
            };
            
            var expectedCulledNodes = leafNodes
                .Where(node => targetPoints.Contains(node.GetBotLeftPoint()))
                .ToList();
            
            Debug.Log($"Expected culled nodes: {expectedCulledNodes}");
            
            go.transform.position = new Vector3(2f, 0f, 2f);
            Debug.Log("========================");
            var culledNodes = quadTreeUnderTest.Update();
            quadTreeUnderTest.PrintTree();
            
            
            Assert.That(culledNodes, Is.EqualTo(expectedCulledNodes));
            
           
        }
        
        
    }
    
    
}
