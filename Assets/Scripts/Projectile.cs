using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Projectile : MonoBehaviour
{
    // Start is called before the first frame update
    Rigidbody2D rigid2d;
    float time = 3f;
    float timeDestroy;
    Animator animator;
 
    void Awake()
    {
       rigid2d = GetComponent<Rigidbody2D>();
        timeDestroy = time;
        animator = GetComponent<Animator>();    
    }

    // Update is called once per frame
    void Update()
    {
         timeDestroy-=Time.deltaTime;
        if(timeDestroy < 0)
        {
            Destroy(gameObject);
            timeDestroy = time;
        }
       

    }
    public void Launch(Vector2 direction, float force)
    {
        rigid2d.AddForce(direction*force);
        animator.SetFloat("LookX", direction.x);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
       
        animator.SetTrigger("Explotion");
        Destroy(gameObject,0.5f);
    }

}
