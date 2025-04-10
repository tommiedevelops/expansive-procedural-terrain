using UnityEngine;
using Unity.Mathematics;
using System.Collections.Generic;
using static QuadTree;
using UnityEngine.AI;

public class QuadTreeTester : MonoBehaviour {

    // Behaves like the main function of the program for now
    const int minChunkSideLength = 240; // this shouldn't change
    
    [SerializeField] int rootNodeSideLengthMultiplier; // this should always be a multiple of the minChunkSideLength
    [SerializeField] QTViewer viewer;

    QuadNode rootNode;

    List<Bounds> boundsToDraw = new();
    QuadTree tree;
    List<QuadNode> leafNodes; // For testing
    private void OnDrawGizmos() {
        if (tree == null) return;

        // Draw each node in the QuadTree
        GizmosDrawNodeSquares();
        GizmosDrawTriBounds();
        GizmosDrawViewTriangle();
        //GizmosDrawLeafNodes();
    }

    // Gizmos functions
    private void GizmosDrawNodeSquares() {
        Gizmos.color = Color.green;
        foreach (Bounds bounds in boundsToDraw) {
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }
    }
    private void GizmosDrawTriBounds() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(viewer.GetTriBounds().center, viewer.GetTriBounds().size);
    }
    private void GizmosDrawViewTriangle() {
        Vector3[] viewTriangle = viewer.GetViewTriangle();
        Gizmos.DrawLine(viewTriangle[0], viewTriangle[1]);
        Gizmos.DrawLine(viewTriangle[1], viewTriangle[2]);
        Gizmos.DrawLine(viewTriangle[0], viewTriangle[2]);
    }
    private void GizmosDrawLeafNodes() {
        Gizmos.color = Color.red;
        foreach (QuadNode node in leafNodes) {
            Debug.Log($"Bot left point: {node.GetBotLeftPoint()}");
            Gizmos.DrawSphere(new Vector3(node.GetBotLeftPoint().x, 0f, node.GetBotLeftPoint().y), 10f);
        }
    }


    private void Start() {
        // create a QuadNode first and set the side length
        // define a min chunk side length
        
        int rootNodeSideLength = minChunkSideLength * rootNodeSideLengthMultiplier;

        // The first rootNode should be at x=0, z=0
        //Debug.Log($"The side length of root nodes is: {rootNodeSideLength}");

        // initially, position of viewer is in the bottom left quadrant of the first chunk
        rootNode = new(new Vector2(-1f*rootNodeSideLength/2, -1f*rootNodeSideLength/2), rootNodeSideLength * 2);

        QuadTree tree = new(rootNode, viewer, minChunkSideLength);
        this.tree = tree;
        tree.SaveTree(ref boundsToDraw);

        leafNodes = tree.GetAllLeafNodes();

    }
 
    Dictionary<uint, QuadChunk> chunkDictionary = new();
    private void Update() {

        // Compute quad tree based on viewer position and orientation
        QuadTree tree = new(rootNode, viewer, minChunkSideLength);
        this.tree = tree;
        tree.SaveTree(ref boundsToDraw);

        // Get all leaf nodes of the quad tree
        leafNodes = tree.GetAllLeafNodes();

        foreach(QuadNode leafNode in leafNodes) {
            // Compute the hash
            uint leafNodeHash = leafNode.ComputeHash();

            // Check if hash exists
            if(chunkDictionary.TryGetValue(leafNodeHash, out QuadChunk chunk)) {
                // Chunk Exists already
            } else {
                // Chunk does not exist yet

                // Create the chunk

                // LOD will depend on ratio between sideLength and rootNodeSideLength
                float lodConstant = (float) leafNode.GetSideLength() / rootNode.GetSideLength();

                QuadChunk newChunk = new(leafNode.GetBotLeftPoint(), lodConstant, leafNode.GetSideLength());
                GameObject chunkObject = newChunk.RenderChunk();

            }
        }

    }

}
