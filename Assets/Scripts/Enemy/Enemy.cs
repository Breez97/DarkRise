using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    [Header("Скорость движения и остановки")]
    [SerializeField] protected float walkSpeed = 3f;
    [SerializeField] protected float walkStopRate = 0.6f;
    [Space]
    [SerializeField] protected string playerTag = "Player";
    [Header("Дистанция обнаружения")]
    [SerializeField] protected float detectionRange = 5f;
    [Space]
    [Header("Видимость игрока")]
    [SerializeField] protected bool playerVisibility = false;
    [Header("Видимость врага")]
    [SerializeField] protected bool enemyVisibility = false;
    [Header("Точки перехода")]
    [SerializeField] protected Transform waypointA;
    [SerializeField] protected Transform waypointB;
    protected float minX;
    protected float maxX;
    protected float midPointX;
    protected float switchTimer = 0f;

    [SerializeField] protected PlayerController playerController;
    [SerializeField] protected enum WalkableDirection { Right, Left }
    private WalkableDirection _walkableDirection;
    protected Vector2 walkDirectionVector = Vector2.right;

    protected GameObject player;
    protected Animator animator;
    protected Rigidbody2D rigidBody;

    public bool IsAlive { get => animator.GetBool(AnimationStrings.isAlive); }

    public void SetPlayerController(PlayerController playerController)
    {
        this.playerController = playerController;
    }

    public void SetPoints(Transform waypointA, Transform waypointB)
    {
        this.waypointA = waypointA;
        this.waypointB = waypointB;
    }

    private bool _characterSpotted;
    protected virtual bool CharacterSpotted
    {
        get => _characterSpotted;
        set
        {
            _characterSpotted = value;
            animator.SetBool(AnimationStrings.isSpotted, value);
        }
    }

    private bool _isIdle = false;
    protected virtual bool IsIdle
    {
        get => _isIdle;

        set
        {
            _isIdle = value;
            animator.SetBool(AnimationStrings.isIdle, value);
        }
    }

    private bool _hasTarget = false;
    protected virtual bool Attack
    {
        get => _hasTarget;

        set
        {
            _hasTarget = value;
            animator.SetBool(AnimationStrings.hasTarget, value);
        }
    }

    protected virtual bool CanMove
    {
        get => animator.GetBool(AnimationStrings.canMove);

        set
        {
            animator.SetBool(AnimationStrings.canMove, value);
        }
    }

    protected virtual bool HitTrigger
    {
        get => animator.GetBool(AnimationStrings.hitTrigger);
    }

    protected virtual bool IsGrounded
    {
        get => animator.GetBool(AnimationStrings.isGrounded);
    }

    protected virtual WalkableDirection WalkDirection
    {
        get { return _walkableDirection; }
        set
        {
            if (_walkableDirection != value)
            {
                gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x * -1, gameObject.transform.localScale.y);

                if (value == WalkableDirection.Right)
                {
                    walkDirectionVector = Vector2.right;
                }
                else if (value == WalkableDirection.Left)
                {
                    walkDirectionVector = Vector2.left;
                }
                _walkableDirection = value;
            }
        }
    }

    public Transform WaypointB { get => waypointB; set => waypointB = value; }
    public Transform WaypointA { get => waypointA; set => waypointA = value; }

    protected virtual void Start()
    {
        minX = Mathf.Min(waypointA.position.x, waypointB.position.x);
        maxX = Mathf.Max(waypointA.position.x, waypointB.position.x);
        midPointX = (minX + maxX) / 2f;
        player = GameObject.FindGameObjectWithTag("Player");
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    protected void FlipDirection()
    {
        if (WalkDirection == WalkableDirection.Right)
        {
            WalkDirection = WalkableDirection.Left;
        }
        else if (WalkDirection == WalkableDirection.Left)
        {
            WalkDirection = WalkableDirection.Right;
        }
    }

    public virtual void OnHit(int damage, Vector2 knockback)
    {
        rigidBody.velocity = new Vector2(knockback.x, rigidBody.velocity.y + knockback.y);
    }

    protected virtual void CheckWaypointBoundaries()
    {
        float enemyX = transform.position.x;
        float playerX = player.transform.position.x;

        if ((enemyX < minX || enemyX > maxX))
        {
            enemyVisibility = false;
        }
        else
        {
            enemyVisibility = true;
        }
        if ((playerX < minX || playerX > maxX))
        {
            playerVisibility = false;
        }
        else
        {
            playerVisibility = true;
        }
    }

    protected virtual void ChangeDirectionToWaypoints()
    {
        if (!enemyVisibility)
        {
            float directionToMidPoint = Mathf.Sign(midPointX - transform.position.x);
            WalkDirection = (directionToMidPoint > 0) ? WalkableDirection.Right : WalkableDirection.Left;
        }
    }
}