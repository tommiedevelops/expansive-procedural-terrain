using UnityEngine;
using System.Collections.Generic;
using System.Data;
public class QuadTree {

    #region Fields
    readonly QuadNode rootNode;
    readonly QTViewer viewer;

    public const int MIN_CHUNK_SIZE = 120;
    int treeHeight;
    private const int maxLOD = 7;

    // Debugging
    List<QuadNode> recentlyCulledNodes;
    #endregion

    public QuadTree(QuadNode rootNode, QTViewer viewer) {
        // Assign Vars
        this.rootNode = rootNode;
        this.viewer = viewer;

        // Construct the Quad Tree
        ConstructQuadTree(MIN_CHUNK_SIZE, rootNode.GetSideLength());
    }

    #region Getters & Setters
    public QuadNode GetRootNode() { return rootNode; }
    public int GetTreeHeight() { return treeHeight; }
    #endregion

    #region Helper Functions
    public List<uint> Update(Vector3[] viewTriangle, Bounds triBounds) {

        // BFS to detect culled nodes and split nodes
        Queue<QuadNode> queue = new();
        queue.Enqueue(rootNode);

        List<QuadNode> nodesToCull = new();

        // Detect nodes to cull and split
        while (queue.Count > 0) {
            QuadNode curr = queue.Dequeue();
            if (null == curr) continue;

            if(!IntersectsWithViewTri(curr) && !curr.IsLeafNode()) {
                // Add node to nodesToCull
                nodesToCull.Add(curr);
                // Create a new node to replace it
                QuadNode newNode = new(curr.GetParent(), curr.GetBotLeftPoint(), curr.GetSideLength());
                // Replace the node
                curr.GetParent().ReplaceChild(curr, newNode);
            }

            if(IntersectsWithViewTri(curr) && curr.IsLeafNode() && curr.GetSideLength() > MIN_CHUNK_SIZE) {
                SplitNode(curr);
                EnqueueChildren(queue,curr);
            }

        }
        
        List<uint> culledLeafNodeHashes = new();

        foreach (QuadNode node in nodesToCull) {
            var leafNodes = node.GetAllLeafNodes();
            recentlyCulledNodes.AddRange(leafNodes); // Debugging
            foreach(QuadNode leafNode in leafNodes) { culledLeafNodeHashes.Add(leafNode.ComputeHash()); }
        }

        return culledLeafNodeHashes;
    }
    private void ConstructQuadTree(int minChunkSideLength, float worldSideLength) {

        int maxHeight = 0;
        Queue<QuadNode> queue = new();
        queue.Enqueue(rootNode);

        while (queue.Count > 0) {
            QuadNode curr = queue.Dequeue();
            if (null == curr) continue;

            if (curr.GetLevel() > maxHeight) maxHeight = curr.GetLevel();

            if (IntersectsWithViewTri(curr) && (curr.GetSideLength() > minChunkSideLength)) {
                SplitNode(curr);
                EnqueueChildren(queue, curr);
            }
        }

        treeHeight = maxHeight;
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
    private bool IntersectsWithViewTri(QuadNode node) {
        // Test performed using Separating Axis Theorem
        // More info: https://dyn4j.org/2010/01/sat/

        // TODO: COMPLETE SAT IMPLEMENTATION

        // Get 3 points from the view triangle
        Vector3[] triPoints = viewer.GetViewTriangle();

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
        return nodeBounds.Intersects(viewer.GetTriBounds());
    }
    public void DrawCulledNodesForDebugging(ref List<Bounds> culledBounds) {
        foreach(QuadNode node in recentlyCulledNodes) { culledBounds.Add(node.GetBounds()); }
    }
    public void DrawTreeForDebugging(ref List<Bounds> boundsToDraw) {
        Queue<QuadNode> queue = new();
        queue.Enqueue(rootNode);
        while(queue.Count > 0) {
            QuadNode curr = queue.Dequeue();

            foreach (QuadNode child in curr.GetChildren())
                if (child != null) queue.Enqueue(child);

            boundsToDraw.Add(curr.GetBounds());
        }
    }
    #endregion

}

