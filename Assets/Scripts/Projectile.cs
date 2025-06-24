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
    public int dame;
 
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
        // Nếu đối tượng nằm trong Layer "Enemy"

        // Gọi hàm ChangeHealth nếu có
        Debug.Log("Da va cham");
            EnemyAI enemyAI = collision.GetComponent<EnemyAI>();
            EnemyShooter enemyShoot = collision.GetComponent<EnemyShooter>();
            BossLv4 bossLv4 = collision.GetComponent<BossLv4>();  
        BossUltimate bossUltimate = collision.GetComponent<BossUltimate>();
            if (enemyAI != null)
            {
            Debug.Log("Gay dame ai");
                enemyAI.ChangeHealth(-dame);
            }
            else if (enemyShoot != null)
            {
                enemyShoot.ChangeHealth(-dame);
            }
            else if(bossLv4 != null)
            {
                bossLv4.ChangeHealth(-dame);
            }
            else if (bossUltimate != null)
        {
            bossUltimate.ChangeHealth(-dame);
        }

            // Hoặc bạn có thể làm việc với các loại enemy khác tại đây
        

        animator.SetTrigger("Explotion");
        Destroy(gameObject, 0.5f);
    }
    

}
