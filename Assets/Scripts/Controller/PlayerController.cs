using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [Tooltip("If true, rotation snaps instantly. If false, it turns smoothly.")]
    [SerializeField] private bool snapRotation = true;
    [SerializeField] private float rotationSpeed = 720f;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Subscribe to the channel when this object is enabled
    private void OnEnable()
    {
        InputBroadcaster.MoveEvent += HandleMoveInput;
    }

    // Unsubscribe when disabled (CRITICAL to prevent errors)
    private void OnDisable()
    {
        InputBroadcaster.MoveEvent -= HandleMoveInput;
    }

    // This function runs whenever the Broadcaster yells "Move!"
    private void HandleMoveInput(Vector2 direction)
    {
        moveInput = direction;
    }



    private void FixedUpdate()
    {
        // 1. Handle Movement
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);

        // 2. Handle Rotation
        // We only rotate if there is actual input, otherwise the character 
        // would snap back to 0 degrees (Right) when you stop pressing keys.
        if (moveInput != Vector2.zero)
        {
            // Calculate the angle in degrees from the vector
            float angle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg;

            // Create the target rotation (Z-axis only for 2D)
            Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);

            if (snapRotation)
            {
                // Snap instantly to the new direction (Best for pixel art/8-dir)
                transform.rotation = targetRotation;
            }
            else
            {
                // Rotate smoothly over time
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            }
        }
    }
}