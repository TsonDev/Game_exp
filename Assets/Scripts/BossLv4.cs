using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossLv4 : MonoBehaviour
{
    [Header("Zones")]
    public Transform damageZone;
    public Transform teleportZone;

    [Header("Health")]
    public float maxHealth = 100f;
    private float currentHealth;
    //public Slider healthSlider; // UI thanh máu Boss

    [Header("Combat")]
    public float teleportCooldownMin = 5f;
    public float teleportCooldownMax = 10f;
    public int damageAmount = -20;

    [Header("FX")]
    public GameObject disappearEffect;
    public GameObject appearEffect;
    public GameObject deathEffect;

    [Header("Item Drop")]
    [SerializeField] private GameObject[] dropItems;
    [SerializeField] private float dropRate = 0.5f;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Vector3 originalPosition;
    private Vector3 originalScale;
    private GameObject player;
    private float nextTeleportTime = 0f;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalPosition = transform.position;
        originalScale = transform.localScale;
        player = GameObject.FindGameObjectWithTag("Player");

        //currentHealth = maxHealth;
        //if (healthSlider != null)
        //{
        //    healthSlider.maxValue = maxHealth;
        //    healthSlider.value = currentHealth;
        //}

        StartCoroutine(BossAttackRoutine());
    }

    void Update()
    {
        if (Time.time >= nextTeleportTime)
        {
            StartCoroutine(TeleportAndAttack());
            nextTeleportTime = Time.time + Random.Range(teleportCooldownMin, teleportCooldownMax);
        }
    }

    IEnumerator BossAttackRoutine()
    {
        while (true)
        {
            yield return null;
        }
    }

    IEnumerator TeleportAndAttack()
    {
        if (player == null || teleportZone == null)
            yield break;

        if (!IsInTeleportZone(player.transform.position))
        {
            transform.position = originalPosition;
            transform.localScale = originalScale;
            animator.Play("BossIdle");
            yield break;
        }

        if (disappearEffect != null)
            Instantiate(disappearEffect, transform.position, Quaternion.identity);

        spriteRenderer.enabled = false;
        yield return new WaitForSeconds(0.3f);

        Vector3 targetPos = player.transform.position + new Vector3(Random.Range(-1.5f, 1.5f), 0, 0);
        targetPos = ClampPositionToTeleportZone(targetPos);
        targetPos.y = originalPosition.y;

        transform.position = targetPos;

        if (appearEffect != null)
            Instantiate(appearEffect, transform.position, Quaternion.identity);

        spriteRenderer.enabled = true;

        Vector3 dir = player.transform.position - transform.position;
        if (dir.x < 0)
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        else
            transform.localScale = originalScale;

        if (damageZone != null)
        {
            float offset = 1.5f * Mathf.Sign(transform.localScale.x);
            damageZone.position = transform.position + new Vector3(offset, 0, 0);
        }

        yield return new WaitForSeconds(2f);

        string skill = Random.value < 0.5f ? "BossSkill1" : "BossSkill2";
        animator.Play(skill);

        yield return new WaitForSeconds(0.3f);
        TryDamagePlayer();
    }

    void TryDamagePlayer()
    {
        if (player == null || damageZone == null) return;

        Vector2 center = damageZone.position;
        Vector2 size = damageZone.localScale;

        Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, 0);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Debug.Log("Boss gây sát thương!");
                hit.GetComponent<PlayerController>()?.ChangeHealth(damageAmount);
            }
        }
    }

    public void ChangeHealth(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);

        //if (healthSlider != null)
        //    healthSlider.value = currentHealth;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Boss chết!");
        animator.SetTrigger("Hit");

        if (deathEffect != null)
            Instantiate(deathEffect, transform.position, Quaternion.identity);

        TryDropItem();
        Destroy(gameObject, 1.5f);
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
                Vector3 dropPos = transform.position + new Vector3(dropCount * 0.5f, 0, 0);
                GameObject drop = Instantiate(item, dropPos, Quaternion.identity);

                Rigidbody2D rb = drop.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    float forceX = Random.Range(-2f, 2f);
                    float forceY = Random.Range(3f, 6f);
                    rb.AddForce(new Vector2(forceX, forceY), ForceMode2D.Impulse);
                }

                dropCount++;
            }
        }
    }

    Vector3 ClampPositionToTeleportZone(Vector3 position)
    {
        Vector3 min = teleportZone.position - teleportZone.localScale / 2f;
        Vector3 max = teleportZone.position + teleportZone.localScale / 2f;

        position.x = Mathf.Clamp(position.x, min.x, max.x);
        position.y = Mathf.Clamp(position.y, min.y, max.y);
        return position;
    }

    bool IsInTeleportZone(Vector3 position)
    {
        Vector3 min = teleportZone.position - teleportZone.localScale / 2f;
        Vector3 max = teleportZone.position + teleportZone.localScale / 2f;

        return (position.x >= min.x && position.x <= max.x &&
                position.y >= min.y && position.y <= max.y);
    }

    void OnDrawGizmos()
    {
        if (damageZone != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(damageZone.position, damageZone.localScale);
        }

        if (teleportZone != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(teleportZone.position, teleportZone.localScale);
        }
    }
}
