using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class AnimatedSprite : MonoBehaviour
{
    // Array to hold the sprites for animation
    public Sprite[] sprites;

    // Frame rate of the animation (in seconds per frame)
    public float framerate = 1f / 60f;

    // Reference to the SpriteRenderer component
    private SpriteRenderer spriteRenderer;

    // Current frame index
    private int frame;

    private void Awake()
    {
        // Get the SpriteRenderer component attached to this GameObject
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        // Start animating when this GameObject becomes enabled
        InvokeRepeating(nameof(Animate), framerate, framerate);
    }

    private void OnDisable()
    {
        // Stop the animation when this GameObject becomes disabled
        CancelInvoke();
    }

    // Method to animate the sprite
    private void Animate()
    {
        // Increment the frame index
        frame++;

        // Reset the frame index if it exceeds the length of the sprites array
        if (frame >= sprites.Length) {
            frame = 0;
        }

        // Ensure the frame index is within the valid range
        if (frame >= 0 && frame < sprites.Length) {
            // Update the sprite to display the current frame
            spriteRenderer.sprite = sprites[frame];
        }
    }
}
