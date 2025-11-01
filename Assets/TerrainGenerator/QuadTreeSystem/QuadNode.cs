using UnityEngine;
using System.Collections.Generic;
using System;

namespace TerrainGenerator.QuadTreeSystem {
    public class QuadNode {
        public enum NodeType {
            BL,
            TL,
            TR,
            BR,
            NOTSET
        }

        #region Fields

        // LOGICAL
        NodeType type;
        bool levelSet;
        int level;
        int maxLOD;
        QuadNode parent;
        QuadNode[] children = null;

        // SPATIAL
        Vector2 bottomLeftPoint;
        float sideLength; // in metres
        Bounds bounds;

        #endregion

        #region Constructor

        // CONSTRUCTOR
        public QuadNode(QuadNode parent, Vector2 botLeftPoint, float sideLength) {

            Vector3 boundsCenter = new(botLeftPoint.x + sideLength / 2f, 0f, botLeftPoint.y + sideLength / 2f);
            Vector3 boundsDimensions = new(sideLength, 0f, sideLength);

            this.bottomLeftPoint = botLeftPoint;
            this.sideLength = sideLength;
            this.children = new QuadNode[4];
            this.parent = parent;
            this.type = NodeType.NOTSET;
            this.levelSet = false;
            this.bounds = new Bounds(boundsCenter, boundsDimensions);
        }

        #endregion

        #region Helper Functions

        // HELPERS

        public override string ToString() {
            return $"BLP: {bottomLeftPoint.ToString()} SL: {sideLength.ToString()}";
        }

        public bool IsCloseEnoughToSplitNode(Vector2 viewerPosition, float multiplier) {
            var quadNodeCenter = new Vector2(bottomLeftPoint.x + sideLength / 2f, bottomLeftPoint.y + sideLength / 2f);
            var threshold = multiplier * sideLength;

            return (viewerPosition - quadNodeCenter).magnitude < threshold;

        }

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

        internal void RemoveChild(NodeType nodeType) {
            if (nodeType == NodeType.NOTSET) {
                Debug.Log("Node type not set");
                return;
            }

            children[(int)nodeType] = null;
        }

        internal NodeType GetNodeType() {
            return type;
        }

        internal void ReplaceChild(QuadNode oldChild, QuadNode newChild) {
            NodeType type = oldChild.GetNodeType();

            switch (type) {
                case NodeType.BL:
                    SetBotLeftChild(newChild);
                    break;
                case NodeType.TL:
                    SetTopLeftChild(newChild);
                    break;
                case NodeType.TR:
                    SetTopRightChild(newChild);
                    break;
                case NodeType.BR:
                    SetBotRightChild(newChild);
                    break;
            }
        }

        public List<QuadNode> GetAllLeafNodes() {
            List<QuadNode> leafNodes = new();

            // BFS traverse the tree. If leaf node, add to array
            Queue<QuadNode> queue = new();
            queue.Enqueue(this);

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

        public void PrintNode() {
            Debug.Log($"BotLeftPoint:{GetBotLeftPoint()} SideLength:{GetSideLength()}");
        }

        #endregion

        #region Getters & Setters

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
            return bottomLeftPoint;
        }

        public QuadNode[] GetChildren() {
            return children;
        }

        internal void SetBotLeftChild(QuadNode botLeft) {
            this.type = NodeType.BL;
            children[0] = botLeft;
        }

        internal void SetTopLeftChild(QuadNode topLeft) {
            this.type = NodeType.TL;
            children[1] = topLeft;
        }

        internal void SetTopRightChild(QuadNode topRight) {
            this.type = NodeType.TR;
            children[2] = topRight;
        }

        internal void SetBotRightChild(QuadNode botRight) {
            this.type = NodeType.BR;
            children[3] = botRight;
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

}