using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class CameraController : NetworkBehaviour
{
    [SerializeField] private Transform playerCameraTransform = null;
    [SerializeField] private float speed = 20f;
    [SerializeField] private float screenBorderThickness = 10f; // How close to the edge of the screen for it to start moving the camera in that direction
    [SerializeField] private Vector2 screenXLimits = Vector2.zero;
    [SerializeField] private Vector2 screenZLimits = Vector2.zero;

    private Vector2 previousInput;

    private Controls controls;

    public override void OnStartAuthority()
    {
        playerCameraTransform.gameObject.SetActive(true); // turn the camera on

        controls = new Controls();

        controls.Player.MoveCamera.performed += SetPreviousInput;
        controls.Player.MoveCamera.canceled -= SetPreviousInput;

        controls.Enable();
    }

    [ClientCallback]
    private void Update() 
    {
        if (!hasAuthority || !Application.isFocused)
        {
            // Don't move the camera if we're not tabbed in or if we don't have authority, so we don't move other people's cameras
            return;
        }

        UpdateCameraPosition();
    }

    private void UpdateCameraPosition()
    {
        Vector3 position = playerCameraTransform.position;

        if (previousInput == Vector2.zero)
        {
            // Check if the mouse is in one of the edges of the screen
            Vector3 cursorMovement = Vector3.zero;

            Vector2 cursorPosition = Mouse.current.position.ReadValue();

            if (cursorPosition.y >= Screen.height - screenBorderThickness)
            {
                cursorMovement.z += 1;
            }
            else if (cursorPosition.y <= screenBorderThickness)
            {
                cursorMovement.z -= 1;
            }
            if (cursorPosition.x >= Screen.width - screenBorderThickness)
            {
                cursorMovement.z += 1;
            }
            else if (cursorPosition.x <= screenBorderThickness)
            {
                cursorMovement.x -= 1;
            }

            position += cursorMovement.normalized * speed * Time.deltaTime;
        }
        else 
        {
            position += new Vector3(previousInput.x, 0f, previousInput.y) * speed * Time.deltaTime;
        }

        position.x = Mathf.Clamp(position.x, screenXLimits.x, screenZLimits.y);
        position.z = Mathf.Clamp(position.z, screenZLimits.x, screenZLimits.y);

        playerCameraTransform.position = position;
    }

    private void SetPreviousInput(InputAction.CallbackContext context)
    {
        previousInput = context.ReadValue<Vector2>();
    }
}
