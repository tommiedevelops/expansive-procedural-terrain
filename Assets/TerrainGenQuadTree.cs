using UnityEngine;
using System.Collections.Generic;
public class TerrainGenQuadTree {
    Vector3[] viewTriangle;
    private float renderDistance;
    private float minChunkSize;
    Camera cam;

    Node rootNode;
    private class Node {
        /* Nodes only work in metres NOT in chunk units */
        Vector2 botLeftPoint;
        float sideLength; // in metres
        Node[] children; // {0,1,2,3} => {BR, TR, TL, BL}
        Bounds bounds;

        public Node(Vector2 botLeftPoint, float sideLength) {
            this.botLeftPoint = botLeftPoint;
            this.sideLength = sideLength;
            this.bounds = new Bounds(botLeftPoint + new Vector2(0.5f * sideLength, 0.5f * sideLength), Vector3.one * sideLength);
            this.children = new Node[4];
        }

        public Bounds GetBounds() { return bounds; }
    }

//    queue<node> = [root_node]

//    while(queue not empty)
//    curr = queue.pop()
//    if(curr intersects view triangle & is greater than min size)
//        create 4 new nodes, set them to be children of curr and add to queue
    
    // CONSTRUCTOR
    public TerrainGenQuadTree(Camera cam, float renderDistance, float minChunkSize) {
        // Assign Vars
        this.renderDistance = renderDistance;
        this.minChunkSize = minChunkSize;
        this.cam = cam;

        // Construct the quad tree
        rootNode = new Node(ComputeBotLeftPoint(), renderDistance * 2);
        Queue<Node> queue = new();
        queue.Enqueue(rootNode);

        while(queue.Count > 0) {
            Node curr = queue.Dequeue();
            //TODO
            break;
        }

    }

    private bool IntersectsWithViewTri(Node node) {
        // Test performed using Separating Axis Theorem
        // More info: https://dyn4j.org/2010/01/sat/
        
        Bounds nodeBounds = node.GetBounds();
        // approximate triangle as rectangle for now
        Vector3 triBoundsCenter = 0.5f*(viewTriangle[0] + 0.5f * (viewTriangle[2] - viewTriangle[1]));
        Bounds triBounds = new Bounds(triBoundsCenter, Vector3.one * triBoundsCenter.magnitude); // can approx better by using isoceles properties
        
        return nodeBounds.Intersects(triBounds);
    }

    private Vector2 ComputeBotLeftPoint() {
        var botLeftPointV3 = cam.transform.position - new Vector3(renderDistance, 0f, renderDistance) * 0.5f;
        var botLeftPointV2 = new Vector2(botLeftPointV3.x, botLeftPointV3.z);
        return botLeftPointV2;
    }
    private Vector3[] GetViewTriangleFromCamera() {
        /* Calculates the view triangle from camera position and render distance */
        Vector3 camPos = cam.transform.position;
        Vector3 camForward = cam.transform.forward;
        Vector3 camRight = cam.transform.right;

        // Get the base width of the view triangle
        float FOVAngle = cam.fieldOfView;
        float halfAngle = (float)FOVAngle / 2;
        float halfWidth = 2 * renderDistance * Mathf.Tan(halfAngle);

        Vector3 leftPoint = camPos + camForward * renderDistance - camRight * halfWidth;
        Vector3 rightPoint = camPos + camForward * renderDistance + camRight * halfWidth;

        Vector3[] triangle = { camPos, leftPoint, rightPoint };

        this.viewTriangle = triangle;
        return triangle;

    }
}

