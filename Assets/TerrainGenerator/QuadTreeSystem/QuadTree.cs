using System.Collections.Generic;
using UnityEngine;

namespace TerrainGenerator.QuadTreeSystem
{
    // ReSharper disable all
    public class QuadTree {

        #region Fields

        private readonly QuadNode _rootNode;
        private QTViewer _viewer;

        private int _minChunkSize;
        private int _treeHeight;
        private float _nodeMultiplier;

        #endregion

        public QuadTree(QuadNode rootNode, int minChunkSize, float nodeMultiplier) {
            // Assign Vars
            _rootNode = rootNode;
            _minChunkSize =  minChunkSize;
            _nodeMultiplier = nodeMultiplier;
        }

        #region Getters & Setters
        public QuadNode GetRootNode() { return _rootNode; }
        public int GetTreeHeight() { return _treeHeight; }
        #endregion

        #region Helper Functions
        public List<QuadNode> Update() {
            // Function updates the quad tree based on the view triangle
            int maxLevel = 0;
            
            if (_viewer == null) { throw new System.Exception("Viewer has not been set");}

            // BFS to detect culled nodes and split nodes
            Queue<QuadNode> queue = new();
            queue.Enqueue(_rootNode);

            List<QuadNode> culledNodes = new();

            // Detect nodes to cull and split
            while (queue.Count > 0) {
                QuadNode curr = queue.Dequeue();
                if (null == curr) continue;
                if(curr.GetLevel() > maxLevel) maxLevel = curr.GetLevel();

                var viewerPosition = _viewer.GetPosition();

                if(!curr.IsLeafNode() && curr.IsCloseEnoughToSplitNode(viewerPosition, _nodeMultiplier)) {
                    EnqueueChildren(queue, curr);
                    continue;
                }

                if(!curr.IsLeafNode() && !curr.IsCloseEnoughToSplitNode(viewerPosition, _nodeMultiplier)) {
                    // turn this bad boy into a leaf node
                    culledNodes.AddRange(curr.GetAllLeafNodes());
                    curr.ClearChildren();
                    continue;
                }

                if (curr.IsLeafNode() && curr.IsCloseEnoughToSplitNode(viewerPosition, _nodeMultiplier)) {
                    if (curr.GetSideLength() > _minChunkSize) {
                        SplitNode(curr);
                        culledNodes.Add(curr);
                        EnqueueChildren(queue, curr);
                    }
                    continue;
                }

                if(curr.IsLeafNode() && !curr.IsCloseEnoughToSplitNode(viewerPosition, _nodeMultiplier)) {
                    continue;
                }

                throw new System.Exception("The code shouldn't reach this point");         
            }

            _treeHeight = maxLevel + 1;
            return culledNodes;
        }
        private void SplitNode(QuadNode curr) {
            Vector2 botLeftPoint = curr.GetBotLeftPoint();
            float sideLength = curr.GetSideLength();

            QuadNode botLeft = new(curr, botLeftPoint, 0.5f * sideLength);
            QuadNode topLeft = new(curr, new Vector2(botLeftPoint.x, botLeftPoint.y + 0.5f * sideLength), 0.5f * sideLength);
            QuadNode topRight = new(curr, new Vector2(botLeftPoint.x + 0.5f * sideLength, botLeftPoint.y + 0.5f * sideLength), 0.5f * sideLength);
            QuadNode botRight = new(curr, new Vector2(botLeftPoint.x + 0.5f * sideLength, botLeftPoint.y), 0.5f * sideLength);
        
            botLeft.SetLevel(curr.GetLevel() + 1);
            botRight.SetLevel(curr.GetLevel() + 1);
            topLeft.SetLevel(curr.GetLevel() + 1);
            topRight.SetLevel(curr.GetLevel() + 1);

            curr.SetBotLeftChild(botLeft);
            curr.SetTopLeftChild(topLeft);
            curr.SetBotRightChild(botRight);
            curr.SetTopRightChild(topRight);

        }
        private void EnqueueChildren(Queue<QuadNode> queue, QuadNode curr) {
            foreach (QuadNode child in curr.GetChildren()) queue.Enqueue(child);
        }
        public void SetViewer(QTViewer viewer) { this._viewer = viewer; }

        public void PrintTree()
        {
            Queue<QuadNode> queue = new();
            queue.Enqueue(_rootNode);
            while (queue.Count > 0)
            {
                var curr = queue.Dequeue();
                if (curr is null) continue;
                Debug.Log($"BL:{curr.GetBotLeftPoint()}, SL: {curr.GetSideLength()}");
                EnqueueChildren(queue, curr);
            }
        }
        
        public object GetViewer() {
            return _viewer;
        }
        #endregion

    }
}
