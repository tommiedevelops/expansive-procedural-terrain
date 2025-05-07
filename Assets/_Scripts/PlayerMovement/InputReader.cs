using UnityEngine;
using System;
using UnityEngine.InputSystem;
using static PlayerInputActions;
using UnityEngine.Events;

namespace CharacterControllerFactory {

    [CreateAssetMenu(fileName = "InputReader", menuName = "CharacterControllerFactory/Input/InputReader")]
    public class InputReader : ScriptableObject, IPlayerActions {

        public event UnityAction<Vector2> Move = delegate { };
        public event UnityAction<Vector2, bool> Look = delegate { };
        public event UnityAction EnableMouseControlCamera = delegate { };
        public event UnityAction DisableMouseControlCamera = delegate { };
        public event UnityAction<bool> Jump = delegate { };

        PlayerInputActions inputActions;

        public Vector3 Direction => inputActions.Player.Move.ReadValue<Vector2>();

        void OnEnable() {
            if (inputActions == null) {
                inputActions = new PlayerInputActions();
                inputActions.Player.SetCallbacks(this); // generated function that assigns an IPlayerActions interface implementation.
            }
        }

        public void EnablePlayerActions() {
            inputActions.Enable();
        }

        void OnDisable() {
            if (inputActions != null) {
                inputActions.Player.Disable();
                inputActions.UI.Disable();
            }
        }

        public void OnLook(InputAction.CallbackContext context) {
            Look.Invoke(context.ReadValue<Vector2>(), IsDeviceMouse(context));
        }

        bool IsDeviceMouse(InputAction.CallbackContext context) => context.control.device.name == "Mouse";
        
        public void OnMouseControlCamera(InputAction.CallbackContext context) {
            switch (context.phase) {
                case InputActionPhase.Started:
                    EnableMouseControlCamera();
                    break;
                case InputActionPhase.Canceled:
                    DisableMouseControlCamera();
                    break;
            }
        }

        public void OnZoom(InputAction.CallbackContext context) {
            // noop
        }

        public void OnMove(InputAction.CallbackContext context) {
           Move.Invoke(context.ReadValue<Vector2>());
        }

        public void OnSprint(InputAction.CallbackContext context) {
            // noop
        }

        public void OnFire(InputAction.CallbackContext context) {
            // noop
        }

        public void OnRun(InputAction.CallbackContext context) {
            // noop
        }

        public void OnJump(InputAction.CallbackContext context) {
            switch (context.phase) {
                case InputActionPhase.Started:
                    Jump.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    Jump.Invoke(false);
                    break;
            }
        }
    }

}