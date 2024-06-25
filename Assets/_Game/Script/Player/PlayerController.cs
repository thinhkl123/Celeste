using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static Vector3 savePoint;

    [SerializeField] private float speed = 7f;
    [SerializeField] private float jumpforce = 12f;
    [SerializeField] private float slideSpeed = 1f;
    [SerializeField] private float wallJumpLerp = 5f;
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashTime = .3f;
    [SerializeField] private float coyoteTime = 0.2f;

    private Rigidbody2D rb;
    private Collision coll;
    private BetterJump betterJump;
    private AnimationScript animator;

    private float coyoteTimeCounter;

    public bool wallSlide;
    public bool wallGrab;
    public bool grabEdge;
    public bool canMove;
    public bool wallJump;
    public bool isDashing;
    public bool canDash;
    public bool isDead;

    public Vector2 dashDir;
    public int side = 1;

    [Header("Ghost Effect")]
    [SerializeField] private GhostTrail ghostTrail;

    [Header("Particle System")]
    [SerializeField] private ParticleSystem dashPartical;
    [SerializeField] private ParticleSystem jumpPartical;
    [SerializeField] private ParticleSystem wallJumpPartical;
    [SerializeField] private ParticleSystem slideParticle;

    private Vector3 initalPos;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collision>();
        betterJump = GetComponent<BetterJump>();
        animator = GetComponentInChildren<AnimationScript>();

        initalPos = transform.position;
    }

    private void Start()
    {
        LevelManager.Ins.OnLoadNewLevel += LevelManager_OnLoadNewLevel;
        LevelManager.Ins.OnLoadContinueLevel += LevelManager_OnLoadContinueLevel;

        rb.gravityScale = 0f;
    }

    private void LevelManager_OnLoadContinueLevel(object sender, System.EventArgs e)
    {
        OnContinueInit();
    }

    private void LevelManager_OnLoadNewLevel(object sender, System.EventArgs e)
    {
        OnInit();
    }


    private void Update()
    {
        if (GameManager.Ins.state != GameManager.GameState.Playing)
        {
            return;
        }

        if (isDead)
        {
            return;
        }

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        float xRaw = Input.GetAxisRaw("Horizontal");
        float yRaw = Input.GetAxisRaw("Vertical");

        Vector2 dir = new Vector2(x, y);

        Walk(dir);
        animator.SetAxis(x, y, rb.velocity.y);

        if (coll.onWall && (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.V) || Input.GetKeyDown(KeyCode.Z)))
        //if (coll.onWall && GameInput.Ins.isGrabPressed && canMove)
        {
            if (side != coll.wallSide)
            {
                animator.Flip(side * -1);
            }

            wallGrab = true;
            wallSlide = false;
        }

        if (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.V) || Input.GetKeyUp(KeyCode.Z) || !coll.onWall || !canMove)
        //if (!GameInput.Ins.isGrabPressed || !coll.onWall || !canMove)
        {
            wallGrab = false;
            wallSlide = false;
        }

        if (coll.onGround && !isDashing)
        {
            wallJump = false;
            betterJump.enabled = true;
        }

        if (coll.onGround)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;   
        }

        if (wallGrab && coll.greenBox && !coll.redBox && !grabEdge)
        {
            //Debug.Log("Grab Edge");
            grabEdge = true;
        }

        if (grabEdge)
        {
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0;
            ChangePosWhenGrabEge();
            return;
        }

        /*
        if (isDashing)
        {
            //rb.velocity = dashDir.normalized * dashSpeed;
            return;
        }
        */

        if (wallGrab && !isDashing)
        {
            rb.gravityScale = 0;
            WallGrab(x,y);
        }
        else
        {
            rb.gravityScale = 3f;
        }

        if (coll.onWall && !coll.onGround)
        {
            if (x != 0 && !wallGrab)
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
        //if (GameInput.Ins.isJumpPressed)
        {
            animator.SetTrigger("Jump");

            if (coyoteTimeCounter > 0f)
            {
                Jump(Vector2.up, false);
            }
            else if (coll.onWall && !coll.onGround)
            {
                WallJump();
            }
        }

        if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.X)) && canDash)
        //if (GameInput.Ins.isDashPressed && canDash)
        {
            animator.SetTrigger("Dash");

            ghostTrail.ShowGhost();

            dashDir = new Vector2(xRaw, yRaw);
            if (dashDir == Vector2.zero)
            {
                dashDir = new Vector2(animator.GetFlipSprite(), 0);
            }

            rb.velocity = Vector2.zero;
            rb.velocity += dashDir.normalized * dashSpeed;

            StartCoroutine(DashWait());

            /*
            isDashing = true;
            canDash = false;
            dashDir = new Vector2(xRaw, yRaw);
            dashPartical.Play();
            */
            //StartCoroutine(StopDash());
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

    private void Walk(Vector2 dir)
    {
        if (wallGrab) 
        { 
            return;
        }

        if (!wallJump)
        {
            //Debug.Log("Walk");
            rb.velocity = new Vector2(dir.x * speed, rb.velocity.y);
        }
        else
        {
            //Debug.Log("WalkJump");
            rb.velocity = Vector2.Lerp(rb.velocity, (new Vector2(dir.x * speed, rb.velocity.y)), wallJumpLerp * Time.deltaTime);
        }
    }

    private void Jump(Vector2 dir, bool wall)
    {
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

        if (!canMove)
        {
            return;
        }

        bool pushingWall = false; 

        if ((coll.onRightWall && rb.velocity.x > 0) || (coll.onLeftWall && rb.velocity.x < 0))
        {
            pushingWall = true;
        }

        float push = pushingWall ? 0 : rb.velocity.x;

        rb.velocity = new Vector2(push, -slideSpeed);
    }

    private void WallGrab(float x, float y)
    {
        if (x > .2f || x < -.2f)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }

        float speedModifier = y > 0 ? .5f : 1;

        rb.velocity = new Vector2(rb.velocity.x, y * (speed * speedModifier));
    }

    private void ChangePosWhenGrabEge()
    {
        transform.position = new Vector2(transform.position.x + 0.5f, transform.position.y + 0.4f);
        rb.gravityScale = 3;
        grabEdge = false;
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

    IEnumerator DisableMovement(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

    IEnumerator StopDash()
    {
        yield return new WaitForSeconds(dashTime);
        dashPartical.Stop();
        isDashing = false;
        canDash = true;
    }

    IEnumerator DashWait()
    {
        dashPartical.Play();
        rb.gravityScale = 0;
        betterJump.enabled = false;
        wallJump = true;
        isDashing = true;
        canDash = false;

        yield return new WaitForSeconds(dashTime);

        dashPartical.Stop();
        rb.gravityScale = 3f;
        betterJump.enabled = true;
        wallJump = false;
        isDashing = false;
        canDash = true;
    }

    private void WallParticle(float vertical)
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log(collision.gameObject.tag);
        if (collision.gameObject.CompareTag("Thorn"))
        {
            //Debug.Log("Death");
            Death();
            animator.SetTrigger("Death");
            isDead = true;
            if (GameManager.Ins.IsAlive())
            {
                Invoke(nameof(DeSpawn), 1f);
            }
        }
    }

    private void Death()
    {
        rb.gravityScale = 0f;
        betterJump.enabled = false;

        animator.SetPlayerDead();

        wallSlide = false;
        wallGrab = false;
        grabEdge = false;
        canMove = false;
        wallJump = false;
        isDashing = false;
        canDash = false;

        rb.velocity = Vector2.zero;

        //GameManager.Ins.DecreaseHeart();
    }

    private void DeSpawn()
    {
        rb.gravityScale = 3f;
        betterJump.enabled = true;

        animator.SetPlayerLive();

        isDead = false;
        canDash = true;
        canMove = true;
        transform.position = savePoint;

    }

    private void OnInit()
    {
        rb.gravityScale = 3f;
        betterJump.enabled = true;

        animator.SetPlayerLive();

        isDead = false;
        canDash = true;
        canMove = true;
        transform.position = initalPos;
    }

    private void OnContinueInit()
    {
        rb.gravityScale = 3f;
        betterJump.enabled = true;

        animator.SetPlayerLive();

        isDead = false;
        canDash = true;
        canMove = true;

        float x = PlayerPrefs.GetFloat(Constant.SAVEPOINT_X);
        float y = PlayerPrefs.GetFloat(Constant.SAVEPOINT_Y);
        float z = PlayerPrefs.GetFloat(Constant.SAVEPOINT_Z);

        Vector3 pos = new Vector3(x, y, z);

        transform.position = pos;
    }
}
