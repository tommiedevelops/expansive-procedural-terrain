using UnityEngine;
using System.Collections.Generic;
using static TerrainGenQuadTree;

public class QuadTreeTester : MonoBehaviour {
    [SerializeField] Camera cam;
    [SerializeField] float renderDistance;
    [SerializeField] int minChunkSideLength;

    List<Bounds> boundsToDraw = new();
    TerrainGenQuadTree tree;
    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        foreach (Bounds bounds in boundsToDraw) {
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(tree.ComputeTriBounds().center, tree.ComputeTriBounds().size);

        //Gizmos.color = Color.blue;
        //Gizmos.DrawLine(cam.transform.position, cam.transform.position + cam.transform.forward * 10f);
    }

    private void Awake() {
        TerrainGenQuadTree tree = new(cam, renderDistance, minChunkSideLength);
        this.tree = tree;
        tree.PrintTree(ref boundsToDraw);
    }

}
;