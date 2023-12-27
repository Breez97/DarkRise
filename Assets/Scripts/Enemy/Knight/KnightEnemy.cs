using Assets.Scripts.Enemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D), typeof(DamagableKnight), typeof(TouchingDirections))]
public class KnightEnemy : Enemy
{
    [SerializeField] private DetectionZone attackZone;
    [SerializeField] private DetectionZone jumpZone;
    [SerializeField] private DetectionZone wallZone;
    [Header("Сила прыжка")]
    [SerializeField] private float jumpImpulse = 10f;
    [Header("Настройки атаки рыцаря")]
    [SerializeField] private float delayBeforeImpact = 2f;
    [SerializeField] private float delayNumberOfStrokes = 0.5f;
    [Space]
    [Header("Скорость изменения поворота врага при смене направления (во время движения за вами)")]
    [SerializeField] private float delayBeforeDirectionChange = 1.0f;
    [Header("Возможность увернуться от удара игрока")]
    [SerializeField] protected bool canDodge;
    [SerializeField] protected float dodgeChance = 0f;
    [SerializeField] protected float dodgeTime = 0.5f;
    [SerializeField] protected float dodgeDistance = 2f;
    [Space]
    [Header("Время жизни после начала падения")]
    [SerializeField] private float maxFallTime = 3f;
    [Header("Время перехода с состояния покоя в режим движения (Для поведения)")]
    [SerializeField] protected float timeToIdle = 2f;
    [SerializeField] protected float timeToRun = 5f;
    private Collider2D attackCollider;
    private bool isIdleState = true;
    private float fallTimer = 0f;
    private DamagableKnight damageable;
    private bool isChangingDirection = false;
    private bool isAttackCoroutineRunning = false;
    private TouchingDirections touchingDirections;

    public bool EnteringWallCollider
    {
        get => wallZone.DetectedColliders.Count > 0;
    }

    public bool EnteringAttackingCollider
    {
        get => attackZone.DetectedColliders.Count > 0;
    }

    public bool EnteringJumpCollider
    {
        get => jumpZone.DetectedColliders.Count > 0;
    }

    public bool DodgeVelocity
    {
        get => animator.GetBool(AnimationStrings.dodgeVelocity);
    }

    public bool JumpVelocity
    {
        get => animator.GetBool(AnimationStrings.jumpVelocity);
    }

    public bool HitVelocity
    {
        get => animator.GetBool(AnimationStrings.hitVelocity);
    }

    public virtual bool DodgeTrigger
    {
        get => animator.GetBool(AnimationStrings.dodgeTrigger);
    }

    public virtual bool JumpTrigger
    {
        get => animator.GetBool(AnimationStrings.jumpTrigger);
    }

    protected override void Start()
    {
        base.Start();
        touchingDirections = GetComponent<TouchingDirections>();
        damageable = GetComponent<DamagableKnight>();
        damageable.ActiveDodges = new Dodge(dodgeChance, dodgeDistance, canDodge, dodgeTime);

        attackCollider = attackZone.GetComponent<Collider2D>();
    }

    private IEnumerator TimeToAttack()
    {
        isAttackCoroutineRunning = true;
        CanMove = false;
        IsIdle = true;
        yield return new WaitForSeconds(delayBeforeImpact);
        Attack = true;
        yield return new WaitForSeconds(delayNumberOfStrokes);
        Attack = false;
        CanMove = true;
        IsIdle = false;
        isAttackCoroutineRunning = false;
    }

    private void FixedUpdate()
    {
        if (animator.GetBool(AnimationStrings.isAlive))
        {
            behaviorStates();
            CheckWaypointBoundaries();
            ChangeDirectionToWaypoints();
            changingSides();
            Jump();
            Dodge();
            if (playerController.IsAlive)
            {
                AttackKnight();
            }

            CheckFallState();
            PlayerHarassment();
            animator.SetFloat(AnimationStrings.yVelocity, rigidBody.velocity.y);
        }

        if (!playerController.IsAlive)
        {
            attackCollider.enabled = false;
            CharacterSpotted = false;
        }
    }

