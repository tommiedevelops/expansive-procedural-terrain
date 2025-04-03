using UnityEngine;
using System.Collections.Generic;
using static TerrainGenQuadTree;

public class QuadTreeTester : MonoBehaviour {
    [SerializeField] Camera cam;
    [SerializeField] float renderDistance;
    [SerializeField] int minChunkSideLength;
    [SerializeField] float worldSideLength;

    List<Bounds> boundsToDraw = new();
    TerrainGenQuadTree tree;
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
        TerrainGenQuadTree tree = new(cam, renderDistance, minChunkSideLength, worldSideLength);
        this.tree = tree;
        tree.PrintTree(ref boundsToDraw);
    }

}
;