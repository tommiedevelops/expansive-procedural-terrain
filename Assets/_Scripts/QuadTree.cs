using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
using Unity.Mathematics;
using UnityEditor.Rendering;
using Unity.Jobs;
public class QuadTree {
    QuadNode rootNode;
    Vector3[] viewTriangle;
    Bounds tempTriBounds;

    int treeHeight;
    private const int maxLOD = 7;

    // CHILD CLASSES
    public class QuadNode {
        /* Nodes only work in metres NOT in chunk units */
        Vector2 botLeftPoint;
        float sideLength; // in metres
        public QuadNode botLeftChild, topLeftChild, topRightChild, botRightChild;
        Bounds bounds;
        int level;
        bool levelSet;

        // CONSTRUCTOR
        public QuadNode(Vector2 botLeftPoint, float sideLength) {
            this.botLeftPoint = botLeftPoint;
            this.sideLength = sideLength;
            levelSet = false;

            Vector3 boundsCenter = new(botLeftPoint.x + sideLength / 2f, 0f, botLeftPoint.y + sideLength / 2f);
            Vector3 boundsDimensions = new(sideLength, 0f, sideLength);

            this.bounds = new Bounds(boundsCenter, boundsDimensions);

            this.botLeftChild = null;
            this.botRightChild = null;
            this.topRightChild = null;
            this.topLeftChild = null;
        }

        // HELPERS
        public bool HasChildren() {
            return (botLeftChild != null) && (botRightChild != null) && (topLeftChild != null) && (topRightChild != null);
        }
        public bool IsLevelSet() { return levelSet; }

        
        public void SetLevel(int level) { this.level = Mathf.Min(level, maxLOD); levelSet = true; }
        public int GetLevel() { return this.level; }
        public Bounds GetBounds() { return bounds; }
        public float GetSideLength() { return sideLength; }
        public Vector2 GetBotLeftPoint() { return botLeftPoint; }
        private bool IsLeafNode() {
            return botLeftChild != null
            || botRightChild != null
            || topRightChild != null
            || topLeftChild != null;
        }
        public uint ComputeHash() {
            float3 data = new float3(botLeftPoint.x, botLeftPoint.y, sideLength);
            return math.hash(data);
        }
        private void RenderLeafNodeChunk() {
            // 1. Check if this node is a leaf node.
            if (IsLeafNode()) {
                Debug.Log("This node is not a leaf node. Cannot render.");
                return;
            }

            // 2. Initialise a thread for this chunk to request mesh data
            // 3. Initialise a thread for this chunk to request noise data
            // 4. 
            // 3. 

            // Ignoring all shading for now.

            // This function should generate the mesh and overlay the correct noise values
            // I should probably multithread this from the start
            // I should only be calling this for leaf nodes
        }
    }

    // CONSTRUCTOR
    public QuadTree(QuadNode rootNode, Vector3[] viewTriangle, Bounds tempTriBounds, int minChunkSideLength) {
        // Assign Vars
        this.rootNode = rootNode;
        this.viewTriangle = viewTriangle;
        this.tempTriBounds = tempTriBounds;

        // Construct the Quad Tree
        ConstructQuadTree(minChunkSideLength, rootNode.GetSideLength());
    }

    // GETTERS
    public QuadNode GetRootNode() { return rootNode; }

