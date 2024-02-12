
using UnityEngine;


public class Movement : MonoBehaviour
{
    private new Rigidbody2D rigidbody;

    private Vector2 velocity;
    private float inputAxis;

    public float moveSpeed = 8f;
    public float maxJumpHeight=5f;
    public float maxJumpTime=1f;
    public float jumpForce => (2f * maxJumpHeight)/ (maxJumpTime / 2f);
    public float gravity => (-2f * maxJumpHeight)/ Mathf.Pow((maxJumpTime / 2f),2);

    public bool grounded { get; private set; }
    public bool jumping { get; private set; }




    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();


    }

    private void Update()
    {
        HorizontalMovement();

        grounded = Raycast(rigidbody,Vector2.down);

        if (grounded) {
            GroundedMovement();
        }

       ApplyGravity();
        

    }

    

    private void GroundedMovement()
    {
        // prevent gravity from infinitly building up
        velocity.y = Mathf.Max(velocity.y, 0f);
        jumping = velocity.y > 0f;

        // perform jump
        if (Input.GetButtonDown("Jump"))
        {
            velocity.y = jumpForce;
            jumping = true;
        }
    }

    private void HorizontalMovement()
    {
        inputAxis = Input.GetAxis("Horizontal");
        velocity.x = Mathf.MoveTowards(velocity.x, inputAxis*moveSpeed, moveSpeed*Time.deltaTime);

    }

    private void ApplyGravity()
    {
        // check if falling
        bool falling = velocity.y < 0f || !Input.GetButton("Jump");
        float multiplier = falling ? 2f : 1f;

        // apply gravity and terminal velocity
        velocity.y += gravity * multiplier * Time.deltaTime;
        velocity.y = Mathf.Max(velocity.y, gravity / 2f);
    }

    private void FixedUpdate()
    {
        Vector2 position = rigidbody.position;
        position += velocity * Time.fixedDeltaTime;

        rigidbody.MovePosition(position);
    }

    private bool Raycast( Rigidbody2D rigidbody, Vector2 direction)
    {
        LayerMask layerMask = LayerMask.GetMask("Default");
        
        if (rigidbody.isKinematic) {
            return false;
        }

        float radius = 0.25f;
        float distance = 0.375f;

        RaycastHit2D hit = Physics2D.CircleCast(rigidbody.position, radius, direction.normalized, distance, layerMask);
        return hit.collider != null && hit.rigidbody != rigidbody;
    }

    





    
}

