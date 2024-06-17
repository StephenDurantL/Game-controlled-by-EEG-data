using UnityEngine;

public class Movement : MonoBehaviour
{
    private new Rigidbody2D rigidbody;
    private new Camera camera;
    private new Collider2D collider;

    private Vector2 velocity;
    private float inputAxis;

    public float moveSpeed = 8f;
    public float maxJumpHeight = 5f;
    public float maxJumpTime = 1f;
    public float jumpForce => (2f * maxJumpHeight) / (maxJumpTime / 2f);
    public float gravity => (-2f * maxJumpHeight) / Mathf.Pow((maxJumpTime / 2f), 2);

    public bool grounded { get; private set; }
    public bool jumping { get; private set; }

    public bool running => Mathf.Abs(velocity.x) > 0.25f || Mathf.Abs(inputAxis) > 0.25f;
    public bool falling => velocity.y < 0f && !grounded;

    private LayerMask layerMask;
    private EEGReceiver eegReceiver;
    EEGDataProcessor processor = new EEGDataProcessor();
    private double settingband;

    private void Awake()
    {
        camera = Camera.main;
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        layerMask = LayerMask.GetMask("Default");
        eegReceiver = FindObjectOfType<EEGReceiver>();
    }

    private void OnEnable()
    {
        rigidbody.isKinematic = false;
        collider.enabled = true;
        velocity = Vector2.zero;
        jumping = false;
    }

    private void OnDisable()
    {
        rigidbody.isKinematic = true;
        collider.enabled = false;
        velocity = Vector2.zero;
        jumping = false;
    }

    private void Update()
    {
        HorizontalMovement();
        grounded = Raycast(rigidbody, Vector2.down);

        if (grounded)
        {
            GroundedMovement();
        }

        ApplyGravity();
    }

    public void Jump()
    {
        if (grounded)
        {
            velocity.y = jumpForce;
            jumping = true;
            // Process EEG data and set horizontal velocity based on it
            settingband = processor.ProcessData(eegReceiver.dataQueue);
            Debug.Log("settingband"+ settingband);
            if (settingband > 200)
            {
                velocity.x = 1 * 8f; // Move right if EEG band amplitude is above threshold
            }

        }
    }

    private void GroundedMovement()
    {
        velocity.y = Mathf.Max(velocity.y, 0f);
        jumping = velocity.y > 0f;

        // Perform a jump if the jump button is pressed
        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
    }

    private void HorizontalMovement()
    {
        // Flip the character sprite based on horizontal velocity direction
        if (velocity.x > 0f)
        {
            transform.eulerAngles = Vector3.zero;
        }
        else if (velocity.x < 0f)
        {
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }
    }

    private void ApplyGravity()
    {
        // Apply gravity with increased force while falling
        bool falling = velocity.y < 0f || !Input.GetButton("Jump");
        float multiplier = falling ? 2f : 1f;

        velocity.y += gravity * multiplier * Time.deltaTime;
        velocity.y = Mathf.Max(velocity.y, gravity / 2f);
    }

    private void FixedUpdate()
    {
        // Move the character using Rigidbody's MovePosition method
        Vector2 position = rigidbody.position;
        position += velocity * Time.fixedDeltaTime;
        rigidbody.MovePosition(position);
    }

    private bool Raycast(Rigidbody2D rigidbody, Vector2 direction)
    {
        // Perform a raycast to detect ground
        if (rigidbody.isKinematic)
        {
            return false;
        }

        float radius = 0.25f;
        float distance = 0.375f;

        RaycastHit2D hit = Physics2D.CircleCast(rigidbody.position, radius, direction.normalized, distance, layerMask);
        return hit.collider != null && hit.rigidbody != rigidbody;
    }

    private bool IsCollisionAtDirection(Collision2D collision, Vector2 direction)
    {
        // Check if collision normal aligns with the specified direction
        foreach (ContactPoint2D point in collision.contacts)
        {
            if (Vector2.Dot(point.normal, direction) > 0)
            {
                return true;
            }
        }
        return false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Handle collisions with enemies and other objects
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            // Perform a mini-jump if colliding with an enemy from below
            if (IsCollisionAtDirection(collision, Vector2.up))
            {
                velocity.y = jumpForce / 2f;
                jumping = true;
            }
        }
        else if (collision.gameObject.layer != LayerMask.NameToLayer("PowerUp"))
        {
            // Stop vertical movement when colliding with solid ground
            if (IsCollisionAtDirection(collision, Vector2.down))
            {
                velocity.y = 0f;
            }
        }
    }
}
