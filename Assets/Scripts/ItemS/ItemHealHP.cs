using UnityEngine;

public class ItemHealHP : MonoBehaviour
{
    [SerializeField] private float healPercent = 0.2f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.GetComponent<PlayerController>();
        if (player != null)
        {
            float amount = player.maxHealth * healPercent;
            player.ChangeHealth((int)amount);
            Destroy(gameObject);
        }
    }
}
