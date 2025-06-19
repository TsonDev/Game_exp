using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement")]
    public Transform player;
    public float chaseSpeed = 2f;
    public float jumpForce = 2f;
    public LayerMask groundLayer;

    [Header("Health")]
    public float maxHealth = 5f;
    private float currentHealth;

    [Header("Drop Settings")]
    [SerializeField] private GameObject[] dropItems;
    [SerializeField] private float dropRate = 0.5f; // Xác suất rơi mỗi item

    private Rigidbody2D rigid2d;
    private Animator animator;
    private AudioSource source;
    private bool isGrounded;
    private bool shouldJump;
    private Vector2 movedirection = Vector2.right;

    private EnemyHealthUI healthUI;

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

        float direction = Mathf.Sign(player.position.x - transform.position.x);
        bool isPlayerAbove = Physics2D.Raycast(transform.position, Vector2.up, 5f, 1 << player.gameObject.layer);

        if (isGrounded)
        {
            rigid2d.velocity = new Vector2(direction * chaseSpeed, rigid2d.velocity.y);

            RaycastHit2D groundInFront = Physics2D.Raycast(transform.position, new Vector2(direction, 0), 2f, groundLayer);
            RaycastHit2D gapAhead = Physics2D.Raycast(transform.position + new Vector3(direction, 0, 0), Vector2.down, 2f, groundLayer);
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

            Vector2 direction = (player.position - transform.position).normalized;
            Vector2 jumpDirection = direction * jumpForce;

            rigid2d.AddForce(new Vector2(jumpDirection.x, jumpForce), ForceMode2D.Impulse);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Projectile projectile = other.gameObject.GetComponent<Projectile>();
        if (projectile != null)
        {
            if (source != null)
                source.Play();
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

        if (healthUI != null)
        {
            healthUI.SetHealth(currentHealth, maxHealth);
        }

        if (source != null)
            source.Play();

        if (currentHealth <= 0f)
        {
            TryDropItem();
            animator.SetTrigger("Hit");
            Destroy(gameObject, 1f);
        }
    }

    void TryDropItem()
    {
        Debug.Log("Trying to drop item");

        if (dropItems.Length == 0) return;

        int dropCount = 0;
        foreach (GameObject item in dropItems)
        {
            if (item == null) continue;
            if (Random.value <= dropRate)
            {
                // Offset nhỏ theo chiều X để tách item ra (dựa vào số lượng đã spawn)
                Vector3 dropPos = transform.position + new Vector3(dropCount * 0.5f, 0, 0);

                GameObject drop = Instantiate(item, dropPos, Quaternion.identity);
                Debug.Log("Dropped: " + drop.name);

                Rigidbody2D rb = drop.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    float forceX = Random.Range(-2f, 2f);
                    float forceY = Random.Range(3f, 6f);
                    rb.AddForce(new Vector2(forceX, forceY), ForceMode2D.Impulse);
                }

                dropCount++; // Đếm số lượng đã drop
            }
        }
    }


}
