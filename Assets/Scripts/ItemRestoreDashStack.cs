using UnityEngine;

public class ItemRestoreDashStack : MonoBehaviour
{
    [SerializeField] private int stackToAdd = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.GetComponent<PlayerController>();
        if (player != null)
        {
            player.AddDashStack(stackToAdd);
            Destroy(gameObject);
        }
    }
}
