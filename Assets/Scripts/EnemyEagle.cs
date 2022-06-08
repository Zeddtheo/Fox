using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEagle : Enemy
{
    private Rigidbody2D rb;
    //private Collider2D coll;
    public Transform topPoint, bottomPoint;
    private float topY, bottomY;
    public float jumpForce,speed;
    private bool faceTop;

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        //coll = GetComponent<Collider2D>();
        transform.DetachChildren();
        topY = topPoint.position.y;
        bottomY = bottomPoint.position.y;
        Destroy(topPoint.gameObject);
        Destroy(bottomPoint.gameObject);
    }

   
    void Update()
    {
        Movement();
    }

    void Movement()
    {
        if (faceTop)
        {
            rb.velocity = new Vector2(rb.velocity.x, speed);
            if (transform.position.y > topY)
            {
                faceTop = false;
            }
        }
        else
        {
            rb.velocity = new Vector2(rb.velocity.x, -speed);
            if(transform.position.y < bottomY)
            {
                faceTop = true;
            }
        }
        
     

    }
}
