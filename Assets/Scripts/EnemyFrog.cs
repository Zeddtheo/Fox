using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFrog : Enemy
{
    private Rigidbody2D rb;
    private Collider2D coll;
    public Transform leftPoint, rightPoint;
    private bool FaceLeft = true;
    public float speed,jumpForce;
    public float leftx, rightx;
    public LayerMask ground;

    //public Animator anim;
    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        //anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
        transform.DetachChildren();
        leftx = leftPoint.position.x;
        rightx = rightPoint.position.x;
        Destroy(leftPoint.gameObject);
        Destroy(rightPoint.gameObject);
    }

    void Update()
    {
        SwitchAnimation();
    }
    void Movement()
    {
        if (FaceLeft)
        {
            if (coll.IsTouchingLayers(ground))
            {
                anim.SetBool("jumping", true);
                rb.velocity = new Vector2(-speed, jumpForce);
            }          
            if (transform.position.x < leftx)
            {
                transform.localScale = new Vector3(-1, 1, 1);
                FaceLeft = false;
            }         
        }
        else
        {
            if (coll.IsTouchingLayers(ground))
            {
                anim.SetBool("jumping", true);
                rb.velocity = new Vector2(speed, jumpForce);
            }
            if (transform.position.x > rightx)
            {
                transform.localScale = new Vector3(1, 1, 1);
                FaceLeft = true;
            }
        }
    }

    void SwitchAnimation()
    {
        if (anim.GetBool("jumping")){
            if (rb.velocity.y < 0.1f)
            {
                anim.SetBool("jumping", false);
                anim.SetBool("falling", true);
            }
        }
        if (coll.IsTouchingLayers(ground) && anim.GetBool("falling"))
        {
            anim.SetBool("falling", false);
        }
    }
   
}
