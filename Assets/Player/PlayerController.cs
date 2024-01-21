using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    public Camera playerCamera;
    public float moveSpeed = 10f;
    public float lookSpeed = 10f;
    public GameObject paintMarker;

    private Vector2 move;
    private Vector2 look;
    private bool painting;

    public void Start()
    {
        // Hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        look = context.ReadValue<Vector2>();
    }

    public void OnPaint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            painting = true;
            Debug.Log("Painting started");
        }
        if (context.canceled)
        {
            painting = false;
            Debug.Log("Painting stopped");
        }
    }

    private void Paint()
    {
        // Create a ray that shoots forward from the camera
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        // The RaycastHit object will contain information about what the ray hit
        RaycastHit hit;

        // Create a layer mask that includes all layers except "IgnorePaintRaycast"
        int layerMask = ~LayerMask.GetMask("IgnorePaintRaycast");

        // Perform the raycast
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            // If the ray hit something, the hit point is where the player is looking
            Vector3 lookPoint = hit.point;

            // move the paint marker to the hit point
            paintMarker.SetActive(true);
            paintMarker.transform.position = lookPoint;
        }
        else
        {
            paintMarker.SetActive(false);
        }
    }

    void Update()
    {
        // Rotate the player based on the look input (mouse delta)
        Vector3 rotation = new Vector3(-look.y, look.x, 0) * lookSpeed * Time.deltaTime;
        transform.Rotate(rotation, Space.Self);

        // Move the player in the direction of the move input, relative to the player's rotation
        Vector3 moveDirection = new Vector3(move.x, 0, move.y);
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.Self);

        // Ensure Z rotation in Euler angles is always zero
        Vector3 currentEulerAngles = transform.eulerAngles;
        currentEulerAngles.z = 0;
        transform.eulerAngles = currentEulerAngles;

        if (painting)
        {
            Paint();
        }
        else
        {
            paintMarker.SetActive(false);
        }
    }
}