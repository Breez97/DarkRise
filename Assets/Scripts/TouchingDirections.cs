using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchingDirections : MonoBehaviour
{
    [SerializeField] private ContactFilter2D contactFilter;
    private CapsuleCollider2D touchingCollider;
    private RaycastHit2D[] groundHits = new RaycastHit2D[5];
    private RaycastHit2D[] wallHits = new RaycastHit2D[5];
    private RaycastHit2D[] ceilingHits = new RaycastHit2D[5];
    private Animator animator;
    [SerializeField] private float groundDistance = 0.05f;
    [SerializeField] private float wallDistance = 0.2f;
    [SerializeField] private float ceilingDistance = 0.05f;

    [SerializeField] private bool isGrounded;

    public bool IsGrounded
    {
        get { return isGrounded; }
        set {

            isGrounded = value;
            animator.SetBool(AnimationStrings.isGrounded, isGrounded);
        }
    }
    [SerializeField] private bool isOnWall;

    public bool IsOnWall
    {
        get { return isOnWall; }
        set
        {

            isOnWall = value;
            animator.SetBool(AnimationStrings.isOnWall, isOnWall);
        }
    }

    [SerializeField] private  Vector2 wallCheckDirection => gameObject.transform.localScale.x > 0 ? Vector2.right : Vector2.left;
    [SerializeField] private bool isOnCeiling;

    public bool IsOnCeiling
    {
        get { return isOnCeiling; }
        set
        {

            isOnCeiling = value;
            animator.SetBool(AnimationStrings.isOnCeiling, isOnCeiling);
        }
    }


    private void Awake()
    {
        touchingCollider = GetComponent<CapsuleCollider2D>();   
        animator = GetComponent<Animator>();    
    }


    void Update()
    {
        IsGrounded = touchingCollider.Cast(Vector2.down, contactFilter, groundHits, groundDistance) > 0;
        IsOnWall = touchingCollider.Cast(wallCheckDirection, contactFilter, wallHits, wallDistance) > 0;
        IsOnCeiling = touchingCollider.Cast(Vector2.up, contactFilter, ceilingHits, ceilingDistance) > 0;

    }
}
