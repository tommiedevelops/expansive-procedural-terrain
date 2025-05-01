using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using static QuadTree;

namespace EditModeTests
{
    public class QuadTreeTests
    {

        [Test]
        public void Can_Create_Empty_QuadTree() {
            float rootNodeSideLength = 1024;
            Vector2 botLeftPoint = new Vector2(-rootNodeSideLength / 2f, -rootNodeSideLength / 2f);
            var rootNode = new QuadNode(null, botLeftPoint, rootNodeSideLength);
            var tree = new QuadTree(rootNode);

            Assert.That(tree != null);
            Assert.That(tree.GetRootNode(), Is.EqualTo(rootNode));
            Assert.That(tree.GetRootNode().GetChildren(), Is.All.Null);
            Assert.That(tree.GetViewer() == null);
        }

        [Test]
        public void Can_Create_Small_QuadTree_With_Viewer() {
            //arrange
            
            //act
            //assert
        }
    }
}
