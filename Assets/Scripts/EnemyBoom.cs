using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoom : MonoBehaviour
{
    public Transform player;
    public float chaseSpeed = 3f;
    public float explosionRange = 2f;
    public float explosionDelay = 1f;
    public int explosionDamage = 2;

    private Rigidbody2D rb;
    private Animator animator;
    private bool isExploding = false;
    private SpriteRenderer spriteRenderer;

    public GameObject explosionEffect;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isExploding || player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= explosionRange)
        {
            StartCoroutine(Explode());
        }
        else
        {
            Vector2 dir = (player.position - transform.position).normalized;
            rb.velocity = new Vector2(dir.x * chaseSpeed, rb.velocity.y);
            spriteRenderer.flipX = dir.x < 0;
        }
    }

    IEnumerator Explode()
    {
        Debug.Log("Enemy chuẩn bị phát nổ!");
        isExploding = true;
        rb.velocity = Vector2.zero;

        // Optional: cảnh báo nhấp nháy
        float flashTime = 0.1f;
        for (int i = 0; i < (explosionDelay / (flashTime * 2)); i++)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(flashTime);
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(flashTime);
        }

        // Tạo hiệu ứng nổ
        if (explosionEffect != null)
        {
            GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(explosion, 1f);
        }

        // Gây sát thương
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(transform.position, explosionRange, LayerMask.GetMask("Player"));
        foreach (var hit in hitPlayers)
        {
            PlayerController pc = hit.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.ChangeHealth(-explosionDamage);
            }
        }

        Destroy(gameObject); // Tự hủy
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}
