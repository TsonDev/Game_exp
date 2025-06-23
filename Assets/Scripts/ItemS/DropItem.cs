using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    [SerializeField] private GameObject[] dropItems;
    [SerializeField] private float dropRate = 0.5f;

    public void DropItems()
    {
        if (dropItems.Length == 0) return;

        int dropCount = 0;
        foreach (GameObject item in dropItems)
        {
            if (item == null) continue;
            if (Random.value <= dropRate)
            {
                Debug.Log("Rơi item: " + item.name);
                Vector3 dropPos = transform.position + new Vector3(dropCount * 1f, 0, 0);

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
}
