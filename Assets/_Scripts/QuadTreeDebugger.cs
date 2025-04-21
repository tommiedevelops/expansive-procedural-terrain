using System.Collections.Generic;
using UnityEngine;

public class QuadTreeDebugger : MonoBehaviour {
    [SerializeField] QTViewer viewer;
    [SerializeField] TerrainManager manager;
    QuadTree quadTree;

     // For Debugging
    List<Bounds> culledBounds = new();

    private void Start() {
        quadTree = manager.GetQuadTree();
    }
    private void OnDrawGizmos() {
        if (!Application.isPlaying) return;
        GizmosDrawViewTriangleAndTriBounds();
        GizmosDrawNodeSquares();
    }
    void GizmosDrawCulledNodes() {
        Gizmos.color = Color.red;
        foreach (Bounds bounds in culledBounds) {
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }
    }
    private void GizmosDrawNodeSquares() {
        List<Bounds> boundsToDraw = new();
        Gizmos.color = Color.green;
        Queue<QuadNode> queue = new();
        queue.Enqueue(quadTree.GetRootNode());
        
        while(queue.Count > 0) {
            QuadNode curr = queue.Dequeue();
            if (null == curr) continue;
            boundsToDraw.Add(curr.GetBounds());
            foreach(QuadNode child in curr.GetChildren()) {
                queue.Enqueue(child);
            }
        }

        foreach (Bounds bounds in boundsToDraw) {
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }
    }
    private void GizmosDrawViewTriangleAndTriBounds() {
        Vector3[] viewTriangle = viewer.GetViewTriangle();

        if (viewTriangle == null || viewTriangle.Length == 0) return;
        Gizmos.DrawLine(viewTriangle[0], viewTriangle[1]);
        Gizmos.DrawLine(viewTriangle[1], viewTriangle[2]);
        Gizmos.DrawLine(viewTriangle[0], viewTriangle[2]);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(viewer.GetTriBounds().center, viewer.GetTriBounds().size);
    }
    public void DrawTreeForDebugging(ref List<Bounds> boundsToDraw) {

        boundsToDraw = new();

        Queue<QuadNode> queue = new();
        queue.Enqueue(quadTree.GetRootNode());
        while (queue.Count > 0) {
            QuadNode curr = queue.Dequeue();

            foreach (QuadNode child in curr.GetChildren())
                if (child != null) queue.Enqueue(child);

            boundsToDraw.Add(curr.GetBounds());
        }
    }
}