    public int GetTreeHeight() { return treeHeight; }
    // HELPERS
    public List<QuadNode> UpdateQuadTree(Vector3[] viewTriangle, Bounds triBounds) {
        // Returns a list of culled leaf nodes
        List<QuadNode> culledLeafNodes = new();

        // Update variables
        this.viewTriangle = viewTriangle;
        this.tempTriBounds = triBounds;

        // BFS and and test correctness for each node
        Queue<QuadNode> queue = new();
        queue.Enqueue(rootNode);

        while(queue.Count > 0) {
            QuadNode curr = queue.Dequeue();
            if (curr == null) continue;

            //test 1
            if (!IntersectsWithViewTri(curr) && curr.HasChildren()) {
                // Need to cull these children
                culledLeafNodes.Add(curr.botLeftChild);
                culledLeafNodes.Add(curr.botRightChild);
                culledLeafNodes.Add(curr.topLeftChild);
                culledLeafNodes.Add(curr.topRightChild);

                queue.Enqueue(curr.botLeftChild);
                queue.Enqueue(curr.botRightChild);
                queue.Enqueue(curr.topLeftChild);
                queue.Enqueue(curr.topRightChild);

                curr.topLeftChild = null;
                curr.botLeftChild = null;
                curr.topRightChild = null;
                curr.topLeftChild = null;
                treeHeight--;
            }

            // test 2
            if (IntersectsWithViewTri(curr) && !curr.HasChildren()) {
                // Need to create children for this node
                SplitNodeAndEnqueueChildren(queue, curr);
                treeHeight++;
            }
        }

        return culledLeafNodes;
    }
    public List<QuadNode> GetAllLeafNodes() {
        List<QuadNode> leafNodes = new();

        // Quick null check
        if(null == rootNode) { Debug.Log("rootNode is null. Cannot proceed."); }
        // BFS traverse the tree. If leaf node, add to array
        Queue<QuadNode> queue = new();
        queue.Enqueue(rootNode);

        while(queue.Count > 0) {
            QuadNode curr = queue.Dequeue();

            if (curr == null) continue;

            // check if a leaf node
            if(curr.botLeftChild == null
            && curr.topLeftChild == null
            && curr.botRightChild == null
            && curr.topRightChild == null) {
                leafNodes.Add(curr);
            }

            queue.Enqueue(curr.botLeftChild);
            queue.Enqueue(curr.botRightChild);
            queue.Enqueue(curr.topRightChild);
            queue.Enqueue(curr.topLeftChild);
           
        }



        return leafNodes;
    }
    private void ConstructQuadTree(int minChunkSideLength, float worldSideLength) {

        int maxHeight = 0;
        // Construct the quad tree
        Queue<QuadNode> queue = new();
        queue.Enqueue(rootNode);

        while (queue.Count > 0) {
            
            // Breadth First Search Construction of Quad Tree
            QuadNode curr = queue.Dequeue();

            // Update maxHeight
            if (curr.GetLevel() > maxHeight) maxHeight = curr.GetLevel();

            //Debug.Log($"{curr.GetBotLeftPoint()} intersects with view tri: {IntersectsWithViewTri(curr)}");

            if (IntersectsWithViewTri(curr) && (curr.GetSideLength() > minChunkSideLength)) {
                SplitNodeAndEnqueueChildren(queue, curr);
            }
        }

        treeHeight = maxHeight;
    }

    private static void SplitNodeAndEnqueueChildren(Queue<QuadNode> queue, QuadNode curr) {
        Vector2 botLeftPoint = curr.GetBotLeftPoint();
        float sideLength = curr.GetSideLength();

        QuadNode botLeft = new(botLeftPoint, 0.5f * sideLength);
        QuadNode topLeft = new(new Vector2(botLeftPoint.x, botLeftPoint.y + 0.5f * sideLength), 0.5f * sideLength);
        QuadNode topRight = new(new Vector2(botLeftPoint.x + 0.5f * sideLength, botLeftPoint.y + 0.5f * sideLength), 0.5f * sideLength);
        QuadNode botRight = new(new Vector2(botLeftPoint.x + 0.5f * sideLength, botLeftPoint.y), 0.5f * sideLength);

        botLeft.SetLevel(curr.GetLevel() + 1);
        botRight.SetLevel(curr.GetLevel() + 1);
        topLeft.SetLevel(curr.GetLevel() + 1);
        topRight.SetLevel(curr.GetLevel() + 1);

        curr.botLeftChild = botLeft;
        curr.topLeftChild = topLeft;
        curr.botRightChild = botRight;
        curr.topRightChild = topRight;

        queue.Enqueue(botLeft);
        queue.Enqueue(topLeft);
        queue.Enqueue(topRight);
        queue.Enqueue(botRight);
    }

    private bool IntersectsWithViewTri(QuadNode node) {
        // Test performed using Separating Axis Theorem
        // More info: https://dyn4j.org/2010/01/sat/

        // TODO: COMPLETE SAT IMPLEMENTATION

        // Get 3 points from the view triangle
        Vector3[] triPoints = viewTriangle;

        // Get all 4 points of the node 
        Vector3[] nodePoints = new Vector3[4];

        Vector3 bl = node.GetBotLeftPoint();
        float l = node.GetSideLength();

        // constructed clockwise from bot left point
        nodePoints[0] = bl;
        nodePoints[1] = new Vector3(bl.x, 0f, bl.z + l);
        nodePoints[2] = new Vector3(bl.x + l, 0f, bl.z + l);
        nodePoints[3] = new Vector3(bl.x + l, 0f, bl.z); 

        // Use SAT to check for an intersection between the viewTri and Node

        /* TO DO
         * 1. get tri axes and squ axes from points
         * 2. for each axis, proj tri and squ onto it and get tri-max, tri-min, squ-max, squ-min
         * 3. perform 1D collision test (see GoodNotes) for each axis
         * 
         * if no collision, return False else return True.
         * maybe can optimise instead of n^2, log(n)
        */

        // Below is temporary
        Bounds nodeBounds = node.GetBounds();
        return nodeBounds.Intersects(tempTriBounds);
    }
    public void SaveTree(ref List<Bounds> boundsToDraw) {
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

