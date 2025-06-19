using UnityEngine;

public class ItemRestoreMana : MonoBehaviour
{
    [SerializeField] private float manaPercent = 0.2f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.GetComponent<PlayerController>();
        if (player != null)
        {
            float manaToAdd = player.maxMana * manaPercent;
            player.RestoreMana(manaToAdd);
            Destroy(gameObject);
        }
    }
}
