using UnityEngine;

public class OverheadCameraDragController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;  // Speed of camera movement

    [Header("Drag Settings")]
    public bool enableDrag = true;  // Enable dragging with the mouse
    private Vector3 dragOrigin;  // The position where the drag started
    private bool isDragging = false;  // Whether the mouse is currently being dragged

    void Update()
    {
        if (enableDrag)
        {
            HandleDrag();
        }

        // Use WASD keys or arrow keys for movement
        HandleKeyboardMovement();
    }

    private void HandleDrag()
    {
        // Check for mouse button press to start dragging
        if (Input.GetMouseButtonDown(0))  // Left mouse button
        {
            dragOrigin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            dragOrigin.z = transform.position.z;  // Keep the camera's Z position the same
            isDragging = true;
        }

        // Check for mouse button release to stop dragging
        if (Input.GetMouseButtonUp(0))  // Left mouse button
        {
            isDragging = false;
        }

        // If dragging, move the camera
        if (isDragging)
        {
            Vector3 currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentMousePosition.z = transform.position.z;  // Maintain Z axis position
            Vector3 offset = dragOrigin - currentMousePosition;  // Get the difference between current and origin
            transform.position += offset;  // Move the camera by the offset
            dragOrigin = currentMousePosition;  // Update drag origin to current position
        }
    }

    private void HandleKeyboardMovement()
    {
        // Get input for camera movement (WASD or Arrow keys)
        float horizontal = Input.GetAxisRaw("Horizontal");  // A/D or Left/Right arrow
        float vertical = Input.GetAxisRaw("Vertical");      // W/S or Up/Down arrow

        // Move the camera based on input
        Vector3 movement = new Vector3(horizontal, vertical, 0f).normalized;
        transform.Translate(movement * moveSpeed * Time.deltaTime);
    }
}
