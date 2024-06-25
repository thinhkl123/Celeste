using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public SpriteRenderer sr;

    private Animator animator;
    private Movement movement;
    private Collision coll;

    private void Start()
    {
        animator = GetComponent<Animator>();
        movement = GetComponentInParent<Movement>();
        coll = GetComponentInParent<Collision>();
    }

    private void Update()
    {
        animator.SetBool("canMove", movement.canMove);
        animator.SetBool("onGround", coll.onGround);
        animator.SetBool("onWall", coll.onWall);
        animator.SetBool("onRightWall", coll.onRightWall);
        animator.SetBool("wallGrab", movement.wallGrab);
        animator.SetBool("wallSlide", movement.wallSlide);
        animator.SetBool("isDashing", movement.isDashing);
        animator.SetBool("isdead", movement.isDead);
    }

    public void SetHorizontalMovement(float x, float y, float yVel)
    {
        animator.SetFloat("HorizontalAxis", x);
        animator.SetFloat("VerticalAxis", y);
        animator.SetFloat("VerticalVelocity", yVel);
    }

    public void SetTrigger(string trigger)
    {
        animator.SetTrigger(trigger);
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
}
