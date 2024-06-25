using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationScript : MonoBehaviour
{
    private Animator anim;
    private SpriteRenderer sr;
    private PlayerController movement;
    private Collision coll;

    private bool isPlayerDead;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        movement = GetComponentInParent<PlayerController>();
        coll = GetComponentInParent<Collision>();
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        if (isPlayerDead)
        {
            return;
        }

        anim.SetBool("canMove", movement.canMove);
        anim.SetBool("onGround", coll.onGround);
        anim.SetBool("isDashing", movement.isDashing);
        anim.SetBool("wallGrab", movement.wallGrab);
        anim.SetBool("wallSlide", movement.wallSlide);
        anim.SetBool("isDead", movement.isDead);
    }

    public void SetPlayerDead()
    {
        isPlayerDead = true;
        anim.SetBool("onGround", true);
        anim.SetBool("canMove", false);
        anim.SetBool("isDashing", false);
        anim.SetBool("wallGrab", false);
        anim.SetBool("wallSlide", false);
        anim.SetBool("isDead", true);
    }

    public void SetPlayerLive()
    {
        isPlayerDead = false;
        Flip(1);
    }

    public void Flip(int side)
    {
        if (movement.wallGrab || movement.wallSlide)
        {
            if (side == -1 && sr.flipX)
                return;

            if (side == 1 && !sr.flipX)
            {
                return;
            }
        }

        bool state = (side == 1) ? false : true;
        sr.flipX = state;
    }

    public int GetFlipSprite()
    {
        if (sr.flipX) return -1;
        else return 1;
    }

    public void SetAxis(float x, float y, float veloY)
    {
        anim.SetFloat("x", x);
        anim.SetFloat("y", y);
        anim.SetFloat("Speed", Mathf.Abs(x));
        anim.SetFloat("VeloY", veloY);
    }

    public void SetTrigger(string s)
    {
        anim.SetTrigger(s);
    }
}
