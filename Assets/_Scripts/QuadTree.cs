using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEditor.Rendering;
using Unity.Jobs;
using System;
using System.Linq;
public class QuadTree {

    // CONSTRUCTOR
    public QuadTree(QuadNode rootNode, Vector3[] viewTriangle, Bounds tempTriBounds, int minChunkSideLength) {
        // Assign Vars
        this.rootNode = rootNode;
        this.viewTriangle = viewTriangle;
        this.tempTriBounds = tempTriBounds;
        this.minChunkSideLength = minChunkSideLength;

        // Construct the Quad Tree
        ConstructQuadTree(minChunkSideLength, rootNode.GetSideLength());
    }

    #region Fields
    QuadNode rootNode;
    Vector3[] viewTriangle;
    Bounds tempTriBounds;

    int minChunkSideLength;
    int treeHeight;
    private const int maxLOD = 7;

    #endregion
    
    #region Getters & Setters
    // GETTERS
    public QuadNode GetRootNode() { return rootNode; }
    public int GetTreeHeight() { return treeHeight; }
    #endregion

    #region Helper Functions
    // HELPERS
    public List<uint> UpdateQuadTree(Vector3[] viewTriangle, Bounds triBounds) {
        /* Function returns a list of leaf nodes that have been culled in this frame
         */

        // Returns a list of culled leaf nodes
        List<QuadNode> culledNodes = new();

        // Update variables
        this.viewTriangle = viewTriangle;
        this.tempTriBounds = triBounds;

        // BFS to detect culled nodes and split nodes
        Queue<QuadNode> queue = new();
        queue.Enqueue(rootNode);
        List<uint> culledLeafNodeHashes = new();

        while (queue.Count > 0) {
            QuadNode curr = queue.Dequeue();

            if(IntersectsWithViewTri(curr) && (curr.GetSideLength() > minChunkSideLength)) {
                if (!curr.HasChildren()) SplitNode(curr);
                EnqueueChildren(queue,curr);
            } else {
                if (!curr.HasChildren()) culledLeafNodeHashes.Add(curr.ComputeHash()); // leaf node
                else EnqueueChildren(queue, curr);
            }
        }
       
        return culledLeafNodeHashes;
    }
    public List<QuadNode> GetAllLeafNodes(QuadNode root) {
        List<QuadNode> leafNodes = new();

        // Quick null check
        if(null == root) { Debug.Log("root is null. Cannot proceed."); }
        // BFS traverse the tree. If leaf node, add to array
        Queue<QuadNode> queue = new();
        queue.Enqueue(root);

        while(queue.Count > 0) {
            QuadNode curr = queue.Dequeue();

            if (curr == null) continue;

            // check if a leaf node
            if(!curr.HasChildren()) {
                leafNodes.Add(curr);
            }

            foreach(QuadNode child in curr.GetChildren()) {
                queue.Enqueue(child);
            }
           
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
                SplitNode(curr);
                EnqueueChildren(queue, curr);
            }
        }

        treeHeight = maxHeight;
    }
    private void SplitNode(QuadNode curr) {
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

            foreach (QuadNode child in curr.GetChildren())
                if (child != null) queue.Enqueue(child);

            boundsToDraw.Add(curr.GetBounds());
        }
    }
    #endregion

}

