using System;
using UnityEditorInternal;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]

// For a player to interact with the quad tree, I could define an interface for it to implement
// which enforces the requried fields and methods
public class QTViewer : MonoBehaviour {

    #region Fields
    CharacterController _cc;
    [SerializeField] Transform cameraTransform;
    [SerializeField] float speed;
    [SerializeField] Quaternion rotation;
    [SerializeField] float renderDistance;

    Vector3[] viewTriangle;
    Bounds triBounds; //temporary
    float cameraFOV = 60;
    #endregion

    #region Unity Functions
    // UNITY GAME LOOP
    private void Awake() {
        _cc = GetComponent<CharacterController>();
    }

    void Start() {
        ComputeViewTriangle();
        ComputeTriBounds();
    }

    public void SetRenderDistance(int renderDistance) {
        this.renderDistance = renderDistance;
    }

    public void SetCameraTransform(Transform cameraTransform) {
        this.cameraTransform = cameraTransform;
    }

    private void Update() {
        HandleMovement();
    }
    #endregion

    #region Helpers
    // HELPERS
    public void UpdateViewTriangle() {
        ComputeTriBounds();
        ComputeViewTriangle();
    }
    private void HandleMovement() {
        Vector3 diff = transform.position - cameraTransform.transform.position;
        Vector3 forward = new Vector3(diff.x, 0f, diff.z);
        transform.rotation = rotation;
        if (Input.GetKey(KeyCode.W)) _cc.Move(speed * Time.deltaTime * forward);
    }
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
    float DegToRad(float angleInDeg) { return angleInDeg * Mathf.PI / 180f; }
    public Bounds ComputeTriBounds() {
        // approximate triangle as rectangle for now

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
    #endregion

    #region Setters & Getters
    // SETTERS & GETTERS
    public Vector3[] GetViewTriangle() {
        ComputeViewTriangle();
        return viewTriangle;
    }
    public Bounds GetTriBounds() {
        ComputeTriBounds();
        return triBounds;
    }

    public Vector3 GetPosition() {
        return transform.position;
    }
    public Transform GetCameraTransform() { return cameraTransform.transform;  }
    public float GetRenderDist() { return renderDistance; }

    #endregion

}
