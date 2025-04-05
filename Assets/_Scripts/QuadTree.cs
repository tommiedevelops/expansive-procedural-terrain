using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Linq;
using System.Runtime.CompilerServices;
public class QuadTree {
    #region Fields
    Vector3[] viewTriangle;
    Bounds triBounds;
    float renderDistance;
    Camera cam;
    QuadNode rootNode;
    #endregion  

    // CHILD CLASSES
    public class QuadNode {
        /* Nodes only work in metres NOT in chunk units */
        Vector2 botLeftPoint;
        float sideLength; // in metres
        public QuadNode botLeftChild, topLeftChild, topRightChild, botRightChild;
        Bounds bounds;

        // CONSTRUCTOR
        public QuadNode(Vector2 botLeftPoint, float sideLength) {
            this.botLeftPoint = botLeftPoint;
            this.sideLength = sideLength;

            Vector3 boundsCenter = new(botLeftPoint.x + sideLength / 2f, 0f, botLeftPoint.y + sideLength / 2f);
            Vector3 boundsDimensions = new(sideLength, 0f, sideLength);

            this.bounds = new Bounds(boundsCenter, boundsDimensions);

            this.botLeftChild = null;
            this.botRightChild = null;
            this.topRightChild = null;
            this.topLeftChild = null;
        }

        // HELPERS
        public Bounds GetBounds() { return bounds; }
        public float GetSideLength() { return sideLength; }
        public Vector2 GetBotLeftPoint() { return botLeftPoint; }

    }

    // CONSTRUCTOR
    public QuadTree(Camera cam, float renderDistance, int minChunkSideLength, float rootNodeSideLength) {
        // Assign Vars
        this.cam = cam;
        this.renderDistance = renderDistance;
        this.viewTriangle = GetViewTriangleFromCamera(cam, renderDistance);
        this.rootNode = CreateRootNode(cam, rootNodeSideLength);
        this.triBounds = ComputeTriBounds();
        
        ConstructQuadTree(minChunkSideLength, rootNodeSideLength);
    }

    // GETTERS
    public QuadNode GetRootNode() { return rootNode; }

    // HELPERS
    private void ConstructQuadTree(int minChunkSideLength, float worldSideLength) {
        // Construct the quad tree
        Queue<QuadNode> queue = new();
        queue.Enqueue(rootNode);
        while (queue.Count > 0) {

            // Breadth First Search Construction of Quad Tree
            QuadNode curr = queue.Dequeue();

            //Debug.Log($"{curr.GetBotLeftPoint()} intersects with view tri: {IntersectsWithViewTri(curr)}");

            if (IntersectsWithViewTri(curr) && (curr.GetSideLength() > minChunkSideLength)) {
                Vector2 botLeftPoint = curr.GetBotLeftPoint();
                float sideLength = curr.GetSideLength();
                QuadNode botLeft = new(botLeftPoint, 0.5f * sideLength);
                QuadNode topLeft = new(new Vector2(botLeftPoint.x, botLeftPoint.y + 0.5f * sideLength), 0.5f * sideLength);
                QuadNode topRight = new(new Vector2(botLeftPoint.x + 0.5f * sideLength, botLeftPoint.y + 0.5f * sideLength), 0.5f * sideLength);
                QuadNode botRight = new(new Vector2(botLeftPoint.x + 0.5f * sideLength, botLeftPoint.y), 0.5f * sideLength);

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
    private QuadNode CreateRootNode(Camera cam, float worldSideLength) {
        return new(ComputeBotLeftPoint(cam, worldSideLength), worldSideLength * 2);
    }
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
    private bool IntersectsWithViewTri(QuadNode node) {
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
    public Bounds GetTriBounds() { return triBounds; }
    public void PrintTree(ref List<Bounds> boundsToDraw) {
        Queue<QuadNode> queue = new();
        queue.Enqueue(rootNode);
        while(queue.Count > 0) {
            QuadNode curr = queue.Dequeue();

            if (curr.botLeftChild != null) queue.Enqueue(curr.botLeftChild);
            if (curr.botRightChild != null) queue.Enqueue(curr.botRightChild);
            if (curr.topLeftChild != null) queue.Enqueue(curr.topLeftChild);
            if (curr.topRightChild != null) queue.Enqueue(curr.topRightChild);

            Debug.Log($"{curr.GetBotLeftPoint()}, {curr.GetSideLength()}");
            boundsToDraw.Add(curr.GetBounds());
        }
    }
}

