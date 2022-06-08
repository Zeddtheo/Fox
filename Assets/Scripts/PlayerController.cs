using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    
    private Rigidbody2D rb;
    private Animator anim;
    public Collider2D coll;
    public Collider2D disColl;
    public float speed, jumpForce;
    private float horizontalMove;
    public Transform groundCheck;
    public Transform cellingCheck;
    public LayerMask ground;
    [Header("CD的UI组件")]
    public Image CDImage;
    [Header("Dash参数")]
    public float dashTime;
    private float dashTimeLeft;
    public float dashSpeed;
    private float lastDash = -10;
    public float dashCoolDown;

    bool jumpPressed,ctrlPressed;
    public bool isGround, isJump,isDashing;
    int jumpCount;

    public Text CherryNum;

    private bool isHurt;
    public int Cherry;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump") && jumpCount>0)
        {              
           
            jumpPressed = true;           
        }
        //蹲下检测      
        if (Input.GetButton("Crouch"))
        {
            anim.SetBool("crouching", true);
            disColl.enabled = false;
        }
        else 
        {
            if(!Physics2D.OverlapCircle(cellingCheck.position, 0.2f, ground))
            {
                anim.SetBool("crouching", false);
                disColl.enabled = true;
            }          
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (Time.time >= (lastDash + dashCoolDown))
            {
                //可以执行dash
                ReadyToDash();
            }
        }
        CDImage.fillAmount -= 1.0f * dashCoolDown * Time.deltaTime;
        CherryNum.text = Cherry.ToString();
    }
    void FixedUpdate()
    {
        
        isGround = Physics2D.OverlapCircle(groundCheck.position, 0.1f, ground);
        Dash();
        if (isDashing)
        {
            return;
        }
        if (!isHurt)
        {
            Movement();
        }
        SwitchAnimation();

        jump();
    }
    //移动
    void Movement()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(horizontalMove * speed, rb.velocity.y);

        if (horizontalMove != 0)
        {
            transform.localScale = new Vector3(horizontalMove, 1, 1);
        }
        //Crouch();
    }
    //跳跃
    void jump()       
    {
        
        if (isGround)
        {
            jumpCount = 2;//二段跳
            isJump = false;
        }
        if(jumpPressed && isGround)
        {
            SoundManager.Instance.JumpAudio();
            isJump = true;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpCount--;
            jumpPressed = false;
        }
        else if(jumpPressed && jumpCount >0 && isJump)
        {
            SoundManager.Instance.JumpAudio();
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpCount--;
            jumpPressed = false;
        }
    }
    //切换动画
    void SwitchAnimation()
    {
        anim.SetFloat("running", Mathf.Abs(rb.velocity.x));
        if (rb.velocity.y < 0.1f && !isGround)
        {
            anim.SetBool("falling", true);
        }
        if (isGround)
        {
            anim.SetBool("falling", false);
        }else if(!isGround && rb.velocity.y > 0)
        {
            anim.SetBool("jumping", true);
        }else if(rb.velocity.y < 0)
        {
            anim.SetBool("jumping", false);
            anim.SetBool("falling", true);
        }
        if (isHurt)
        {
            
            anim.SetBool("hurt", true);
            anim.SetFloat("running", 0);
            if (Mathf.Abs(rb.velocity.x) < 0.1f)
            {
                anim.SetBool("hurt", false);
                isHurt = false;
            }
        }

    }
    //碰撞触发器
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //收集物品
        if(collision.tag == "Collection")
        {
            SoundManager.Instance.CherryAudio();
            collision.GetComponent<Animator>().Play("getCollection");
            
        }
        if(collision.tag == "DeadLine")
        {
            GetComponent<AudioSource>().enabled = false;
            Invoke("Restart", 1f);
        }
    }
    //消灭怪物
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (anim.GetBool("falling"))
            {
                enemy.JumpOn();
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                anim.SetBool("jumping", true);
            }else if (transform.position.x<collision.gameObject.transform.position.x)
            {
                SoundManager.Instance.HurtAudio();
                rb.velocity = new Vector2(-3,rb.velocity.y);
                isHurt = true;
            }else if (transform.position.x > collision.gameObject.transform.position.x)
            {
                SoundManager.Instance.HurtAudio();
                rb.velocity = new Vector2(3,rb.velocity.y);
                isHurt = true;
            }
        }
        
       
    }
    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void CherryCount()
    {
        Cherry += 1;
    }

    void ReadyToDash()
    {
        isDashing = true;
        dashTimeLeft = dashTime;
        lastDash = Time.time;
        CDImage.fillAmount = 1;
    }
    void Dash()
    {
        if (isDashing)
        {
            if(dashTimeLeft > 0)
            {
                if (rb.velocity.y > 0 && !isGround)
                {
                    rb.velocity = new Vector2(dashSpeed* gameObject.transform.localScale.x, jumpForce);
                }
                rb.velocity = new Vector2(dashSpeed * gameObject.transform.localScale.x, rb.velocity.y);
                dashTimeLeft -= Time.deltaTime;
                ShadowPool.instance.GetFromPool();
            }
            if(dashTimeLeft <= 0)
            {
                isDashing=false;
                if (!isGround)
                {
                    rb.velocity = new Vector2(dashSpeed*horizontalMove,jumpForce);
                }
            }
        }
    }
}
