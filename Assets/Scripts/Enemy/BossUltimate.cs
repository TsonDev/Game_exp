using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossUltimate : MonoBehaviour
{
    [Header("General")]
    public Transform player;
    public LayerMask bulletLayer;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float patrolRange = 4f;
    private Vector2 startPos;
    private bool isFacingRight = true;

    [Header("Health")]
    public float maxHealth = 10f;
    private float currentHealth;
    public Image healthFill;

    [Header("Attack")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 5f;
    public float fireRate = 1f;
    private float fireCooldown;

    [Header("AOE Attack")]
    public float aoeRadius = 2f;
    public int aoeDamage = 2;
    public float aoeCooldown = 6f;
    private float aoeTimer;
    public LayerMask playerLayer;

    [Header("Jump to Avoid")]
    public float jumpForce = 7f;
    private Rigidbody2D rb;
    public float findBullet = 3f;
   

    [Header("Phase 2 Boost")]
    private bool isPhase2 = false;

    [Header("Effects")]
    public GameObject aoeEffect;
    public GameObject deathVFX;
    public AudioClip shootSFX, aoeSFX, deathSFX;
    private AudioSource audioSource;
    [Header("Detection")]
    public float detectionRange = 12f;
    [Header("Phase 2 Clone")]
    public GameObject clonePrefab;
    public int numberOfClones = 2;
    public float cloneSpawnRadius = 2f;



    private Animator animator;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        startPos = transform.position;
        currentHealth = maxHealth;
        fireCooldown = 0f;
        aoeTimer = aoeCooldown;

        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();

        if (healthFill) healthFill.fillAmount = 1f;
    }

    void Update()
    {
        if (!player) return;

        float distance = Vector2.Distance(transform.position, player.position);

        fireCooldown -= Time.deltaTime;
        aoeTimer -= Time.deltaTime;

        if (CheckIncomingBullet())
        {
            JumpAvoid();
            return;
        }

        if (distance < detectionRange)
        {
            LookAtPlayer();
            animator?.SetInteger("Speed", 0);

            if (fireCooldown <= 0) Fire();
            if (aoeTimer <= 0) AOEAttack();
        }
        else
        {
            Patrol();
            animator?.SetInteger("Speed", 1);
        }

        if (!isPhase2 && currentHealth <= maxHealth / 2f)
        {
            EnterPhase2();
        }

        if (healthFill) healthFill.fillAmount = currentHealth / maxHealth;
        bool grounded = IsGrounded();
        animator?.SetBool("isGrounded", grounded);

    }

    void Patrol()
    {
        float dir = isFacingRight ? 1 : -1;
        transform.Translate(Vector2.right * dir * moveSpeed * Time.deltaTime);

        if (Mathf.Abs(transform.position.x - startPos.x) >= patrolRange)
        {
            Flip();
        }
    }

    void LookAtPlayer()
    {
        if (player.position.x > transform.position.x && !isFacingRight)
            Flip();
        else if (player.position.x < transform.position.x && isFacingRight)
            Flip();
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void Fire()
    {
        if (!bulletPrefab || !firePoint) return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();
        Vector2 dir = Vector2.right * (isFacingRight ? 1 : -1);
        if (rbBullet) rbBullet.velocity = dir * bulletSpeed;

        if (shootSFX) audioSource.PlayOneShot(shootSFX);
        animator?.SetTrigger("Attack");

        fireCooldown = 1f / fireRate;
    }

    void AOEAttack()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, aoeRadius, playerLayer);
        if (hit)
        {
            hit.SendMessage("ChangeHealth", -aoeDamage, SendMessageOptions.DontRequireReceiver);
        }

        if (aoeEffect) Instantiate(aoeEffect, transform.position, Quaternion.identity);
        if (aoeSFX) audioSource.PlayOneShot(aoeSFX);
        animator?.SetTrigger("Attack1");

        aoeTimer = aoeCooldown;
    }

    bool CheckIncomingBullet()
    {
        Collider2D[] bullets = Physics2D.OverlapCircleAll(transform.position, findBullet, bulletLayer);
        foreach (var b in bullets)
        {
            Rigidbody2D bulletRb = b.GetComponent<Rigidbody2D>();
            if (bulletRb == null)
            {
                
                continue;
            }

                Vector2 bulletDir = bulletRb.velocity;
            Vector2 toBoss = transform.position - b.transform.position;

            if (Vector2.Dot(toBoss.normalized, bulletDir.normalized) > 0.8f)
            {
                return true; // Đạn đang bay về phía boss
            }
        }
        return false;
    }

    void JumpAvoid()
    {
        if (IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            animator?.SetTrigger("Jump"); // Dùng lại anim Dash để biểu diễn né
        }
        else { }

    }

    bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 10f, LayerMask.GetMask("Ground"));
        return hit.collider != null;
    }

    public void ChangeHealth(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        Debug.Log("Da danh trung"
            +
            currentHealth);

        if (healthFill)
            healthFill.fillAmount = currentHealth / maxHealth;

        if (currentHealth <= 0)
        {
            if (deathVFX) Instantiate(deathVFX, transform.position, Quaternion.identity);
            if (deathSFX) AudioSource.PlayClipAtPoint(deathSFX, transform.position);
            Destroy(gameObject);
        }
    }

    void EnterPhase2()
    {
        isPhase2 = true;
        fireRate *= 1.5f;
        aoeCooldown *= 0.75f;
        moveSpeed *= 1.25f;
        aoeDamage = Mathf.RoundToInt(aoeDamage * 1.5f);

        Debug.Log("Boss vào Phase 2!");

        SpawnClones();
    }
    void SpawnClones()
    {
        if (clonePrefab == null) return;

        for (int i = 0; i < numberOfClones; i++)
        {
            Vector2 spawnPos = (Vector2)transform.position + Random.insideUnitCircle * cloneSpawnRadius;
            Instantiate(clonePrefab, spawnPos, Quaternion.identity);
        }
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aoeRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, findBullet); // vùng phát hiện đạn
    }
}
