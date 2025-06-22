using System.Collections;
using UnityEngine;


public class Da : MonoBehaviour
{
    public float explodeAfter = 2f;
    public float respawnAfter = 3f;
    public int damage = 50;
    public float radius = 2f;
    public GameObject explosionAnimPrefab; // prefab chứa Animation clip nổ
    public LayerMask destructibleLayer;
    private AudioSource audioEx;

    private SpriteRenderer sr;
    private Coroutine explodeRoutine;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        audioEx = GetComponent<AudioSource>();
        Debug.Log("Script Da đã chạy!");
    }

    void OnCollisionStay2D(Collision2D col)
    {
        if (col.collider.CompareTag("Player") && explodeRoutine == null)
        {
            Debug.Log("Nổ!");
            explodeRoutine = StartCoroutine(ExplodeSequence());
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.collider.CompareTag("Player") && explodeRoutine != null)
        {
            StopCoroutine(explodeRoutine);
            explodeRoutine = null;
            if (sr) sr.color = Color.white;
        }
    }

    IEnumerator ExplodeSequence()
    {
        float blinkTime = 0.2f;
        float timer = 0;

        // Nhấp nháy đỏ
        while (timer < explodeAfter)
        {
            if (sr) sr.color = sr.color == Color.white ? Color.red : Color.white;
            yield return new WaitForSeconds(blinkTime);
            timer += blinkTime;
        }

        // Phát hoạt ảnh nổ + tự hủy sau 1s
        if (explosionAnimPrefab)
        {
            Debug.Log("Nổ!");

            GameObject vfx = Instantiate(explosionAnimPrefab, transform.position, Quaternion.identity);
            if (audioEx != null && audioEx.clip != null)
            {
                GameObject audioTemp = new GameObject("ExplosionSound");
                audioTemp.transform.position = transform.position;

                AudioSource tempSource = audioTemp.AddComponent<AudioSource>();
                tempSource.clip = audioEx.clip;
                tempSource.volume = audioEx.volume;
                tempSource.pitch = audioEx.pitch;
                tempSource.outputAudioMixerGroup = audioEx.outputAudioMixerGroup;

                tempSource.Play();
                Destroy(audioTemp, tempSource.clip.length); // tự hủy sau khi phát xong
            }
            Destroy(vfx, 1f); // chỉnh thời gian theo độ dài animation
        }

        // Gây sát thương & phá hủy object trong vùng
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                var player = hit.GetComponent<PlayerController>();
                if (player != null)
                    player.ChangeHealth(-damage);
            }

            if (((1 << hit.gameObject.layer) & destructibleLayer) != 0)
            {
                hit.gameObject.SetActive(false);
            }
        }

        // Ẩn bản thân rồi khôi phục
        gameObject.SetActive(false);
        Invoke(nameof(Reactivate), respawnAfter);
        yield return new WaitForSeconds(respawnAfter);
        gameObject.SetActive(true);
        if (sr) sr.color = Color.white;
        explodeRoutine = null;
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
    void Reactivate()
    {
        gameObject.SetActive(true);
        if (sr) sr.color = Color.white;
        explodeRoutine = null;
        Debug.Log("Đá đã spawn lại bằng Invoke!");
    }
}
