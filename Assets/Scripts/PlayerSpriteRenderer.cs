using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerSpriteRenderer : MonoBehaviour
{
    private Movement movement; // Reference to the Movement component of the parent object
    public SpriteRenderer spriteRenderer { get; private set; } // Reference to the SpriteRenderer component
    public Sprite idle; // Sprite for idle state
    public Sprite jump; // Sprite for jump state

    public AnimatedSprite run; // Reference to the AnimatedSprite component for running animation

    private void Awake()
    {
        movement = GetComponentInParent<Movement>(); // Get the Movement component in the parent object
        spriteRenderer = GetComponent<SpriteRenderer>(); // Get the SpriteRenderer component
    }

    private void LateUpdate()
    {
        // Enable run animation if the player is running
        run.enabled = movement.running;

        // Set sprite based on player state
        if (movement.jumping)
        {
            spriteRenderer.sprite = jump; // Set jump sprite if the player is jumping
        }
        else if (!movement.running)
        {
            spriteRenderer.sprite = idle; // Set idle sprite if the player is not running
        }
    }

    private void OnEnable()
    {
        spriteRenderer.enabled = true; // Enable sprite renderer when the object is enabled
    }

    private void OnDisable()
    {
        spriteRenderer.enabled = false; // Disable sprite renderer when the object is disabled
        run.enabled = false; // Disable run animation when the object is disabled
    }
}
