using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MOOSE
{
  public class InputHandler : MonoBehaviour
  {
    public float horizontal;
    public float vertical;
    public float moveAmount;
    public float mouseX;
    public float mouseY;

    PlayerControls inputActions;
    CameraHandler cameraHandler;
    Vector2 movementInput;
    Vector2 cameraInput;

    // Used to init variables or states before the application starts
    private void Awake()
    {
      cameraHandler = CameraHandler.singleton;
    }

    // called every fixed frame-rate frame
    private void FixedUpdate()
    {
      float delta = Time.fixedDeltaTime; // rate of time in seconds that the game is updating between frames

      if (cameraHandler != null)
      {
        cameraHandler.FollowTarget(delta);
        cameraHandler.handleCameraRotation(delta, mouseX, mouseY);
      }
    }

    public void OnEnable()
    {
      if (inputActions == null)
      {
        inputActions = new PlayerControls();
        inputActions.PlayerMovement.Movement.performed += inputActions => movementInput = inputActions.ReadValue<Vector2>();
        inputActions.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();
      }

      inputActions.Enable();
    }

    public void OnDisable()
    {
      inputActions.Disable();
    }

    public void TickInput(float delta)
    {
      MoveInput(delta);
    }

    private void MoveInput(float delta)
    {
      horizontal = movementInput.x;
      vertical = movementInput.y;
      // Clamps (bounds) value between 0 and 1 and returns value.
      // If the value is negative then zero is returned. If value is greater than one then one is returned.
      moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
      mouseX = cameraInput.x;
      mouseY = cameraInput.y;
    }
  }
}
