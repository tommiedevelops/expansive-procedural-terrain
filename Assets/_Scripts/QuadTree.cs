using UnityEngine;
using System.Collections.Generic;
public class QuadTree {
    QuadNode rootNode;
    QTViewer viewer;

    // CHILD CLASSES
    public class QuadNode {
        /* Nodes only work in metres NOT in chunk units */
        Vector2 botLeftPoint;
        float sideLength; // in metres
        public QuadNode botLeftChild, topLeftChild, topRightChild, botRightChild;
        Bounds bounds;

        // CONSTRUCTOR
        public QuadNode(Vector2 botLeftPoint, float sideLength) {
            this.botLeftPoint = botLeftPoint;
            this.sideLength = sideLength;

            Vector3 boundsCenter = new(botLeftPoint.x + sideLength / 2f, 0f, botLeftPoint.y + sideLength / 2f);
            Vector3 boundsDimensions = new(sideLength, 0f, sideLength);

            this.bounds = new Bounds(boundsCenter, boundsDimensions);

            this.botLeftChild = null;
            this.botRightChild = null;
            this.topRightChild = null;
            this.topLeftChild = null;
        }

        // HELPERS
        public Bounds GetBounds() { return bounds; }
        public float GetSideLength() { return sideLength; }
        public Vector2 GetBotLeftPoint() { return botLeftPoint; }

    }

    // CONSTRUCTOR
    public QuadTree(QuadNode rootNode, QTViewer viewer, int minChunkSideLength) {
        // Assign Vars
        this.rootNode = rootNode;
        this.viewer = viewer;

        // Construct the Quad Tree
        ConstructQuadTree(minChunkSideLength, rootNode.GetSideLength());
    }

    // GETTERS
    public QuadNode GetRootNode() { return rootNode; }
    public Bounds GetTriBounds() { return viewer.GetTriBounds(); }
    public Vector3[] GetViewTriangle() { return viewer.GetViewTriangle(); }

    // HELPERS
    private void ConstructQuadTree(int minChunkSideLength, float worldSideLength) {
        // Construct the quad tree
        Queue<QuadNode> queue = new();
        queue.Enqueue(rootNode);
        while (queue.Count > 0) {

            // Breadth First Search Construction of Quad Tree
            QuadNode curr = queue.Dequeue();

            //Debug.Log($"{curr.GetBotLeftPoint()} intersects with view tri: {IntersectsWithViewTri(curr)}");

            if (IntersectsWithViewTri(curr) && (curr.GetSideLength() > minChunkSideLength)) {
                Vector2 botLeftPoint = curr.GetBotLeftPoint();
                float sideLength = curr.GetSideLength();
                QuadNode botLeft = new(botLeftPoint, 0.5f * sideLength);
                QuadNode topLeft = new(new Vector2(botLeftPoint.x, botLeftPoint.y + 0.5f * sideLength), 0.5f * sideLength);
                QuadNode topRight = new(new Vector2(botLeftPoint.x + 0.5f * sideLength, botLeftPoint.y + 0.5f * sideLength), 0.5f * sideLength);
                QuadNode botRight = new(new Vector2(botLeftPoint.x + 0.5f * sideLength, botLeftPoint.y), 0.5f * sideLength);

                curr.botLeftChild = botLeft;
                curr.topLeftChild = topLeft;
                curr.botRightChild = botRight;
                curr.topRightChild = topRight;

                queue.Enqueue(botLeft);
                queue.Enqueue(topLeft);
                queue.Enqueue(topRight);
                queue.Enqueue(botRight);
            }
        }
    }
    private bool IntersectsWithViewTri(QuadNode node) {
        // Test performed using Separating Axis Theorem
        // More info: https://dyn4j.org/2010/01/sat/
        
        Bounds nodeBounds = node.GetBounds();
        return nodeBounds.Intersects(viewer.GetTriBounds());
    }
    public void PrintTree(ref List<Bounds> boundsToDraw) {
        Queue<QuadNode> queue = new();
        queue.Enqueue(rootNode);
        while(queue.Count > 0) {
            QuadNode curr = queue.Dequeue();

            if (curr.botLeftChild != null) queue.Enqueue(curr.botLeftChild);
            if (curr.botRightChild != null) queue.Enqueue(curr.botRightChild);
            if (curr.topLeftChild != null) queue.Enqueue(curr.topLeftChild);
            if (curr.topRightChild != null) queue.Enqueue(curr.topRightChild);

            boundsToDraw.Add(curr.GetBounds());
        }
    }
}

