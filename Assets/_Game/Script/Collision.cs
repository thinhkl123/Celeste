using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer;

    public bool onGround;
    public bool onWall;
    public bool onLeftWall;
    public bool onRightWall;
    public int wallSide;

    [SerializeField] private float collisionRadius = 0.25f;
    public Vector2 bottomOffest, rightOffset, leftOffset;
    [SerializeField] private Color debugCollsionColor = Color.red;

    //Grab Edge
    public float redXOffset, redYOffset, redXSize, redYSize;
    public float greenXOffset, greenYOffset, greenXSize, greenYSize;
    public bool greenBox, redBox;

    private void Start()
    {
        
    }
    private void Update()
    {
        onGround = Physics2D.OverlapCircle((Vector2) transform.position + bottomOffest, collisionRadius, groundLayer);
        onWall = Physics2D.OverlapCircle((Vector2) transform.position + rightOffset, collisionRadius, groundLayer)
            || Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, groundLayer);

        onRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, groundLayer);
        onLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, groundLayer);

        greenBox = Physics2D.OverlapBox(new Vector2(transform.position.x + greenXOffset, transform.position.y + greenYOffset), new Vector2(greenXSize, greenYSize), 0f, groundLayer);
        redBox = Physics2D.OverlapBox(new Vector2(transform.position.x + redXOffset, transform.position.y + redYOffset), new Vector2(redXSize, redYSize), 0f, groundLayer);

        wallSide = onRightWall ? -1 : 1;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        var positions = new Vector2[] { bottomOffest, rightOffset, leftOffset };

        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffest, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, collisionRadius);

        Gizmos.DrawWireCube(new Vector2(transform.position.x + redXOffset, transform.position.y + redYOffset), new Vector2(redXSize, redYSize));
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector2(transform.position.x + greenXOffset, transform.position.y + greenYOffset), new Vector2(greenXSize, greenXSize));
    }
}
