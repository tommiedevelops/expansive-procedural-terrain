using UnityEngine;
using System.Collections.Generic;
using static TerrainGenQuadTree;

public class QuadTreeTester : MonoBehaviour {
    [SerializeField] Camera cam;
    [SerializeField] float renderDistance;
    [SerializeField] int minChunkSideLength;

    List<Bounds> boundsToDraw = new();
    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        foreach (Bounds bounds in boundsToDraw) {
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }
    }

    private void Awake() {
        TerrainGenQuadTree tree = new(cam, renderDistance, minChunkSideLength);
        tree.PrintTree(ref boundsToDraw);
    }

}
;