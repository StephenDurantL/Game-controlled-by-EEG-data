using UnityEngine;

public class CharacterJump : MonoBehaviour
{
    private bool shouldJump = false; // This variable is used to indicate whether the character should jump
    private float force; // This variable is used to store the jump force
    private Rigidbody2D rb;
    private Movement movement;

    private void Awake()
    {
        // Get the Rigidbody2D component attached to this GameObject
        rb = GetComponent<Rigidbody2D>();
        // Get the Movement component attached to this GameObject
        movement = GetComponent<Movement>();
    }

    private void Update()
    {
        // Check if the character should jump in the main thread (Update method)
        if (shouldJump && force > 0)
        {
            // Execute the jump action from the Movement component
            movement.Jump();
            // Reset the jump flag
            shouldJump = false;
        }
    }

    // Method to handle jump requests
    public void AttemptJump(float jumpForce)
    {
        // Set the jump force
        force = jumpForce;
        // Set the flag to indicate that the character should jump
        shouldJump = true;
    }

    private void OnEnable()
    {
        // Subscribe to the OnJumpRequested event when this GameObject is enabled
        EventManager.OnJumpRequested += AttemptJump;
    }

    private void OnDisable()
    {
        // Unsubscribe from the OnJumpRequested event when this GameObject is disabled
        EventManager.OnJumpRequested -= AttemptJump;
    }
}
