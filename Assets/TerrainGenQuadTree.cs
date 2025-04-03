using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Linq;
using System.Runtime.CompilerServices;
public class TerrainGenQuadTree {
    Vector3[] viewTriangle;
    Bounds triBounds;
    float renderDistance;
    Camera cam;

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

            Vector3 boundsCenter = new(botLeftPoint.x, 0f, botLeftPoint.y);
            Vector3 boundsDimensions = new(sideLength, 0f, sideLength);

            this.bounds = new Bounds(boundsCenter, boundsDimensions);

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
        this.cam = cam;
        this.renderDistance = renderDistance;
        this.viewTriangle = GetViewTriangleFromCamera(cam, renderDistance);
        this.rootNode = CreateRootNode(cam, renderDistance);
        this.triBounds = ComputeTriBounds();
        
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

        Vector3 camPos = viewTriangle[0];
        Vector3 leftPoint = viewTriangle[1];
        Vector3 rightPoint = viewTriangle[2];

        Vector3 halfPoint = leftPoint + 0.5f*(rightPoint - leftPoint);
        Vector3 median = halfPoint - camPos;

        Debug.Log($"halfpoint:{halfPoint}");
        Debug.Log(median);

        Vector3 boundsDimensions = new(renderDistance, 0f, renderDistance);

        Bounds triBounds = new(camPos + 0.5f*median, boundsDimensions); // can approx better by using isoceles properties
        return triBounds;
    }
    private bool IntersectsWithViewTri(Node node) {
        // Test performed using Separating Axis Theorem
        // More info: https://dyn4j.org/2010/01/sat/
        
        Bounds nodeBounds = node.GetBounds();
        return nodeBounds.Intersects(triBounds);
    }

    private Vector2 ComputeBotLeftPoint(Camera cam, float renderDistance) {
        var botLeftPointV3 = cam.transform.position - new Vector3(renderDistance, 0f, renderDistance);
        var botLeftPointV2 = new Vector2(botLeftPointV3.x, botLeftPointV3.z);
        return botLeftPointV2;
    }
    private Vector3[] GetViewTriangleFromCamera(Camera cam, float renderDistance) {
        /* Calculates the view triangle from camera position and render distance */
        /* This calculation only cares about the XZ plane */

        // 3D Coords
        Vector3 camPos = cam.transform.position; //world
        Vector3 camForward = cam.transform.forward; 
        Vector3 camRight = cam.transform.right;

        // Projected onto XZ plane
        Vector3 camPosXZ = new(camPos.x, 0f, camPos.z);
        Vector3 camForwardXZ = new(camForward.x, 0f, camForward.z);
        Vector3 camRightXZ = new(camRight.x, 0f, camRight.z);

        // Normalize directions
        camForwardXZ = camForwardXZ.normalized;
        camRightXZ = camRightXZ.normalized;

        // Get the base width of the view triangle
        float FOVAngle = cam.fieldOfView;
        float halfAngle = (float)FOVAngle / 2;
        float halfWidth = renderDistance * Mathf.Tan(DegToRad(halfAngle));

        Vector3 leftPoint = camPosXZ + camForwardXZ * renderDistance - camRightXZ * halfWidth;
        Vector3 rightPoint = camPosXZ + camForwardXZ * renderDistance + camRightXZ * halfWidth;

        Vector3[] triangle = { camPosXZ, leftPoint, rightPoint }; //all in world coords

        this.viewTriangle = triangle;
        return triangle;

    }
    public Vector3[] GetViewTriangle() { return viewTriangle; }
    float DegToRad(float angleInDeg) { return angleInDeg * Mathf.PI / 180f;  }
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

