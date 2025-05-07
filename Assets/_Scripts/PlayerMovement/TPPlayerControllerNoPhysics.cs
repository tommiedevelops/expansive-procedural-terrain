using UnityEngine;
using KBCore.Refs;
using Unity.Cinemachine;
using System;

namespace CharacterControllerFactory {

    [RequireComponent (typeof (CharacterController))] 
    public class TPPlayerControllerNoPhysics : ValidatedMonoBehaviour {

        const float Zerof = 0f;

        [Header("References")]
        [SerializeField, Self] CharacterController controller; // means this SerializeField lives on the same GameObject as this script
        [SerializeField, Self] Animator animator;
        [SerializeField, Anywhere] CinemachineCamera freeLookVCam;
        [SerializeField, Anywhere] InputReader input;

        [Header("Settings")]
        [SerializeField] float moveSpeed = 6f;
        [SerializeField] float rotationSpeed = 15f;
        [SerializeField] float smoothTime = 0.2f; // How fast the animator will change speed

        Transform mainCam;

        float currentSpeed, velocity; // Why is velocity a float??

        // Animator parameters
        static readonly int Speed = Animator.StringToHash("Speed");

        private void Awake() {
            mainCam = Camera.main.transform;

            freeLookVCam.Follow = transform;
            freeLookVCam.LookAt = transform;
            // Invoke event when observed transform is teleported, adjusting freeLookVCam's position accordingly
            freeLookVCam.OnTargetObjectWarped(transform, transform.position - freeLookVCam.transform.position - Vector3.forward);
        }

        private void Start() {
            input.EnablePlayerActions();
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update() {
            HandleMovement();
            UpdateAnimator();
        }

        private void UpdateAnimator() {
            animator.SetFloat(Speed, currentSpeed);
        }

        private void HandleMovement() {
            var movementDirection = new Vector3(input.Direction.x, 0f, input.Direction.y).normalized;
            var adjustedDirection = Quaternion.AngleAxis(mainCam.eulerAngles.y, Vector3.up) * movementDirection; // basically [camera coords] - [transform] * moveDir in 1 line. 

            if(adjustedDirection.magnitude > Zerof) {
                HandleRotation(adjustedDirection);
                HandleCharacterController(adjustedDirection);
                SmoothSpeed(adjustedDirection.magnitude);
            } else {
                SmoothSpeed(Zerof);
            }

        }

        private void HandleRotation(Vector3 adjustedDirection) {
            // Adjust rotation to match movement direction
            var targetRotation = Quaternion.LookRotation(adjustedDirection); // Creates rotation from world up (0,1,0) to specified rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);


        }
        private void HandleCharacterController(Vector3 adjustedDirection) {
            // Move the player
            var adjustedMovement = adjustedDirection * (moveSpeed * Time.deltaTime);
            controller.Move(adjustedMovement); // USING IN BUILT CHAR CONTROLLER
        }

        private void SmoothSpeed(float value) {
            // smoothly transitions currentspeed to match adjustedMovement.magnitude over time. 
            // ref velocity is a reference variable to store velocity changes
            // smoothTime is the time taken to reach the target speed
            currentSpeed = Mathf.SmoothDamp(currentSpeed, value, ref velocity, smoothTime);
        }
    }

}