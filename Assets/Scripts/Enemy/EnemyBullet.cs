using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBu : MonoBehaviour
{
     public float lifeTime = 3f;
    public int damage = 1;
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        Destroy(gameObject, lifeTime);
       /* Đạn theo người chơi*/

       /* Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 velocity = rb.velocity;
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }*/
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Enemy bullet hit: " + collision.name);

        PlayerController player = collision.GetComponent<PlayerController>();
        if (player != null)
        {
            player.ChangeHealth(-damage);
        }
        animator.SetTrigger("Hit");
        Destroy(gameObject,1);
    }

}
