using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;


    Rigidbody2D rigid2d;
    float horizontalMovement;
    Animator animator;
    Vector2 moveDirection =new Vector2(0,0);
    
    // Start is called before the first frame update
    void Start()
    {
       rigid2d = GetComponent<Rigidbody2D>(); 
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        rigid2d.velocity =new Vector2( horizontalMovement * moveSpeed,rigid2d.velocity.y); 
        if(!Mathf.Approximately(rigid2d.velocity.x,0.0f) ){
            moveDirection.Set(rigid2d.velocity.x,0);
            moveDirection.Normalize();
        }
        if (moveDirection.x > 0)
            transform.localScale = new Vector3(1, 1, 1); 
        else if (moveDirection.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);  
        animator.SetFloat("MoveX", moveDirection.x);
        Debug.Log(moveDirection.x);
        if (rigid2d.velocity.x == 0)
        {
            animator.SetInteger("Speed", 0);

        }
        else
        {
            animator.SetInteger("Speed", 1);
            
        }

    }
    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }
}
