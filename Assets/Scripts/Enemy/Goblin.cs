using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Goblin : MonoBehaviour
{
    [Space]
    [Header("Detection Zones")]
    [SerializeField] private DetectionZone attackZone;
    [SerializeField] private DetectionZone bombZone;
    [SerializeField] private DetectionZone groundZone;
    [SerializeField] private DetectionZone endGroundZone;
    [SerializeField] private DetectionZone wallZone;
    [SerializeField] GameObject bombZoneObject;
    private Collider2D bombZoneCollider;
    private int throwCount = 1;
    private float durationBetweenAttack = 1f;
    private bool lostTarget = false;
    private bool lostTargetBombZone = false;

    [Space]
    [Header("Goblin preferences")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float walkStopRate = 0.6f;
    [SerializeField] private float delayBeforeAttack = 0.5f;
    [SerializeField] private float delayAfterAttack = 0.5f;
    [SerializeField] private float timeToReload = 3f;

    [Space]
    [Header("Bomb")]
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private Transform throwPos;

    private bool isOnGround = false;
    private bool isOnGroundEnd = false;
    private bool isOnWall = false;
    private bool isFalling = true;

    private Rigidbody2D rb;
    private Animator animator;
    private Animator playerAnimator;
    private GameObject player;

    public enum WalkableDirection { Right, Left }

    private WalkableDirection _walkDirections;
    private Vector2 walkDirectionVector = Vector2.right;

    public WalkableDirection WalkDirection
    {
        get { return _walkDirections; }
        set 
        {
            if (_walkDirections != value) 
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
            }
            _walkDirections = value;
        }
    }

    public float WalkSpeed 
    {
        get => walkSpeed;
    }

    public bool EnteringGroundCollider
    {
        get => groundZone.DetectedColliders.Count > 0;
    }

    public bool EnteringWallCollider 
    {
        get => wallZone.DetectedColliders.Count > 0;
    }

    public bool EnteringEndGroundCollider 
    {
        get => endGroundZone.DetectedColliders.Count > 0;
    }

    public bool EnteringAttackZone 
    {
        get => attackZone.DetectedColliders.Count > 0;
    }

    public bool EnteringBombZone 
    {
        get => bombZone.DetectedColliders.Count > 0;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerAnimator = player.GetComponent<Animator>();
        bombZoneCollider = bombZoneObject.GetComponent<Collider2D>();
    }

    private void FixedUpdate()
    {
        GoblinMovement();
    }

    private void GoblinMovement() 
    {
        if (animator.GetBool(AnimationStrings.isAlive))
        {
            isFalling = false;
            if (rb.velocity.y < -0.1f)
            {
                isFalling = true;
            }

            CheckStates();

            if (isFalling)
            {
                animator.SetBool(AnimationStrings.isIdle, true);
            }
            else
            {
                if (isOnGround)
                {
                    if (animator.GetBool(AnimationStrings.isThrowing) && !EnteringBombZone && !lostTargetBombZone)
                    {
                        lostTargetBombZone = true;
                        bombZoneCollider.enabled = false;
                        StartCoroutine(WaitThrowBomb());
                    }

                    if (EnteringBombZone && playerAnimator.GetBool(AnimationStrings.isAlive))
                    {
                        if (!lostTargetBombZone) 
                        {
                            if (!animator.GetBool(AnimationStrings.hitTrigger))
                            {
                                rb.velocity = new Vector2(0, rb.velocity.y);
                            }

                            animator.SetBool(AnimationStrings.canMove, false);
                            animator.SetBool(AnimationStrings.isThrowing, true);
                            throwCount = 1;

                            GoblinThrowBomb();
                        }
                    }
                    else 
                    {
                        GoblinThrowBomb();

                        if (!animator.GetBool(AnimationStrings.canMove) && animator.GetBool(AnimationStrings.isThrowing)) 
                        {
                            animator.SetBool(AnimationStrings.canMove, true);
                            animator.SetBool(AnimationStrings.isThrowing, false);
                        }

                        if (EnteringAttackZone && playerAnimator.GetBool(AnimationStrings.isAlive))
                        {
                            LookOnPlayer();

                            if (!animator.GetBool(AnimationStrings.hitTrigger))
                            {
                                rb.velocity = new Vector2(0, rb.velocity.y);
                            }

                            animator.SetBool(AnimationStrings.isIdle, true);
                            animator.SetBool(AnimationStrings.canMove, false);
                            animator.SetBool(AnimationStrings.hasTarget, true);
                        }
                        else
                        {
                            if (animator.GetBool(AnimationStrings.hasTarget))
                            {
                                if (!animator.GetBool(AnimationStrings.hitTrigger))
                                {
                                    rb.velocity = new Vector2(0, rb.velocity.y);
                                }
                                GoblinRun();
                            }
                            else
                            {
                                AnimatorClipInfo[] currentClipInfo = animator.GetCurrentAnimatorClipInfo(0);
                                if (currentClipInfo.Length > 0 && currentClipInfo[0].clip.name == "goblin_attack")
                                {
                                    animator.Rebind();
                                }

                                GoblinRun();
                            }
                        }
                    }
                }
            }

        }
        else 
        {
            if (!animator.GetBool(AnimationStrings.hitTrigger)) 
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }
    }

    private IEnumerator WaitThrowBomb()
    {
        yield return new WaitForSeconds(0.5f);
        lostTargetBombZone = false;
        bombZoneCollider.enabled = true;
    }

    private IEnumerator TurnOfBombCollider()
    {
        yield return new WaitForSeconds(timeToReload);
        bombZoneCollider.enabled = true;
    }

    private void GoblinThrowBomb()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("goblin_throw") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            bombZone.DetectedColliders.Clear();
            StartCoroutine(TurnOfBombCollider());
            if (throwCount == 1)
            {
                ThrowingBomb();
                throwCount--;
            }
        }
    }

    private void ThrowingBomb() 
    {
        Instantiate(bombPrefab, throwPos.position, Quaternion.identity);
    }

    private void GoblinRun()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("goblin_attack") && !lostTarget)
        {
            lostTarget = true;
            StartCoroutine(WaitAttack());
        }

        if (animator.GetBool(AnimationStrings.canMove))
        {
            if (isOnWall || isOnGroundEnd)
            {
                FlipDirection();
            }
            rb.velocity = new Vector2(walkSpeed * walkDirectionVector.x, rb.velocity.y);
        }
    }

    private IEnumerator WaitAttack()
    {
        animator.SetBool(AnimationStrings.hasTarget, false);
        animator.SetBool(AnimationStrings.isIdle, false);
        animator.SetBool(AnimationStrings.canMove, true);
        yield return new WaitForSeconds(durationBetweenAttack);
        lostTarget = false;
    }

    private void CheckStates() 
    {
        isOnGround = EnteringGroundCollider ? true : false;

        isOnGroundEnd = EnteringEndGroundCollider ? false : true;

        isOnWall = EnteringWallCollider ? true : false;
    }

    private void FlipDirection() 
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

    private void LookOnPlayer() 
    {
        float distance = player.transform.position.x - transform.position.x;
        if (distance < 0f && WalkDirection == WalkableDirection.Right)
        {
            FlipDirection();
        }
        if (distance > 0f && WalkDirection == WalkableDirection.Left)
        {
            FlipDirection();
        }
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
    }
}
