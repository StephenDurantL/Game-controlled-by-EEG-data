using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMovement : MonoBehaviour
{
    public float speed = 1f; // Movement speed of the entity
    public Vector2 direction = Vector2.left; // Direction of movement

    private new Rigidbody2D rigidbody; // Reference to the Rigidbody2D component
    private Vector2 velocity; // Current velocity of the entity
    private LayerMask layerMask; // Layer mask for raycasting

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        enabled = false;
        layerMask = LayerMask.GetMask("Default");
    }

    private void OnBecameVisible()
    {
        // Enable movement when the entity becomes visible
        enabled = true;
    }

    private void OnBecameInvisible()
    {
        // Disable movement when the entity becomes invisible
        enabled = false;
    }

    private void OnEnable()
    {
        // Wake up the Rigidbody when the entity is enabled
        rigidbody.WakeUp();
    }

    private void OnDisable()
    {
        // Reset velocity and put the Rigidbody to sleep when the entity is disabled
        rigidbody.velocity = Vector2.zero;
        rigidbody.Sleep();
    }

    private void FixedUpdate()
    {
        // Calculate horizontal movement
        velocity.x = direction.x * speed;
        // Apply gravity to vertical movement
        velocity.y += Physics2D.gravity.y * Time.fixedDeltaTime;

        // Move the entity using Rigidbody's MovePosition method
        rigidbody.MovePosition(rigidbody.position + velocity * Time.fixedDeltaTime);

        // Reverse direction if hitting a wall
        if (Raycast(rigidbody, direction))
        {
            direction = -direction;
        }

        // Prevent falling through platforms
        if (Raycast(rigidbody, Vector2.down))
        {
            velocity.y = Mathf.Max(velocity.y, 0f);
        }
    }

    // Method to perform raycasting to detect obstacles
    private bool Raycast(Rigidbody2D rigidbody, Vector2 direction)
    {
        // Skip raycast if the Rigidbody is kinematic
        if (rigidbody.isKinematic)
        {
            return false;
        }

        // Define circle cast parameters
        float radius = 0.25f;
        float distance = 0.375f;

        // Perform the circle cast
        RaycastHit2D hit = Physics2D.CircleCast(rigidbody.position, radius, direction.normalized, distance, layerMask);
        // Return true if hit a collider other than itself
        return hit.collider != null && hit.rigidbody != rigidbody;
    }
}
