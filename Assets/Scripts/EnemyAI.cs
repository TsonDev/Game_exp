using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public float chaseSpeed = 2f;
    public float jumpForce = 2f;
    public LayerMask groundLayer;
    public float  maxHealth = 5f;
    
     Rigidbody2D rigid2d;
    float currentHealth;
    bool isGrounded;
    bool shouldJump;
    Animator animator;
    AudioSource source;
    Vector2 movedirection = new Vector2(1,0);

    EnemyHealthUI healthUI;


    void Start()
    {
        rigid2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        source = GetComponent<AudioSource>();
        currentHealth = maxHealth;
        healthUI = GetComponentInChildren<EnemyHealthUI>();

    }

    void Update()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 10f, groundLayer);
       /* Debug.Log(isGrounded);*/

        //player direction 
        float direction =Mathf.Sign(player.position.x- transform.position.x);
        //Player dectection
        bool isPlayerAbove = Physics2D.Raycast(transform.position, Vector2.up,5f, 1<<player.gameObject.layer);
        if (isGrounded)
        {
            rigid2d.velocity = new Vector2(direction*chaseSpeed,rigid2d.velocity.y);
            //Kiểm tra đất
            RaycastHit2D groundInFront = Physics2D.Raycast(transform.position, new Vector2(direction, 0), 2f, groundLayer);
            // Kiểm tra khoảng trống phía trước
            RaycastHit2D gapAhead = Physics2D.Raycast(transform.position + new Vector3(direction, 0, 0), Vector2.down, 2f, groundLayer);

            // Kiểm tra nền phía trên
            RaycastHit2D platformAbove = Physics2D.Raycast(transform.position, Vector2.up, 5f, groundLayer);

            if (!groundInFront.collider && !gapAhead.collider)
            {
                shouldJump = true;
            }
            else if (isPlayerAbove && platformAbove.collider)
            {
                shouldJump = true;
            }

        }
        movedirection.Set(direction, 0);
        animator.SetFloat("LookX", movedirection.x);
    }
    private void FixedUpdate()
    {
        if (isGrounded && shouldJump)
        {
            shouldJump = false;

            // Tính hướng từ AI đến người chơi
             Vector2 direction = (player.position - transform.position).normalized;
            

            // Nhân hướng với lực nhảy
            Vector2 jumpDirection = direction * jumpForce;

            // Thêm lực nhảy cho Rigidbody2D
            rigid2d.AddForce(new Vector2(jumpDirection.x, jumpForce),ForceMode2D.Impulse);
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        
        Projectile projectile = other.gameObject.GetComponent<Projectile>();
        if (projectile != null)
        {

            

            if (source != null)
            {
                source.Play();
                Debug.Log("y");
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();


        if (player != null)
        {
            player.ChangeHealth(-1);

           
        }
    }
    public void ChangeHealth(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0f, maxHealth);

        // Cập nhật giao diện máu
        if (healthUI != null)
        {
            healthUI.SetHealth(currentHealth, maxHealth);
            
        }
        Debug.Log("ui ai");


        if (currentHealth <= 0f)
        {
            animator.SetTrigger("Hit");
            Destroy(gameObject, 1f);
        }
        if (source != null)
        {
            source.Play();
            Debug.Log("y");
        }
    }



}