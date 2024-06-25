using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float jumpforce = 50f;
    [SerializeField] private float slideSpeed = 1f;
    [SerializeField] private float dashSpeed = 40f;

    private Rigidbody2D rb;
    private Collision coll;
    private BetterJump betterJump;
    [SerializeField] private PlayerAnimator animator;

    public bool canMove;
    public bool wallGrab;
    public bool wallJump;
    public bool wallSlide;
    public bool isDashing;
    public bool isGrabbing;
    public bool hasDashed;
    public bool groundTouch;
    public bool isDead;

    public int side;

    //SpriteRender
    [SerializeField] private SpriteRenderer spriteRenderer;

    //Effect
    [SerializeField] private GhostTrail ghostTrail;

    //Particle System
    [SerializeField] private ParticleSystem dashPartical;
    [SerializeField] private ParticleSystem jumpPartical;
    [SerializeField] private ParticleSystem wallJumpPartical;
    [SerializeField] private ParticleSystem slideParticle;

    private void Awake()
    {
        canMove = true;

        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collision>();
        betterJump = GetComponent<BetterJump>();
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        float xRaw = Input.GetAxisRaw("Horizontal");
        float yRaw = Input.GetAxisRaw("Vertical");
        Vector2 dir = new Vector2(x, y);

        Walk(dir);
        animator.SetHorizontalMovement(x, y, rb.velocity.y);

        //wallGrab = coll.onWall && Input.GetKey(KeyCode.LeftShift);

        if (coll.onWall && Input.GetKey(KeyCode.LeftControl) && canMove)
        {
            if (side != coll.wallSide)
            {
                animator.Flip(side * -1);
            }

            wallGrab = true;
            wallSlide = false;
        }

        if (Input.GetKeyUp(KeyCode.LeftControl) || !coll.onWall || !canMove)
        {
            wallGrab = false;
            wallSlide = false;
        }

        if (coll.onGround && !isDashing)
        {
            wallJump = false;
            betterJump.enabled = true;
        }

        if (wallGrab && coll.greenBox && !coll.redBox && !isGrabbing)
        {
            //Debug.Log("isGrabbing");
            isGrabbing = true;
        }

        if (isGrabbing)
        {
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0;
            ChangePosWhenGrabEge();
            return;
        }

        if (wallGrab && !isDashing)
        {
            rb.gravityScale = 0;
            if (x > .2f || x < -.2f)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
            }

            float speedModifier = y > 0 ? 0.5f : 1f;
            rb.velocity = new Vector2(rb.velocity.x, y * speed * speedModifier);

        }
        else
        {
            rb.gravityScale = 3;
        }

        if (coll.onWall && !coll.onGround)
        {
            if (!wallGrab && x!=0)
            {
                wallSlide = true;
                WallSlide();
            }
        }

        if (coll.onGround || !coll.onWall)
        {
            wallSlide = false;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("jump");
            if (coll.onGround)
            {
                Jump(Vector2.up, false);
            }
            else if (coll.onWall && !coll.onGround)
            {
                //Debug.Log("Wall Jump");
                WallJump();
            }
        }

        //Debug.Log(hasDashed);

        //if (Input.GetKeyDown(KeyCode.C) && !isDashing)
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing)
        {
            if (xRaw != 0 || yRaw != 0)
            {
                Debug.Log("Dash");
                Dash(xRaw, yRaw);
            }
        }

        if (coll.onGround && !groundTouch)
        {
            GroundTouch();
            groundTouch = true;
        }

        if (!coll.onGround && groundTouch)
        {
            groundTouch = false;
        }

        WallParticle(y);

        if (wallGrab || wallSlide || !canMove)
            return;

        if (x > 0)
        {
            side = 1;
            animator.Flip(side);
        }
        if (x < 0)
        {
            side = -1;
            animator.Flip(side);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Thorn"))
        {
            Debug.Log("Dead");
            isDead = true;
            animator.SetTrigger("death");
        }
    }

    private void Walk(Vector2 dir)
    {
        if (wallSlide)
        {
            return;
        }

        if (wallGrab)
        {
            return;
        }

        if (!wallJump)
        {
            rb.velocity = new Vector2(dir.x * speed, rb.velocity.y);
        }
        else
        {
            rb.velocity = Vector2.Lerp(rb.velocity, (new Vector2(dir.x * speed, rb.velocity.y)), 5f * Time.deltaTime);
        }
    }

    private void Jump(Vector2 dir, bool wall)
    {
        slideParticle.transform.parent.localScale = new Vector3(ParticleSide(), 1, 1);
        ParticleSystem particle = wall ? wallJumpPartical : jumpPartical;

        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += dir * jumpforce;

        particle.Play();
    }

    private void WallSlide()
    {
        if (coll.wallSide != side)
        {
            animator.Flip(side * -1);
        }

        bool pushingWall = false;
        if ((rb.velocity.x > 0 && coll.onRightWall) || (rb.velocity.x < 0 && coll.onLeftWall))
        {
            pushingWall = true;
        }
        float push = pushingWall ? 0 : rb.velocity.x;

        rb.velocity = new Vector2(rb.velocity.x, -slideSpeed);
    }

    private void WallJump()
    {
        if ((side == 1 && coll.onRightWall) || side == -1 && !coll.onRightWall)
        {
            side *= -1;
            animator.Flip(side);
        }

        StopCoroutine(DisableMovement(0));
        StartCoroutine(DisableMovement(.1f));

        Vector2 walkDir = coll.onRightWall ? Vector2.left : Vector2.right;

        Jump(Vector2.up / 1.5f + walkDir / 1.5f, true);

        wallJump = true;
    }

    private void ChangePosWhenGrabEge()
    {
        transform.position = new Vector2(transform.position.x + 0.5f, transform.position.y + 0.4f);
        rb.gravityScale = 3;
        isGrabbing = false;
    }

    IEnumerator DisableMovement(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);  
        canMove = true;
    }

    private void Dash(float x, float y)
    {
        //Show ghost effect
        ghostTrail.ShowGhost();

        hasDashed = true;
        animator.SetTrigger("dash");

        rb.velocity = Vector2.zero;
        Vector2 dir = new Vector2(x, y);

        rb.velocity += dir.normalized * dashSpeed;
        StartCoroutine(DashWait());
    }

    IEnumerator DashWait()
    {
        StartCoroutine(GroundDash());

        dashPartical.Play();
        rb.gravityScale = 0;
        betterJump.enabled = false;
        wallJump = true;
        isDashing = true;

        yield return new WaitForSeconds(.3f);

        dashPartical.Stop();
        rb.gravityScale = 3f;
        betterJump.enabled = true;
        wallJump = false;
        isDashing = false;
    }

    IEnumerator GroundDash()
    {
        yield return new WaitForSeconds(.15f);
        if (coll.onGround)
        {
            hasDashed = false;
        }
    }

    private void GroundTouch()
    {
        hasDashed = false;
        isDashing = false;

        side = animator.sr.flipX ? -1 : 1;

        jumpPartical.Play();
    }

    void WallParticle(float vertical)
    {
        var main = slideParticle.main;

        if (wallSlide || (wallGrab && vertical < 0))
        {
            slideParticle.transform.parent.localScale = new Vector3(ParticleSide(), 1, 1);
            main.startColor = Color.white;
        }
        else
        {
            main.startColor = Color.clear;
        }
    }

    int ParticleSide()
    {
        int particleSide = coll.onRightWall ? 1 : -1;
        return particleSide;
    }
}