    private void behaviorStates()
    {
        if (playerController.IsAlive)
        {
            IsPlayerVisible();
        }
        EnemyBehavior();
    }
    private void Jump()
    {
        if (EnteringJumpCollider && touchingDirections.IsGrounded && !EnteringWallCollider && !HitVelocity) // good!
        {
            animator.SetTrigger(AnimationStrings.jumpTrigger);
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpImpulse);
        }
    }

    public virtual void Dodge()
    {
        if (damageable.ActiveDodges.IsActiveMode && !HitVelocity)
        {
            rigidBody.velocity = new Vector2(damageable.ActiveDodges.DodgeDistance * walkDirectionVector.x, rigidBody.velocity.y);
        }
    }

    public void AttackKnight()
    {
        if (EnteringAttackingCollider && !isAttackCoroutineRunning && playerVisibility && enemyVisibility && !JumpVelocity && !DodgeVelocity && !HitVelocity) //good!
        {
            StartCoroutine(TimeToAttack());
        }
    }



    public void changingSides()
    {
        if (touchingDirections.IsGrounded && enemyVisibility && EnteringWallCollider && !Attack) //good!
        {
            FlipDirection();
        }
    }


    public void PlayerHarassment()
    {
        if (!JumpVelocity && !DodgeVelocity && !HitVelocity)
        {
            if (EnteringAttackingCollider && !IsIdle)
            {
                rigidBody.velocity = new Vector2(Mathf.Lerp(rigidBody.velocity.x, 0, walkStopRate), rigidBody.velocity.y);
            }
            else if (CanMove && !IsIdle)
            {
                if (!EnteringAttackingCollider)
                {
                    rigidBody.velocity = new Vector2(walkSpeed * walkDirectionVector.x, rigidBody.velocity.y);
                }
            }
            else if (IsIdle)
            {
                rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);
            }
        }
        if (EnteringWallCollider && !HitVelocity)
        {
            rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);
        }

    }

    private void EnemyBehavior()
    {
        if (!CharacterSpotted && enemyVisibility)
        {
            switchTimer += Time.deltaTime;

            if (switchTimer >= (isIdleState ? timeToRun : timeToIdle))
            {
                IsIdle = isIdleState;
                CanMove = !isIdleState;

                isIdleState = !isIdleState;
                if (CanMove)
                {
                    float direction = Random.Range(-1f, 1f);
                    WalkDirection = (direction > 0) ? WalkableDirection.Right : WalkableDirection.Left;

                }
                switchTimer = 0f;
            }
        }
    }

    public override void OnHit(int damage, Vector2 knockback)
    {
        base.OnHit(damage, knockback);
    }

    private void IsPlayerVisible()
    {
        if (playerVisibility && enemyVisibility)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
            float direction = Mathf.Sign(player.transform.position.x - transform.position.x);
            CharacterSpotted = distanceToPlayer <= detectionRange;

            if (CharacterSpotted && WalkDirection != (direction > 0 ? WalkableDirection.Right : WalkableDirection.Left) && !DodgeVelocity && !JumpVelocity && !HitVelocity)
            {
                if (!isChangingDirection)
                {
                    StartCoroutine(DelayedDirectionChange(direction));
                }

            }
        }
        else
        {
            CharacterSpotted = false;
        }
    }

    private IEnumerator DelayedDirectionChange(float newDirection)
    {
        isChangingDirection = true;
        yield return new WaitForSeconds(delayBeforeDirectionChange);
        WalkDirection = (newDirection > 0) ? WalkableDirection.Right : WalkableDirection.Left;
        isChangingDirection = false;
    }

    protected void CheckFallState()
    {
        if (damageable.IsAlive && !touchingDirections.IsGrounded && !EnteringWallCollider)
        {
            fallTimer += Time.fixedDeltaTime;
            if (fallTimer >= maxFallTime)
            {
                Die();
            }
        }
        else
        {
            fallTimer = 0f;
        }
    }

    private void Die()
    {
        damageable.IsAlive = false;
    }
}

