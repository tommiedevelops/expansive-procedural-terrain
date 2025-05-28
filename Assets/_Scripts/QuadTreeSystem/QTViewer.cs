using System;
using UnityEditorInternal;
using UnityEngine;

public class QTViewer {
  
    #region Fields

    private Transform _cameraTransform;
    private float _renderDistance;
    private Vector3[] _viewTriangle;
    private Bounds _triBounds; //temporary
    private readonly float _cameraFOV;
    #endregion

    public QTViewer(Transform cameraTransform, float cameraFOV, float renderDistance) {
        // Validate inputs
        if (cameraTransform == null || cameraFOV < 0 || cameraFOV > 90 || renderDistance < 0) {
            throw new SystemException("cannot create viewer, inputs invalid");
        }

        _cameraTransform = cameraTransform;
        _cameraFOV = cameraFOV;
        _renderDistance = renderDistance;
    }
    

    #region Helpers

    private void ComputeViewTriangle() {
        // 3D Coords
        Vector3 camPos = _cameraTransform.position; //world
        Vector3 camForward = _cameraTransform.forward;
        Vector3 camRight = _cameraTransform.right;

        // Projected onto XZ plane
        Vector3 camPosXZ = new(camPos.x, 0f, camPos.z);
        Vector3 camForwardXZ = new(camForward.x, 0f, camForward.z);
        Vector3 camRightXZ = new(camRight.x, 0f, camRight.z);

        // Normalize directions
        camForwardXZ = camForwardXZ.normalized;
        camRightXZ = camRightXZ.normalized;

        // Get the base width of the view triangle
        float fovAngle = _cameraFOV;
        float halfAngle = (float)fovAngle / 2;
        float halfWidth = _renderDistance * Mathf.Tan(DegToRad(halfAngle));

        Vector3 leftPoint = camPosXZ + camForwardXZ * _renderDistance - camRightXZ * halfWidth;
        Vector3 rightPoint = camPosXZ + camForwardXZ * _renderDistance + camRightXZ * halfWidth;

        Vector3[] triangle = { camPosXZ, leftPoint, rightPoint }; //all in world coords

        this._viewTriangle = triangle;
    }
    public Bounds ComputeTriBounds() {
        // approximate triangle as rectangle for now
        ComputeViewTriangle();

        Vector3 camPos = _viewTriangle[0];
        Vector3 leftPoint = _viewTriangle[1];
        Vector3 rightPoint = _viewTriangle[2];

        Vector3 halfPoint = leftPoint + 0.5f * (rightPoint - leftPoint);
        Vector3 median = halfPoint - camPos;

        Vector3 boundsDimensions = new(_renderDistance, 0f, _renderDistance);

        Bounds triBounds = new(camPos + 0.5f * median, boundsDimensions); // can approx better by using isoceles properties
        this._triBounds = triBounds;
        return triBounds;
    }

    private float DegToRad(float angleInDeg) { return angleInDeg * Mathf.PI / 180f; }
    #endregion

    #region Setters & Getters
    // SETTERS & GETTERS
    public void SetRenderDistance(int renderDistance) {
        this._renderDistance = renderDistance;
    }
    public void SetCameraTransform(Transform cameraTransform) {
        this._cameraTransform = cameraTransform;
    }

    public Vector3[] GetViewTriangle() {
        ComputeViewTriangle();
        return _viewTriangle;
    }
    public Bounds GetTriBounds() {
        ComputeTriBounds();
        return _triBounds;
    }
    public Transform GetCameraTransform() { return _cameraTransform.transform;  }
    public float GetRenderDist() { return _renderDistance; }

    public float GetFOV() {
        return _cameraFOV;
    }

    #endregion

    public Vector2 GetPosition() => new Vector2(_cameraTransform.position.x, _cameraTransform.position.z);

}
