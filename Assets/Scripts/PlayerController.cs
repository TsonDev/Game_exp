using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    


    Rigidbody2D rigid2d;
    Animator animator;
    bool isFaceRight = true;
 

    [Header("Movement")]
    public float moveSpeed = 5f;
    Vector2 moveDirection = new Vector2(1, 0);
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

    [Header("WallCheck")]
    public Transform wallCheckPos;
    public Vector2 wallCheckSize = new Vector2(0.5f, 0.5f);
    public LayerMask wallLayer;
    [Header("WallMoment")]
    public float MaxWallSpeed = 2f;
    bool isWallSlide;
    /*  Wall jump*/
    bool isWalljump;
    float wallJumpDirection;
    float wallJumpTime = 0.5f;
    float wallJumpTimer;
    public Vector2 wallJumpPower = new Vector2(5f,10f);


    [Header("Projectile")]
    public GameObject projectPrefab;

    [Header("Health")]
    float currentHealth;
    public float maxHealth = 10;
    public float Health {  get { return currentHealth; } }


    // Time invincible
    public float timeInvincible = 3.0f;
    bool isInvincible;
    float timeDamageCoolDown;

    // Health UI
    PlayerHealthUI pHealthUI;

    // Start is called before the first frame update
    void Start()
    {
       rigid2d = GetComponent<Rigidbody2D>(); 
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        timeDamageCoolDown=timeInvincible;
        pHealthUI = GetComponentInChildren<PlayerHealthUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
        /*if(!Mathf.Approximately(rigid2d.velocity.x,0.0f) ){
            moveDirection.Set(rigid2d.velocity.x, 0);
            moveDirection.Normalize();
        }*/
        animator.SetFloat("MoveX", moveDirection.x);


        /*if (rigid2d.velocity.x == 0)
        {
            animator.SetInteger("Speed", 0);

        }
        else
        {
            animator.SetInteger("Speed", 1);
            
        }*/
        /*float absVelX = Mathf.Abs(rigid2d.velocity.x);
        animator.SetInteger("Speed", absVelX > 0.1f ? 1 : 0);*/
        if (Mathf.Abs(horizontalMovement) > 0.1f && !isWallSlide)
        {
            animator.SetInteger("Speed", 1); // chạy
        }
        else
        {
            animator.SetInteger("Speed", 0); // đứng
        }

        GroundCheck();
        Gravity();
        WallSlide();
        WallJump();
        if(!isWalljump)
        {
            rigid2d.velocity = new Vector2(horizontalMovement * moveSpeed, rigid2d.velocity.y);
            if (!Mathf.Approximately(rigid2d.velocity.x, 0.0f))
            {
                moveDirection.Set(rigid2d.velocity.x, 0);
                moveDirection.Normalize();
            }
            Flip();
        }


        animator.SetBool("isJump", !IsCheckGround());

        if (isInvincible) { 
            timeDamageCoolDown-=Time.deltaTime;
            if (timeDamageCoolDown < 0) { 
            isInvincible = false;
            }
        }

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
        /*Wall jump*/
        if (context.performed && wallJumpTimer > 0)
        {
            isWalljump = true;
            rigid2d.velocity = new Vector2(wallJumpDirection*wallJumpPower.x,wallJumpPower.y);
            wallJumpTimer = 0;
            Invoke(nameof(CancelWallJump), wallJumpTime + 0.1f);//time wall jump 0.5f -- tim wall again 0.6f
            if (transform.localScale.x != wallJumpDirection)
            {
                isFaceRight = !isFaceRight;
                Vector3 ls = transform.localScale;
                ls.x *= -1;
                transform.localScale = ls;
            }
        }

    }



    private bool IsCheckGround()
    {
        if(Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0.0f, groundLayer))
        {

            return true;
        }  
        return false;
    }
    private bool IsCheckWall()
    {
        return Physics2D.OverlapBox(wallCheckPos.position, wallCheckSize, 0.0f, wallLayer);
        //{
           
        //    return true;
        //}
        //return false;
    }
    private void GroundCheck()
    {
        if(IsCheckGround())
        {
            JumpRemaining = numberJump;
        }        
        
    }
    void Gravity()
    {
        if (rigid2d.velocity.y < 0.0f)
        {
            rigid2d.gravityScale = baseGravity * fallSpeed;
            rigid2d.velocity = new Vector2(rigid2d.velocity.x, Mathf.Max(rigid2d.velocity.y, -MaxFallSpeed));
        }
        else rigid2d.gravityScale = baseGravity;
    }
    void WallSlide()
    {
        if(!IsCheckGround() && IsCheckWall() && horizontalMovement != 0)
        {
            rigid2d.velocity = new Vector2(rigid2d.velocity.x, Mathf.Max(rigid2d.velocity.y, -MaxWallSpeed));
            isWallSlide = true;
        }
        else isWallSlide = false;
    }
    void WallJump()
    {
        if (isWallSlide)
        {
            isWalljump = false;
            wallJumpDirection = -transform.localScale.x;
            wallJumpTimer = wallJumpTime;
            CancelInvoke(nameof(CancelWallJump));
        }
        else if(wallJumpTimer > 0)
        {
            wallJumpTimer -= Time.deltaTime;
        }
    }
    void CancelWallJump()
    {
        isWalljump = false ;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize); 
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(wallCheckPos.position, wallCheckSize);
    }
    

    public void Attack(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Performed)
        {
            return;
        }
        if (moveDirection == Vector2.zero)
            moveDirection = Vector2.left;
        GameObject gameObject = Instantiate(projectPrefab,rigid2d.position+Vector2.up*0.3f,Quaternion.identity);
        Projectile projectile = gameObject.GetComponent<Projectile>();
        projectile.Launch(moveDirection, 1000);
       /* animator.SetTrigger("Attack");*/
        
    }   
    public void ChangeHealth(int mount)
    {
        if (mount < 0)
        {
            if (isInvincible)
            {
                return;
            }
            isInvincible = true;
            timeDamageCoolDown = timeInvincible;
        }
        currentHealth = Mathf.Clamp(currentHealth + mount, 0, maxHealth);
        Debug.Log(currentHealth);

        if (pHealthUI != null)
        {
            pHealthUI.SetHealth(currentHealth, maxHealth);
        }
    }   
    private void Flip()
    {
        // Nếu đang quay mặt phải mà đi sang trái, hoặc đang quay mặt trái mà đi sang phải
       /* if (horizontalMovement > 0)
        {
            transform.localScale = new Vector3(1, 1, 1); // Quay phải
           
        }
        else if (horizontalMovement < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1); // Quay trái
           
        }*/

        if(isFaceRight && horizontalMovement<0 || !isFaceRight && horizontalMovement>0)
        {
            isFaceRight = !isFaceRight;
            Vector3 ls = transform.localScale;
            ls.x*=-1;
            transform.localScale = ls;
        }
    }
}
