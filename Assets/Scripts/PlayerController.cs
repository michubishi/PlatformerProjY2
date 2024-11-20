using Unity.Burst.CompilerServices;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rigidbody;
    public float speed = 0.02f;
    public bool isWalking = false;
    public bool isGrounded = false;
    public GameObject GroundTilemap;
    
    public enum FacingDirection
    {
        left, right
    }
    public FacingDirection direction;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();

    }
   

    // Update is called once per frame
    void Update()
    {
        // The input from the player needs to be determined and
        // then passed in the to the MovementUpdate which should
        // manage the actual movement of the character.

        
       Vector2 playerInput = new Vector2(rigidbody.transform.position.x, rigidbody.transform.position.y);
       MovementUpdate(playerInput);
       Debug.DrawRay(rigidbody.transform.position, Vector2.down, Color.white);

    }

    private void MovementUpdate(Vector2 playerInput)
    {
        rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        if (Input.GetKey(KeyCode.A))
        {
            rigidbody.transform.position = new Vector2(rigidbody.transform.position.x - speed, rigidbody.transform.position.y);
            isWalking = true;
            direction = FacingDirection.left;
        }

        else if (Input.GetKey(KeyCode.D))
        {
            rigidbody.transform.position = new Vector2(rigidbody.transform.position.x + speed, rigidbody.transform.position.y);
            isWalking = true;
            direction = FacingDirection.right;
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            rigidbody.AddForce(Vector2.up * 300);
            RaycastHit2D hit = Physics2D.Raycast(rigidbody.transform.position, Vector2.down);
            if(hit.collider.tag == "GroundTilemap")
            {
                isGrounded = true;
            }
            if(hit.collider.tag != "GroundTilemap")
            {
                isGrounded = false;
                Debug.Log("Im in the air");
            }
            Debug.Log(isGrounded);
        }
    }

    public bool IsWalking()
    {
        return isWalking;
    }
    public bool IsGrounded()
    {
        return isGrounded;
    }

    public FacingDirection GetFacingDirection()
    {
        
        return direction;
    }
}
