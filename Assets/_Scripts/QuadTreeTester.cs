using UnityEngine;
using System.Collections.Generic;
using static QuadTree;

public class QuadTreeTester : MonoBehaviour {

    // Behaves like the main function of the program for now
    const int minChunkSideLength = 240; // this shouldn't change
    
    [SerializeField] int rootNodeSideLengthMultiplier; // this should always be a multiple of the minChunkSideLength
    [SerializeField] QTViewer viewer;

    List<Bounds> boundsToDraw = new();
    QuadTree tree;
    private void OnDrawGizmos() {

        // Draw each node in the QuadTree
        Gizmos.color = Color.green;
        foreach (Bounds bounds in boundsToDraw) {
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }

        if (tree == null) return;

        // Draw TriBounds
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(tree.GetTriBounds().center, tree.GetTriBounds().size);

        // Draw Triangle
        Vector3[] viewTriangle = tree.GetViewTriangle();
        Gizmos.DrawLine(viewTriangle[0], viewTriangle[1]);
        Gizmos.DrawLine(viewTriangle[1], viewTriangle[2]);
        Gizmos.DrawLine(viewTriangle[0], viewTriangle[2]);

        //Gizmos.color = Color.blue;
        //Gizmos.DrawLine(cam.transform.position, cam.transform.position + cam.transform.forward * 10f);

    }

    private void Start() {
        // create a QuadNode first and set the side length
        // define a min chunk side length
        
        int rootNodeSideLength = minChunkSideLength * rootNodeSideLengthMultiplier;

        // The first rootNode should be at x=0, z=0
        Debug.Log($"The side length of root nodes is: {rootNodeSideLength}");

        // initially, position of viewer is in the bottom left quadrant of the first chunk
        QuadNode rootNode = new(new Vector2(-1f*rootNodeSideLength/2, -1f*rootNodeSideLength/2), rootNodeSideLength * 2);

        QuadTree tree = new(rootNode, viewer, minChunkSideLength);
        this.tree = tree;
        tree.PrintTree(ref boundsToDraw);

    }

}
