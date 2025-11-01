using System.Collections.Generic;
using UnityEngine;
using TerrainGenerator.QuadTreeSystem;

namespace TerrainGenerator
{
    public class QuadTreeDebugger : MonoBehaviour {
        
        //[SerializeField] TerrainGenerator  _terrainGenerator;
        private QuadTree _quadTree;
        private QTViewer _viewer;
        private void Start() {
       //     _quadTree = terrainGenerator.GetQuadTree();
        //    _viewer = terrainGenerator.GetViewer();
        }
        private void OnDrawGizmos() {
            if (!Application.isPlaying) return;
            GizmosDrawViewTriangleAndTriBounds();
            GizmosDrawNodeSquares();
        }
        private void GizmosDrawNodeSquares() {
            List<Bounds> boundsToDraw = new();
            Gizmos.color = Color.green;
            Queue<QuadNode> queue = new();
            queue.Enqueue(_quadTree.GetRootNode());

            while (queue.Count > 0) {
                QuadNode curr = queue.Dequeue();
                if (null == curr) continue;
                boundsToDraw.Add(curr.GetBounds());
                foreach (QuadNode child in curr.GetChildren()) {
                    queue.Enqueue(child);
                }
            }

            foreach (Bounds bounds in boundsToDraw) {
                Gizmos.DrawWireCube(bounds.center, bounds.size);
            }
        }
        private void GizmosDrawViewTriangleAndTriBounds() {
            var viewTriangle = _viewer.GetViewTriangle();

            if (viewTriangle == null || viewTriangle.Length == 0) return;
            Gizmos.DrawLine(viewTriangle[0], viewTriangle[1]);
            Gizmos.DrawLine(viewTriangle[1], viewTriangle[2]);
            Gizmos.DrawLine(viewTriangle[0], viewTriangle[2]);
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(_viewer.GetTriBounds().center, _viewer.GetTriBounds().size);
        }
        public void DrawTreeForDebugging(ref List<Bounds> boundsToDraw) {

            boundsToDraw = new();

            Queue<QuadNode> queue = new();
            queue.Enqueue(_quadTree.GetRootNode());
            while (queue.Count > 0) {
                QuadNode curr = queue.Dequeue();

                foreach (QuadNode child in curr.GetChildren())
                    if (child != null) queue.Enqueue(child);

                boundsToDraw.Add(curr.GetBounds());
            }
        }
    }
}