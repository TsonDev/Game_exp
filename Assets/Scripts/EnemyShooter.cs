using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 1f;
    public float bulletSpeed = 5f;
    public float detectRange = 5f;

    public float moveSpeed = 2f;
    public float patrolRange = 3f;

    private Vector2 startPos;
    private float fireCooldown = 0f;
    private Transform player;

    private enum State { Patrol, Attack }
    private State currentState = State.Patrol;

    private Animator animator;

    void Start()
    {
        startPos = transform.position;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // Chuyển trạng thái theo khoảng cách
        currentState = distance <= detectRange ? State.Attack : State.Patrol;

        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                animator?.SetBool("isAttacking", false);
                animator?.SetBool("isWalking", true);
                break;

            case State.Attack:
                LookAtPlayer();
                animator?.SetBool("isWalking", false);
                animator?.SetBool("isAttacking", true);
                Attack();
                break;
        }

        fireCooldown -= Time.deltaTime;
    }

    void Patrol()
    {
        float dir = Mathf.Sign(transform.localScale.x);
        transform.Translate(Vector2.right * dir * moveSpeed * Time.deltaTime);

        // Đảo hướng nếu vượt quá phạm vi patrol
        if (Mathf.Abs(transform.position.x - startPos.x) >= patrolRange)
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    void LookAtPlayer()
    {
        Vector3 scale = transform.localScale;
        if (player.position.x > transform.position.x)
            scale.x = Mathf.Abs(scale.x);   // quay phải
        else
            scale.x = -Mathf.Abs(scale.x);  // quay trái

        transform.localScale = scale;
    }

    void Attack()
    {
        if (fireCooldown > 0f || bulletPrefab == null || firePoint == null) return;

        float facingDir = transform.localScale.x < 0 ? -1f : 1f;
        Vector2 shootDir = new Vector2(facingDir, 0f);

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.velocity = shootDir * bulletSpeed;

        // Flip hướng đạn
        bullet.transform.localScale = new Vector3(
            facingDir * Mathf.Abs(bullet.transform.localScale.x),
            bullet.transform.localScale.y,
            bullet.transform.localScale.z
        );

        fireCooldown = 1f / fireRate;
    }
}
