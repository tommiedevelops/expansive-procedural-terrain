using System.Collections.Generic;
using UnityEngine;

namespace TerrainGeneratorAsset
{
    public sealed class QuadNode {

        bool levelSet;
        int level;
        int maxLOD;
        QuadNode parent;
        QuadNode[] children = null;
        Vector2 botLeftPoint;
        float sideLength; // in metres
        Bounds bounds;

        public QuadNode(QuadNode parent, Vector2 botLeftPoint, float sideLength) {

            Vector3 boundsCenter = new(botLeftPoint.x + sideLength / 2f, 0f, botLeftPoint.y + sideLength / 2f);
            Vector3 boundsDimensions = new(sideLength, 0f, sideLength);

            this.botLeftPoint = botLeftPoint;
            this.sideLength = sideLength;
            this.children = new QuadNode[4];
            this.parent = parent;
            this.levelSet = false;
            this.bounds = new Bounds(boundsCenter, boundsDimensions);
        }
        public override string ToString() {
            return $"BLP: {botLeftPoint.ToString()} SL: {sideLength.ToString()}";
        }
        public bool IsCloseEnoughToSplitNode(Vector2 viewerPosition, float multiplier) {
            var quadNodeCenter = new Vector2(botLeftPoint.x + sideLength / 2f, botLeftPoint.y + sideLength / 2f);
            var threshold = multiplier * sideLength;

            return (viewerPosition - quadNodeCenter).magnitude < threshold;
        }

        #region Getters & Setters
        public bool IsLeafNode() {
            return !HasChildren();
        }
        public bool HasChildren() {
            bool hasChildren = false;
            foreach (QuadNode child in children)
                if (child != null)
                    hasChildren = true;

            return hasChildren;
        }
        public bool IsLevelSet() {
            return levelSet;
        }
        public void AddChild(int idx, QuadNode newChild) {
            children[idx] = newChild;
        }
        internal void RemoveChild(int idx) {
            children[idx] = null;
        }
        public void PrintNode() {
            Debug.Log($"BotLeftPoint:{GetBotLeftPoint()} SideLength:{GetSideLength()}");
        }
        public void SetLevel(int level) {
            this.level = level;
            levelSet = true;
        } // JUST BROKE THIS BTW
        public int GetLevel() {
            return this.level;
        }
        public Bounds GetBounds() {
            return bounds;
        }
        public float GetSideLength() {
            return sideLength;
        }
        public void SetParent(QuadNode parent) {
            this.parent = parent;
        }
        public QuadNode GetParent() {
            return this.parent;
        }
        public Vector2 GetBotLeftPoint() {
            return botLeftPoint;
        }
        public QuadNode[] GetChildren() {
            return children;
        }
        internal void ClearChildren() {
            for (int i = 0; i < children.Length; i++) {
                children[i] = null;
            }
        }
        internal int GetLOD() {
            return maxLOD - level;
        }
        internal void SetMaxLOD(int maxHeight) {
            maxLOD = maxHeight;
        }

        #endregion

    }
    public sealed class QuadTree {
        
        private readonly QuadNode _rootNode;

        private int _minChunkSize;
        private int _treeHeight;
        private float _nodeMultiplier;
        public QuadTree(QuadNode rootNode, int minChunkSize, float nodeMultiplier) {
            // Assign Vars
            _rootNode = rootNode;
            _minChunkSize =  minChunkSize;
            _nodeMultiplier = nodeMultiplier;
        }
        public QuadNode GetRootNode() { return _rootNode; }
        public int GetTreeHeight() { return _treeHeight; }
        public List<QuadNode> GetAllLeafNodes(QuadNode startNode) {
            List<QuadNode> leafNodes = new();

            // BFS traverse the tree. If leaf node, add to array
            Queue<QuadNode> queue = new();
            queue.Enqueue(startNode);

            while (queue.Count > 0) {
                QuadNode curr = queue.Dequeue();

                if (curr == null) continue;

                // check if a leaf node
                if (!curr.HasChildren()) {
                    leafNodes.Add(curr);
                }

                foreach (QuadNode child in curr.GetChildren()) {
                    queue.Enqueue(child);
                }

            }

            return leafNodes;
        }
        public List<QuadNode> UpdateChildren(Vector3 viewerPosition) {

            Vector2 pos = new(viewerPosition.x, viewerPosition.z);

            int maxLevel = 0;

            Queue<QuadNode> queue = new();
            queue.Enqueue(_rootNode);

            List<QuadNode> culledNodes = new();

            while (queue.Count > 0) {
                
                QuadNode curr = queue.Dequeue();
                if (null == curr) continue;
                
                if(curr.GetLevel() > maxLevel) maxLevel = curr.GetLevel();

                if(!curr.IsLeafNode() && curr.IsCloseEnoughToSplitNode(pos, _nodeMultiplier)) {
                    EnqueueChildren(queue, curr);
                    continue;
                }

                if(!curr.IsLeafNode() && !curr.IsCloseEnoughToSplitNode(pos, _nodeMultiplier)) {
                    culledNodes.AddRange(GetAllLeafNodes(curr));
                    curr.ClearChildren();
                    continue;
                }

                if (curr.IsLeafNode() && curr.IsCloseEnoughToSplitNode(pos, _nodeMultiplier)) {
                    if (curr.GetSideLength() > _minChunkSize) {
                        SplitNode(curr);
                        culledNodes.Add(curr);
                        EnqueueChildren(queue, curr);
                    }
                }

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

            curr.AddChild(0, botLeft);
            curr.AddChild(1, topLeft);
            curr.AddChild(2, botRight);
            curr.AddChild(3, topRight);

        }
        private void EnqueueChildren(Queue<QuadNode> queue, QuadNode curr) {
            foreach (QuadNode child in curr.GetChildren()) queue.Enqueue(child);
        }
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

    }
}
