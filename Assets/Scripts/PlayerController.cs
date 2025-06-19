using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    


    Rigidbody2D rigid2d;
    Animator animator;
    bool isFaceRight = true;
    [Header("UI")]
   UIGameover uIGameover;

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
    [Header("Mana")]
    public float costMana = 15;
    float currentMana;
    public float maxMana = 100;
    public float Mana {  get { return currentMana; } }

    [Header("Attack")]
    public Transform attackPoint; // Kéo một Empty GameObject vào đây, đặt trước mặt nhân vật
    public float attackRange = 0.5f;
    public int attackDamage = 1;
    public LayerMask enemyLayers;
    public float cooldownAttack = 1;
    private float cooldownAttackTimer;
    bool isCoolDownAttack = false;
    
    [Header("Dash")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    private bool isDashing = false;
    private bool canDash = true;
    private float dashCooldownTimer;
    private Vector2 dashDirection;

    [Header("Dash Stack")]
    public int maxDashStack = 4;
    private int currentDashStack;
    public float dashStackRecoveryTime = 2f; // thời gian hồi 1 stack
    private float dashStackTimer;



    // Time invincible
    public float timeInvincible = 3f;
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
        currentMana = maxMana;
        timeDamageCoolDown=timeInvincible;
        pHealthUI = GetComponentInChildren<PlayerHealthUI>();
        uIGameover = FindObjectOfType<UIGameover>();

        currentDashStack = maxDashStack;
        dashStackTimer = 0f;


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
        if (isCoolDownAttack)
        {
            cooldownAttackTimer -= Time.deltaTime;
            if (cooldownAttackTimer < 0)
            {
                isCoolDownAttack = false;
            }
        }
        // Hồi dash stack
        //if (currentDashStack < maxDashStack)
        //{
        //    dashStackTimer += Time.deltaTime;
        //    if (dashStackTimer >= dashStackRecoveryTime)
        //    {
        //        currentDashStack++;
        //        dashStackTimer = 0f;

        //        // Cập nhật UI sau khi hồi
        //        UIHandler.instance.SetStackValue((float)currentDashStack / maxDashStack);
        //    }
        //}



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
        if (currentMana < 15)
        {
            Debug.Log("Ko du mana");
            return;
        }
        if (isCoolDownAttack)
        {
            return;
        }
        currentMana -= costMana;
        if (moveDirection == Vector2.zero)
            moveDirection = Vector2.left;
        GameObject gameObject = Instantiate(projectPrefab,rigid2d.position+Vector2.up*0.3f,Quaternion.identity);
        Projectile projectile = gameObject.GetComponent<Projectile>();
        projectile.Launch(moveDirection, 1000);
        
        UIHandler.instance.SetManaValue(currentMana / (float)maxMana);
        animator.SetTrigger("Attack");
        cooldownAttackTimer = cooldownAttack;
        isCoolDownAttack = true;

    }   
    public void Attack1(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Performed)
        {
            return;
        }
        if (moveDirection == Vector2.zero)
            moveDirection = Vector2.left;
        /*GameObject gameObject = Instantiate(projectPrefab,rigid2d.position+Vector2.up*0.3f,Quaternion.identity);
        Projectile projectile = gameObject.GetComponent<Projectile>();
        projectile.Launch(moveDirection, 1000);*/
        MeleeAttack();
      

    }
    public void MeleeAttack()
    {
        // Phát hiện tất cả enemy trong phạm vi tấn công
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                enemyAI.ChangeHealth(-attackDamage);
            }
            Enemy enemy1 = enemy.GetComponent<Enemy>();
            if(enemy1)
            {
                Destroy(enemy1.gameObject); 
            }
        }

        // Kích hoạt animation đánh
        animator.SetTrigger("Attack1");
    }
    public void OnDash(InputAction.CallbackContext context)
    {
        Dash(context);
    }


public void Dash(InputAction.CallbackContext context)
{
    if (context.phase != InputActionPhase.Performed || isDashing || currentDashStack <= 0)
        return;

    if (moveDirection == Vector2.zero)
        moveDirection = isFaceRight ? Vector2.right : Vector2.left;

    currentDashStack--;
    UIHandler.instance.SetStackValue((float)currentDashStack / maxDashStack); // cập nhật UI
    StartCoroutine(DashRoutine());
}


    IEnumerator DashRoutine()
    {
        isDashing = true;
        canDash = false;
        isInvincible = true;

        dashDirection = moveDirection.normalized;
        float dashEndTime = Time.time + dashDuration;

        animator.SetTrigger("Dash"); // nếu có animation

        while (Time.time < dashEndTime)
        {
            rigid2d.velocity = dashDirection * dashSpeed;
            yield return null;
        }

        rigid2d.velocity = Vector2.zero;
        isDashing = false;
        isInvincible = false;

        // Cooldown
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
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
        if(currentHealth <= 0)
        {
            currentHealth = 0;
            uIGameover.ShowGameOver();
            Time.timeScale = 0f;
        }
        UIHandler.instance.SetHealthValue(currentHealth/(float)maxHealth);
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
    public void RestoreMana(float amount)
    {
        currentMana = Mathf.Clamp(currentMana + amount, 0, maxMana);
        UIHandler.instance.SetManaValue(currentMana / maxMana);
    }
    public void AddDashStack(int amount)
    {
        currentDashStack = Mathf.Clamp(currentDashStack + amount, 0, maxDashStack);
        UIHandler.instance.SetStackValue((float)currentDashStack / maxDashStack);
    }
    public IEnumerator SpeedBoost(float multiplier, float duration)
    {
        float originalSpeed = moveSpeed;
        moveSpeed *= multiplier;
        yield return new WaitForSeconds(duration);
        moveSpeed = originalSpeed;
    }


}
