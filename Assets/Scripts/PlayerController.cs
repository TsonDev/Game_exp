using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    


    Rigidbody2D rigid2d;
   
    Animator animator;

    [Header("Movement")]
    public float moveSpeed = 5f;
    Vector2 moveDirection = new Vector2(0, 0);
    float horizontalMovement;
    [Header("Jump")]
    public float jumpPower = 10f;
    public int numberJump = 2;
    int JumpRemaining;
    [Header("GroundCheck")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize =new Vector2 (0.5f,0.5f);
    public LayerMask groundLayer;
    [Header("Gravity")]
    public float baseGravity = 2f;
    public float MaxFallSpeed = 18f;
    public float fallSpeed = 2f;

    // Start is called before the first frame update
    void Start()
    {
       rigid2d = GetComponent<Rigidbody2D>(); 
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        rigid2d.velocity =new Vector2( horizontalMovement * moveSpeed,rigid2d.velocity.y); 
        if(!Mathf.Approximately(rigid2d.velocity.x,0.0f) ){
            moveDirection.Set(rigid2d.velocity.x,0);
            moveDirection.Normalize();
        }
        animator.SetFloat("MoveX", moveDirection.x);
        if (rigid2d.velocity.x == 0)
        {
            animator.SetInteger("Speed", 0);

        }
        else
        {
            animator.SetInteger("Speed", 1);
            
        }
        GroundCheck();
        Gravity();
        animator.SetBool("isJump", !IsCheck());

    }
    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
       
    }
    public void Jump(InputAction.CallbackContext context)
    {
        if (JumpRemaining > 0)
        {
            if (context.performed)
            {
                rigid2d.velocity = new Vector2(rigid2d.velocity.x, jumpPower);
                JumpRemaining--;
            }
            else if (context.canceled)
            {
                rigid2d.velocity = new Vector2(rigid2d.velocity.x, rigid2d.velocity.y * 0.5f);
                JumpRemaining--;
            }
        }

    }
    private bool IsCheck()
    {
        if(Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0.0f, groundLayer))
        {
            return true;
        }  
        return false;
    }
    private void GroundCheck()
    {
        if(IsCheck())
        {
            JumpRemaining = numberJump;
        }        
        
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
    }
    void Gravity()
    {
        if (rigid2d.velocity.y < 0.0f)
        {
            rigid2d.gravityScale = baseGravity * fallSpeed;
            rigid2d.velocity = new Vector2(rigid2d.velocity.x, Mathf.Max(rigid2d.velocity.y, -MaxFallSpeed));
        }
    }
}
