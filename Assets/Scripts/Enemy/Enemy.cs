using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Public variables
    public float speed;
    public bool vertical;
    public float changeTime = 3.0f;
   /* public AudioClip deathSound;*/

    // Private variables
    Rigidbody2D rigidbody2d;
    Animator animator;
    float timer;
    int direction = 1;
    bool broken = true;
    AudioSource source;


    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        timer = changeTime;
       source = GetComponent<AudioSource>();

    }


    // Update is called every frame
    void Update()
    {
        timer -= Time.deltaTime;


        if (timer < 0)
        {
            direction = -direction;
            timer = changeTime;
        }
    }


    // FixedUpdate has the same call rate as the physics system
    void FixedUpdate()
    {
        if (!broken)
        {
            return;
        }

        Vector2 position = rigidbody2d.position;

      /*  if (vertical)
        {
            position.y = position.y + speed * direction * Time.deltaTime;
            animator.SetFloat("Move X", 0);
        }
        else
        {*/
            position.x = position.x + speed * direction * Time.deltaTime;
            animator.SetFloat("MoveX", direction);
           
        


        rigidbody2d.MovePosition(position);
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.gameObject.GetComponent<PlayerController>();


        if (player != null)
        {
            player.ChangeHealth(-1);
        }
        Projectile projectile = other.gameObject.GetComponent<Projectile>();
        if (projectile != null)
        {

            animator.SetTrigger("Hit");
            
            if (source != null)
            {
                  source.Play();
                Debug.Log("y");
            }

            Destroy(gameObject, 1f);

        }
    }


    /*void OnCollisionEnter2D(Collision2D collision)
    {
        animator.SetTrigger("Hit");
        Destroy(gameObject,0.5f);
    }*/
 



    /*public void Fix()
    {
        broken = false;
        GetComponent<Rigidbody2D>().simulated = false;
        animator.SetTrigger("Fixed");
    }*/
}
