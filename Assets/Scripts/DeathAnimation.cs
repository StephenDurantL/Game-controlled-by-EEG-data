using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathAnimation : MonoBehaviour
{
    public SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer component
    public Sprite deadSprite; // Sprite to display when the object is "dead"

    // Reset function to assign default references
    private void Reset()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Called when the GameObject becomes enabled and active
    private void OnEnable()
    {
        // Update the sprite, disable physics, and start the animation coroutine
        UpdateSprite();
        DisablePhysics();
        StartCoroutine(Animate());
    }

    // Called when the GameObject is disabled
    private void OnDisable()
    {
        // Stop all coroutines to prevent any ongoing animations
        StopAllCoroutines();
    }

    // Method to update the sprite renderer with the dead sprite
    private void UpdateSprite()
    {
        // Enable the sprite renderer and set sorting order
        spriteRenderer.enabled = true;
        spriteRenderer.sortingOrder = 10;

        // Set the sprite to the dead sprite if it's not null
        if (deadSprite != null) {
            spriteRenderer.sprite = deadSprite;
        }
    }

    // Method to disable physics components and movement scripts
    private void DisablePhysics()
    {
        // Disable all colliders attached to the GameObject
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D collider in colliders) {
            collider.enabled = false;
        }

        // Make the Rigidbody2D kinematic to disable physics simulation
        GetComponent<Rigidbody2D>().isKinematic = true;

        // Disable Movement and EntityMovement scripts if they exist
        Movement movement = GetComponent<Movement>();
        EntityMovement entityMovement = GetComponent<EntityMovement>();
        if (movement != null) {
            movement.enabled = false;
        }
        if (entityMovement != null) {
            entityMovement.enabled = false;
        }
    }

    // Coroutine to animate the "death" by applying upward motion with gravity
    private IEnumerator Animate()
    {
        // Animation duration and variables for jump physics
        float elapsed = 0f;
        float duration = 3f;
        float jumpVelocity = 10f;
        float gravity = -36f;

        // Initial velocity for jump
        Vector3 velocity = Vector3.up * jumpVelocity;

        // Loop until the animation duration is reached
        while (elapsed < duration)
        {
            // Update position based on velocity
            transform.position += velocity * Time.deltaTime;
            // Apply gravity to the velocity
            velocity.y += gravity * Time.deltaTime;
            // Update elapsed time
            elapsed += Time.deltaTime;
            // Wait for the next frame
            yield return null;
        }
    }
}
