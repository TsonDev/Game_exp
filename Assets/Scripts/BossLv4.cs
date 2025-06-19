using System.Collections;
using UnityEngine;

public class BossLv4 : MonoBehaviour
{
    public Transform damageZone;
    public Transform teleportZone;
    public float teleportCooldownMin = 5f;
    public float teleportCooldownMax = 10f;
    public int damageAmount = -20;

    public GameObject disappearEffect;
    public GameObject appearEffect;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Vector3 originalPosition;
    private Vector3 originalScale;
    private Vector3 maxScale = new Vector3(50f, 50f, 1f);
    private GameObject player;
    private float nextTeleportTime = 0f;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalPosition = transform.position;
        originalScale = transform.localScale;
        player = GameObject.FindGameObjectWithTag("Player");

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

        // Nếu Player nằm ngoài vùng → quay về gốc và idle
        if (!IsInTeleportZone(player.transform.position))
        {
            transform.position = originalPosition;
            transform.localScale = originalScale;
            animator.Play("BossIdle");
            yield break;
        }

        // Hiệu ứng biến mất
        if (disappearEffect != null)
            Instantiate(disappearEffect, transform.position, Quaternion.identity);

        spriteRenderer.enabled = false;
        yield return new WaitForSeconds(0.3f);

        // Tính vị trí gần Player nhưng nằm trong vùng teleport
        Vector3 targetPos = player.transform.position + new Vector3(Random.Range(-1.5f, 1.5f), 0, 0);
        targetPos = ClampPositionToTeleportZone(targetPos);
        targetPos.y = originalPosition.y; // tránh chui xuống đất

        transform.position = targetPos;

        // Hiệu ứng xuất hiện
        if (appearEffect != null)
            Instantiate(appearEffect, transform.position, Quaternion.identity);

        spriteRenderer.enabled = true;

        // Hướng Boss về phía Player
        Vector3 dir = player.transform.position - transform.position;
        if (dir.x < 0)
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        else
            transform.localScale = originalScale;

        // Di chuyển vùng damage theo
        if (damageZone != null)
        {
            float offset = 1.5f * Mathf.Sign(transform.localScale.x);
            damageZone.position = transform.position + new Vector3(offset, 0, 0);
        }

        yield return new WaitForSeconds(2f);

        // Random Skill
        string skill = Random.value < 0.5f ? "BossSkill1" : "BossSkill2";
        animator.Play(skill);

        yield return new WaitForSeconds(0.3f);
        TryDamagePlayer();
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

    void TryDamagePlayer()
    {
        if (player == null) return;

        Collider2D playerCol = player.GetComponent<Collider2D>();
        Collider2D dmgCol = damageZone.GetComponent<Collider2D>();

        if (playerCol != null && dmgCol != null && playerCol.IsTouching(dmgCol))
        {
            Debug.Log("Boss gây sát thương!");
            player.GetComponent<PlayerController>()?.ChangeHealth(damageAmount);
        }
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
