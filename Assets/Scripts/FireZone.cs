using UnityEngine;

public class FireZone : MonoBehaviour
{
    public int damagePerSecond = 10;
    public float duration = 5f;
    public float damageInterval = 0.5f;
    public AudioClip fireLoopSound;

    private AudioSource audioSource;
    private float damageTimer;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource && fireLoopSound)
        {
            audioSource.clip = fireLoopSound;
            audioSource.loop = true;
            audioSource.Play();
        }

        Destroy(gameObject, duration);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            damageTimer += Time.deltaTime;
            if (damageTimer >= damageInterval)
            {
                damageTimer = 0f;
                other.GetComponent<PlayerController>()?.ChangeHealth(-damagePerSecond);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            damageTimer = 0f;
    }
}
