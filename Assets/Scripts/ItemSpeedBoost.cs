using UnityEngine;

public class ItemSpeedBoost : MonoBehaviour
{
    [SerializeField] private float speedMultiplier = 2f;
    [SerializeField] private float duration = 2f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.GetComponent<PlayerController>();
        if (player != null)
        {
            player.StartCoroutine(player.SpeedBoost(speedMultiplier, duration));
            Destroy(gameObject);
        }
    }
}
