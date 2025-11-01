using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 20f;          // Base movement speed
    public float fastMoveMultiplier = 2f;  // Hold Shift to move faster
    public float edgeScrollSize = 10f;     // Pixels from screen edge to trigger scroll

    [Header("Zoom Settings")]
    public float zoomSpeed = 200f;
    public float minZoom = 20f;
    public float maxZoom = 120f;

    [Header("Rotation Settings")]
    public float rotationSpeed = 50f;      // Degrees per second

    private Camera cam;

    void Start()
    {
        cam = GetComponentInChildren<Camera>(); // safer if camera is child of rig
    }

    void Update()
    {
        HandleMovement();
        HandleZoom();
        HandleRotation();
    }

    void HandleMovement()
    {
        float speed = moveSpeed;

        // Hold Left Shift for faster movement
        if (Input.GetKey(KeyCode.LeftShift))
            speed *= fastMoveMultiplier;

        Vector3 direction = Vector3.zero;

        // Keyboard input
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            direction += transform.forward;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            direction -= transform.forward;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            direction -= transform.right;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            direction += transform.right;

        // Flatten movement to XZ plane
        direction.y = 0;

        // Apply movement
        transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0.0f)
        {
            cam.fieldOfView -= scroll * zoomSpeed * Time.deltaTime;
            cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, minZoom, maxZoom);
        }
    }

    void HandleRotation()
    {
        if (Input.GetKey(KeyCode.Q))
            transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime, Space.World);
        if (Input.GetKey(KeyCode.E))
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
    }
}