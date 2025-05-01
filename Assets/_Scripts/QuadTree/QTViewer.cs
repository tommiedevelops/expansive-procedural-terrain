using System;
using UnityEditorInternal;
using UnityEngine;

public class QTViewer {
  
    #region Fields
    Transform cameraTransform;
    float renderDistance;
    Vector3[] viewTriangle;
    Bounds triBounds; //temporary
    float cameraFOV = 60;
    #endregion

    public QTViewer(Transform cameraTransform, float cameraFOV, float renderDistance) {
        // Validate inputs
        if (cameraTransform == null || cameraFOV < 0 || cameraFOV > 90 || renderDistance < 0) {
            throw new SystemException("cannot create viewer, inputs invalid");
        }

        this.cameraTransform = cameraTransform;
        this.cameraFOV = cameraFOV;
        this.renderDistance = renderDistance;
    }

    #region Helpers
    void ComputeViewTriangle() {
        // 3D Coords
        Vector3 camPos = cameraTransform.position; //world
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        // Projected onto XZ plane
        Vector3 camPosXZ = new(camPos.x, 0f, camPos.z);
        Vector3 camForwardXZ = new(camForward.x, 0f, camForward.z);
        Vector3 camRightXZ = new(camRight.x, 0f, camRight.z);

        // Normalize directions
        camForwardXZ = camForwardXZ.normalized;
        camRightXZ = camRightXZ.normalized;

        // Get the base width of the view triangle
        float FOVAngle = cameraFOV;
        float halfAngle = (float)FOVAngle / 2;
        float halfWidth = renderDistance * Mathf.Tan(DegToRad(halfAngle));

        Vector3 leftPoint = camPosXZ + camForwardXZ * renderDistance - camRightXZ * halfWidth;
        Vector3 rightPoint = camPosXZ + camForwardXZ * renderDistance + camRightXZ * halfWidth;

        Vector3[] triangle = { camPosXZ, leftPoint, rightPoint }; //all in world coords

        this.viewTriangle = triangle;
    }
    public Bounds ComputeTriBounds() {
        // approximate triangle as rectangle for now
        ComputeViewTriangle();

        Vector3 camPos = viewTriangle[0];
        Vector3 leftPoint = viewTriangle[1];
        Vector3 rightPoint = viewTriangle[2];

        Vector3 halfPoint = leftPoint + 0.5f * (rightPoint - leftPoint);
        Vector3 median = halfPoint - camPos;

        Vector3 boundsDimensions = new(renderDistance, 0f, renderDistance);

        Bounds triBounds = new(camPos + 0.5f * median, boundsDimensions); // can approx better by using isoceles properties
        this.triBounds = triBounds;
        return triBounds;
    }
    float DegToRad(float angleInDeg) { return angleInDeg * Mathf.PI / 180f; }
    #endregion

    #region Setters & Getters
    // SETTERS & GETTERS
    public void SetRenderDistance(int renderDistance) {
        this.renderDistance = renderDistance;
    }
    public void SetCameraTransform(Transform cameraTransform) {
        this.cameraTransform = cameraTransform;
    }

    public Vector3[] GetViewTriangle() {
        ComputeViewTriangle();
        return viewTriangle;
    }
    public Bounds GetTriBounds() {
        ComputeTriBounds();
        return triBounds;
    }
    public Transform GetCameraTransform() { return cameraTransform.transform;  }
    public float GetRenderDist() { return renderDistance; }

    public float GetFOV() {
        return cameraFOV;
    }

    #endregion

}
