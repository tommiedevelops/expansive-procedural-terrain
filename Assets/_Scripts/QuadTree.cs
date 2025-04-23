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
    public List<uint> Update() {

        Bounds updatedTriBounds = viewer.GetTriBounds();

        // BFS to detect culled nodes and split nodes
        Queue<QuadNode> queue = new();
        queue.Enqueue(rootNode);

        List<QuadNode> culledNodes = new();

        // Detect nodes to cull and split
        while (queue.Count > 0) {
            QuadNode curr = queue.Dequeue();
            if (null == curr) continue;

            if(!curr.IsLeafNode() && curr.IntersectsWithViewTri(updatedTriBounds)) {
                EnqueueChildren(queue, curr);
                continue;
            }

            if(!curr.IsLeafNode() && !curr.IntersectsWithViewTri(updatedTriBounds)) {
                // turn this bad boy into a leaf node
                culledNodes.AddRange(curr.GetAllLeafNodes());
                curr.ClearChildren();
                continue;
            }

            if (curr.IsLeafNode() && curr.IntersectsWithViewTri(updatedTriBounds)) {
                if (curr.GetSideLength() > MIN_CHUNK_SIZE) {
                    SplitNode(curr);
                    EnqueueChildren(queue, curr);
                }
                continue;
            }

            if(curr.IsLeafNode() && !curr.IntersectsWithViewTri(updatedTriBounds)) {
                continue;
            }

            throw new System.Exception("The code shouldn't reach this point");         
        }
        
        List<uint> culledLeafNodeHashes = new();
        foreach(QuadNode culledNode in culledNodes) {
            uint hash = culledNode.ComputeHash();
            culledLeafNodeHashes.Add(hash);
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

            if (curr.IntersectsWithViewTri(viewer.GetTriBounds()) && (curr.GetSideLength() > minChunkSideLength)) {
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

    #endregion

}
