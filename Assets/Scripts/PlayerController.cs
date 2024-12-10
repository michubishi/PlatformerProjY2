using UnityEngine;

/**
 * Horizontal/Veritcal default movement is grabbed from A2_Platformer Template. 
 * Numbers for movement may have been adjusted to match the movement that fits right for my platformer.
 */
public enum PlayerDirection
{
    left, right
}

public enum PlayerState
{
    idle, walking, jumping, dead
}

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D body;
    private PlayerDirection currentDirection = PlayerDirection.right;
    public PlayerState currentState = PlayerState.idle;
    public PlayerState previousState = PlayerState.idle;

    [Header("Horizontal")]
    public float maxSpeed = 5f;
    public float accelerationTime = 0f;
    public float decelerationTime = 0f;

    [Header("Vertical")]
    public float apexHeight = 3f;
    public float apexTime = 0.5f;

    [Header("Ground Checking")]
    public float groundCheckOffset = 0.5f;
    public Vector2 groundCheckSize = new(0.4f, 0.1f);
    public LayerMask groundCheckMask;

    private float accelerationRate;
    private float decelerationRate;


    private float dashDistance = 3;

    private float force = 2000;

    private float jumpDistance = 2;

    private float gravity;
    private float initialJumpSpeed;

    private bool isGrounded = false;
    public bool isDead = false;

    private Vector2 velocity;

    public void Start()
    {
        body.gravityScale = 0;

        accelerationRate = maxSpeed / accelerationTime;
        decelerationRate = maxSpeed / decelerationTime;

        gravity = -2 * apexHeight / (apexTime * apexTime);
        initialJumpSpeed = 2 * apexHeight / apexTime;
    }

    public void Update()
    {
        previousState = currentState;

        CheckForGround();

        Vector2 playerInput = new Vector2();
        playerInput.x = Input.GetAxisRaw("Horizontal");

        if (isDead)
        {
            currentState = PlayerState.dead;
        }

        switch(currentState)
        {
            case PlayerState.dead:
                // do nothing - we ded.
                break;
            case PlayerState.idle:
                if (!isGrounded) currentState = PlayerState.jumping;
                else if (velocity.x != 0) currentState = PlayerState.walking;
                break;
            case PlayerState.walking:
                if (!isGrounded) currentState = PlayerState.jumping;
                else if (velocity.x == 0) currentState = PlayerState.idle;
                break;
            case PlayerState.jumping:
                if (isGrounded)
                {
                    if (velocity.x != 0) currentState = PlayerState.walking;
                    else currentState = PlayerState.idle;
                }
                break;
        }

        MovementUpdate(playerInput);
        JumpUpdate();
        Dash();

        if (!isGrounded)
            velocity.y += gravity * Time.deltaTime;
        else
            velocity.y = 0;

        body.velocity = velocity;
    }

    private void MovementUpdate(Vector2 playerInput)
    {
        if (playerInput.x < 0)
            currentDirection = PlayerDirection.left;
        else if (playerInput.x > 0)
            currentDirection = PlayerDirection.right;

        if (playerInput.x != 0)
        {
            velocity.x += accelerationRate * playerInput.x * Time.deltaTime;
            velocity.x = Mathf.Clamp(velocity.x, -maxSpeed, maxSpeed);
        }
        else
        {
            if (velocity.x > 0)
            {
                velocity.x -= decelerationRate * Time.deltaTime;
                velocity.x = Mathf.Max(velocity.x, 0);
            }
            else if (velocity.x < 0)
            {
                velocity.x += decelerationRate * Time.deltaTime;
                velocity.x = Mathf.Min(velocity.x, 0);
            }
        }

        /*
         * Author: Michelle Vuong
         * Description: Blink. When the player presses the Caps lock key the player will blink and shift.
         */
        if (Input.GetKeyDown(KeyCode.CapsLock)) //pressing down capslock
        {
            if (currentDirection == PlayerDirection.left) //when the player is facing left
            {
                transform.position = new Vector2(transform.position.x - 2, transform.position.y); //blink in the players direction
                body.AddForce(Vector2.left * force, ForceMode2D.Force);
                
            }

            else if (currentDirection == PlayerDirection.right) //when the player is facing left
            {
                transform.position = new Vector2(transform.position.x + 2, transform.position.y); //blink in the players direction
                body.AddForce(Vector2.right * force, ForceMode2D.Force);
            }
        }

        
    }

    /*
     * Authors: Michelle Vuong, Platformer final import
     * I adjusted the JumpUpdate() method to try to turn it into variable jumping.
     * The regular jump logic is still in there from the import, just adjusted.
     * Description: Variable jump. Holding down the space key for longer makes you jump more
     */

    private void JumpUpdate()
    {
        if (isGrounded && Input.GetKey(KeyCode.Space)) //as the player holds down the space bar and is on the ground
        {
            velocity.y += jumpDistance + 1f; //constantly add more height to the jump depending on how long the player holds it down for
           
            if (velocity.y >= initialJumpSpeed) //if the jump velocity ever goes over the regular jump speed
            {
                velocity.y = initialJumpSpeed; //set it to the max jump velocity
            }
            if (Input.GetKeyUp(KeyCode.Space)) //letting go of space bar
            {
                ResetJump(); //reset the velocity back to 0 so it the jump does not keep going higher
            }
            isGrounded = false;
        }

    }
    private void ResetJump() //resetting velocity back to 0, for readability.
    {
        velocity.y = 0;
    }
    /*
     * Author: Michelle Vuong
     * Description: Dash. Holding left shift makes the player move forward by the amount of dashDistance.
     */
    private void Dash()
    { 
        if (Input.GetKeyDown(KeyCode.LeftShift) && isGrounded) //check if the player is on the left side of the scene and if the player is on the ground
        {
            if (body.transform.position.x < 0) //check if the player is on the left side of the scene
            {
                if(currentDirection == PlayerDirection.left) //if the player is facing left
                {
                    velocity = new Vector2(transform.position.x * dashDistance, transform.position.y); //dash in the current direction of the player
                }
                if(currentDirection == PlayerDirection.right) //if the player is facing right
                {
                    velocity = new Vector2(transform.position.x * -dashDistance, transform.position.y); //dash in the current direction of the player
                }
               
            }

            else if (body.transform.position.x >= 0 && isGrounded) //check if the player is on the right side of the scene and if the player is on the ground
            {
                if (currentDirection == PlayerDirection.left) //if the player is facing left
                {
                    velocity = new Vector2(transform.position.x * -dashDistance, transform.position.y); //dash in the current direction of the player
                }
                if (currentDirection == PlayerDirection.right) //if the player is facing right
                {
                    velocity = new Vector2(transform.position.x * dashDistance, transform.position.y); //dash in the current direction of the player
                }
            }
        }
    }

    private void CheckForGround()
    {
        isGrounded = Physics2D.OverlapBox(
            transform.position + Vector3.down * groundCheckOffset,
            groundCheckSize,
            0,
            groundCheckMask);
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position + Vector3.down * groundCheckOffset, groundCheckSize);
    }

    public bool IsWalking()
    {
        return velocity.x != 0;
    }
    public bool IsGrounded()
    {
        return isGrounded;
    }

    public PlayerDirection GetFacingDirection()
    {
        return currentDirection;
    }
}
