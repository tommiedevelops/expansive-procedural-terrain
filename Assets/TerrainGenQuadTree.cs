using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Linq;
using System.Runtime.CompilerServices;
public class TerrainGenQuadTree {
    Vector3[] viewTriangle;

    Node rootNode;
    public class Node {
        /* Nodes only work in metres NOT in chunk units */
        Vector2 botLeftPoint;
        float sideLength; // in metres
        public Node botLeftChild, topLeftChild, topRightChild, botRightChild;
        Bounds bounds;

        public Node(Vector2 botLeftPoint, float sideLength) {
            this.botLeftPoint = botLeftPoint;
            this.sideLength = sideLength;
            this.bounds = new Bounds(botLeftPoint + new Vector2(0.5f * sideLength, 0.5f * sideLength), Vector3.one * sideLength);

            this.botLeftChild = null;
            this.botRightChild = null;
            this.topRightChild = null;
            this.topLeftChild = null;
        }

        public Bounds GetBounds() { return bounds; }
        public float GetSideLength() { return sideLength; }
        public Vector2 GetBotLeftPoint() { return botLeftPoint; }

    }

    // CONSTRUCTOR

    private Node CreateRootNode(Camera cam, float renderDistance) {
        return new(ComputeBotLeftPoint(cam, renderDistance), renderDistance * 2);
    }

    public TerrainGenQuadTree(Camera cam, float renderDistance, int minChunkSideLength) {
        // Assign Vars
        this.viewTriangle = GetViewTriangleFromCamera(cam, renderDistance);
        this.rootNode = CreateRootNode(cam, renderDistance);
        ConstructQuadTree(minChunkSideLength);
    }

    private void ConstructQuadTree(int minChunkSideLength) {
        // Construct the quad tree
        Queue<Node> queue = new();
        queue.Enqueue(rootNode);

        while (queue.Count > 0) {

            // Breadth First Search Construction of Quad Tree
            Node curr = queue.Dequeue();

            //Debug.Log($"{curr.GetBotLeftPoint()} intersects with view tri: {IntersectsWithViewTri(curr)}");

            if (IntersectsWithViewTri(curr) && (curr.GetSideLength() > minChunkSideLength)) {
                Vector2 botLeftPoint = curr.GetBotLeftPoint();
                float sideLength = curr.GetSideLength();
                Node botLeft = new(botLeftPoint, 0.5f * sideLength);
                Node topLeft = new(new Vector2(botLeftPoint.x, botLeftPoint.y + 0.5f * sideLength), 0.5f * sideLength);
                Node topRight = new(new Vector2(botLeftPoint.x + 0.5f * sideLength, botLeftPoint.y + 0.5f * sideLength), 0.5f * sideLength);
                Node botRight = new(new Vector2(botLeftPoint.x + 0.5f * sideLength, botLeftPoint.y), 0.5f * sideLength);

                curr.botLeftChild = botLeft;
                curr.topLeftChild = topLeft;
                curr.botRightChild = botRight;
                curr.topRightChild = topRight;

                queue.Enqueue(botLeft);
                queue.Enqueue(topLeft);
                queue.Enqueue(topRight);
                queue.Enqueue(botRight);
            }
        }
    }

    // GETTERS
    public Node GetRootNode() { return rootNode; }
    // HELPERS

    public Bounds ComputeTriBounds() {
        // approximate triangle as rectangle for now
        Vector3 camOrigin = viewTriangle[0];
        Vector3 halfPoint = 0.5f*(viewTriangle[2] - viewTriangle[1]);
        Vector3 triBoundsCenter = camOrigin + 0.5f * (halfPoint - camOrigin);
        Bounds triBounds = new(triBoundsCenter, Vector3.one * (halfPoint-camOrigin).magnitude); // can approx better by using isoceles properties
        return triBounds;
    }
    private bool IntersectsWithViewTri(Node node) {
        // Test performed using Separating Axis Theorem
        // More info: https://dyn4j.org/2010/01/sat/
        
        Bounds nodeBounds = node.GetBounds();
        return nodeBounds.Intersects(ComputeTriBounds());
    }

    private Vector2 ComputeBotLeftPoint(Camera cam, float renderDistance) {
        var botLeftPointV3 = cam.transform.position - new Vector3(renderDistance, 0f, renderDistance);
        var botLeftPointV2 = new Vector2(botLeftPointV3.x, botLeftPointV3.z);
        return botLeftPointV2;
    }
    private Vector3[] GetViewTriangleFromCamera(Camera cam, float renderDistance) {
        /* Calculates the view triangle from camera position and render distance */
        Vector3 camPos = cam.transform.position; //world
        Vector3 camForward = cam.transform.forward; 
        Vector3 camRight = cam.transform.right;

        // Get the base width of the view triangle
        float FOVAngle = cam.fieldOfView;
        float halfAngle = (float)FOVAngle / 2;
        float halfWidth = 2 * renderDistance * Mathf.Tan(halfAngle);

        Vector3 leftPoint = camPos + camForward * renderDistance - camRight * halfWidth;
        Vector3 rightPoint = camPos + camForward * renderDistance + camRight * halfWidth;

        Vector3[] triangle = { camPos, leftPoint, rightPoint }; //all in world coords

        this.viewTriangle = triangle;
        return triangle;

    }

    public void PrintTree(ref List<Bounds> boundsToDraw) {
        Queue<Node> queue = new();
        queue.Enqueue(rootNode);
        while(queue.Count > 0) {
            Node curr = queue.Dequeue();

            if (curr.botLeftChild != null) queue.Enqueue(curr.botLeftChild);
            if (curr.botRightChild != null) queue.Enqueue(curr.botRightChild);
            if (curr.topLeftChild != null) queue.Enqueue(curr.topLeftChild);
            if (curr.topRightChild != null) queue.Enqueue(curr.topRightChild);

            Debug.Log($"{curr.GetBotLeftPoint()}, {curr.GetSideLength()}");
            boundsToDraw.Add(curr.GetBounds());
        }
    }
}

