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
        Gizmos.color = Color.green;
        foreach (Bounds bounds in boundsToDraw) {
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }

        if (tree == null) return;
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

    private void Awake() {
        // create a QuadNode first and set the side length
        // define a min chunk side length
        
        int rootNodeSideLength = minChunkSideLength * rootNodeSideLengthMultiplier;
       
        QuadNode rootNode = new(viewer.GetBotLeftPoint(), rootNodeSideLength * 2);
        QuadTree tree = new(rootNode, viewer, minChunkSideLength);
        this.tree = tree;

    }

    private void Start() {
        tree.PrintTree(ref boundsToDraw);
    }

}
