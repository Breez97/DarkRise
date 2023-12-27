using System.Collections;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(DamagableBoss))]
public class EnemyDemonBoss : Enemy
{
    [SerializeField] private Transform basicLocation;
    [SerializeField] private float returnToStartPositionTime = 5f;
    [SerializeField] private DetectionZone attackZone;
    [SerializeField] private float eachPeakSoar = 0.5f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float stopDistance = 2f;
    [SerializeField] private float timeToStartFollow = 3f;
    [SerializeField] private float returnDelayTime = 3f;
    [SerializeField] private GameObject shieldReturn;
    [SerializeField] BossFightManager manager;
    [SerializeField] private float rageAttackCooldown = 20f;
    [SerializeField] private float timeToMoveBetweenPoints = 3f;
    [SerializeField] private float speedSecondAbility = 10f;

    [SerializeField] private float fireballSpawnInterval = 10f;
    [SerializeField] private float timeBeforeNextFireballCreation = 2f;
    [SerializeField] private Fireball fireballPrefab;
    [SerializeField] private Transform fireballSpawnPoint;
    [SerializeField] private float localScaleBullet = 0.5f;

    private bool isChangingDirection = false;
    [SerializeField] private float directionChangeDelay = 1.0f;

    private float lastAttackTime;
    private float distanceToPlayer;
    private bool isReturningToStartPosition = false;
    private bool returnBack = true;
    private bool isFireballAccumulating = false;
    private int maxFireballs = 3;
    DamagableBoss damagableBoss;
    private List<Fireball> activeFireballs = new List<Fireball>();


    private bool isRageAttacking = false;
    private bool isRageStopAttacking = false;


    Coroutine returnCoroutine = null;

    public bool IsRageAttack
    {
        get => animator.GetBool(AnimationStrings.rageAttack);
    }

    public bool IsRageVelocity
    {
        get => animator.GetBool(AnimationStrings.isRageVelocity);

        set => animator.SetBool(AnimationStrings.isRageVelocity, value);
    }

    public bool EnteringAttackingCollider
    {
        get => attackZone.DetectedColliders.Count > 0;
    }
    public BossFightManager Manager { get => manager; set => manager = value; }

    protected override void Start()
    {
        base.Start();
        damagableBoss = GetComponent<DamagableBoss>();
        StartCoroutine(UpdateCoroutine());
    }

    private void Update()
    {
        if (IsAlive)
        {
            CheckWaypointBoundaries();
            ChangeDirectionToWaypoints();
            if (playerController.IsAlive)
            {
                IsPlayerVisible();

                if (CharacterSpotted && playerVisibility)
                {
                    if (!isRageStopAttacking)
                    {
                        if (!isChangingDirection)
                        {
                            StartCoroutine(DelayedDirectionChange());
                        }
                    }

                    if (EnteringAttackingCollider && isRageAttacking)
                    {
                        AttackPlayer();
                    }
                    if (!isRageAttacking && !isRageStopAttacking)
                    {
                        StartCoroutine(RagdeAttackBoss());
                    }
                    if (!isFireballAccumulating && !isRageStopAttacking)
                    {
                        StartCoroutine(FireballAccumulate());
                    }
                }

                if (playerVisibility && returnBack)
                {
                    WalkDirection = (player.transform.position.x > transform.position.x) ? WalkableDirection.Left : WalkableDirection.Right;
                    ResetStartPosition();
                }
            }
            else
            {
                CharacterSpotted = false;
            }

            if (!returnBack && !CharacterSpotted && !playerVisibility || !returnBack && !playerController.IsAlive)
            {
                StopAllCoroutines();
                returnBack = true;
                isRageAttacking = false;
                isRageStopAttacking = false;
                isChangingDirection = false;
                isFireballAccumulating = false;
                IsRageVelocity = false;
                CanMove = true;
                foreach (var fireball in activeFireballs)
                {
                    Destroy(fireball.gameObject);
                }
                activeFireballs.Clear();
                StartCoroutine(UpdateCoroutine());
                returnCoroutine = StartCoroutine(ReturnToStartPosition());
            }
        }
        else
        {
            manager.IsBossDeath();
            foreach (var fireball in activeFireballs)
            {
                Destroy(fireball.gameObject);
            }
            activeFireballs.Clear();
            shieldReturn.SetActive(false);
            StopAllCoroutines();
        }
    }

    private void ResetStartPosition()
    {
        returnBack = false;
        if (returnCoroutine != null)
        {
            isReturningToStartPosition = false;
            StopCoroutine(returnCoroutine);
        }
        if (shieldReturn.activeSelf && !isReturningToStartPosition)
        {
            shieldReturn.SetActive(false);
        }
    }

    private IEnumerator DelayedDirectionChange()
    {
        isChangingDirection = true;
        yield return new WaitForSeconds(directionChangeDelay);
        WalkDirection = (player.transform.position.x > transform.position.x) ? WalkableDirection.Left : WalkableDirection.Right;

        isChangingDirection = false;
    }

    private IEnumerator ReturnToStartPosition()
    {
        isReturningToStartPosition = true;
        float elapsedTime = 0f;
        yield return new WaitForSeconds(returnDelayTime);
        shieldReturn.SetActive(true);
        damagableBoss.ReturnToSpawn();
        WalkDirection = transform.position.x > basicLocation.position.x ? WalkableDirection.Right : WalkableDirection.Left;
        while (elapsedTime < returnToStartPositionTime)
        {
            Vector2 targetPosition = new Vector2(basicLocation.position.x, transform.position.y);

            float step = walkSpeed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, step);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        WalkDirection = WalkDirection == WalkableDirection.Left ? WalkableDirection.Left : WalkableDirection.Right;
        shieldReturn.SetActive(false);
        isReturningToStartPosition = false;
    }



    private IEnumerator UpdateCoroutine()
    {
        while (true)
        {
            Vector2 playerPosition = player.transform.position;
            distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
            if (CharacterSpotted && playerVisibility)
            {
                if (distanceToPlayer > stopDistance && isRageAttacking && !EnteringAttackingCollider && CanMove)
                {
                    yield return StartCoroutine(FollowPlayer(playerPosition));
                }
                else
                {
                    StopCoroutine(FollowPlayer(playerPosition));
                }
            }

            yield return null;
        }
    }

    private IEnumerator FollowPlayer(Vector2 playerPosition)
    {
        float elapsedTime = 0f;

        while (elapsedTime < timeToStartFollow)
        {
            Vector2 direction = (playerPosition - (Vector2)transform.position).normalized;

            Vector2 targetPosition = new Vector2(
                Mathf.Clamp(playerPosition.x - direction.x * stopDistance, minX, maxX),
                playerPosition.y + stopDistance / 2
            );

            float step = walkSpeed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, step);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private void IsPlayerVisible()
    {
        if (playerVisibility && enemyVisibility)
        {
            float distanceX = Mathf.Abs(player.transform.position.x - transform.position.x);
            float distanceY = Mathf.Abs(player.transform.position.y - transform.position.y);
            bool isPlayerInRangeX = distanceX <= detectionRange;
            bool isPlayerInRangeY = distanceY <= detectionRange;

            if (isPlayerInRangeX && isPlayerInRangeY)
            {
                CharacterSpotted = true;
            }
        }
        else
        {
            CharacterSpotted = false;
        }
    }

    private void AttackPlayer()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            animator.SetTrigger(AnimationStrings.hasTarget);
            lastAttackTime = Time.time;
        }
    }

   

    private IEnumerator RagdeAttackBoss()
    {
        isRageAttacking = true;
        yield return new WaitForSeconds(rageAttackCooldown);
        isRageAttacking = false;
        if (!isRageStopAttacking)
        {
            animator.SetTrigger(AnimationStrings.rageAttack);
            isRageStopAttacking = true;
        }
        Vector2 startingWaypoint = (transform.position.x > player.transform.position.x) ? waypointA.position : waypointB.position;

        yield return StartCoroutine(FlyToWaypoint(startingWaypoint));
        yield return StartCoroutine(FlyToWaypoint((startingWaypoint == (Vector2)waypointA.position) ? waypointB.position : waypointA.position));
        yield return StartCoroutine(FlyToWaypoint(startingWaypoint));
        WalkDirection = (player.transform.position.x > transform.position.x) ? WalkableDirection.Left : WalkableDirection.Right;
        IsRageVelocity = false;
        isRageStopAttacking = false;
        
    }

    private IEnumerator FlyToWaypoint(Vector2 targetPosition)
    {
        targetPosition = new Vector2(targetPosition.x, transform.position.y);
        WalkDirection = targetPosition.x > transform.position.x ? WalkableDirection.Left : WalkableDirection.Right;

        while (Vector2.Distance(transform.position, targetPosition) > 0.1f)
        {
            float step = speedSecondAbility * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

            yield return null;
        }
        transform.position = targetPosition;
    }

    private IEnumerator FireballAccumulate()
    {
        isFireballAccumulating = true;

        while (activeFireballs.Count < maxFireballs)
        {
            yield return new WaitForSeconds(fireballSpawnInterval);
            SpawnFireball();
        }

        if (activeFireballs.Count == maxFireballs)
        {
            for (int i = activeFireballs.Count - 1; i >= 0; i--)
            {
                var fireball = activeFireballs[i];

                if (fireball == null || fireball.gameObject == null)
                {
                    activeFireballs.RemoveAt(i);
                    continue;
                }

                fireball.gameObject.transform.parent = null;
                fireball.gameObject.transform.localScale = new Vector3(-localScaleBullet, player.transform.position.x < transform.position.x ? -localScaleBullet : localScaleBullet, 0f);
                fireball.SetTarget(player.transform);
                fireball.IsActive = true;
            }

            activeFireballs.Clear();
            yield return new WaitForSeconds(timeBeforeNextFireballCreation);
        }

        isFireballAccumulating = false;
    }
   
    private void SpawnFireball()
    {
        Fireball newFireball = Instantiate(fireballPrefab, fireballSpawnPoint.position, Quaternion.identity);
        activeFireballs.Add(newFireball);
        newFireball.transform.parent = fireballSpawnPoint;
        float enemyScaleX = gameObject.transform.localScale.x;
        newFireball.transform.localScale = new Vector3(localScaleBullet,
            newFireball.transform.localScale.y,
            newFireball.transform.localScale.z
        );
        float angle = 360f / 3 * activeFireballs.Count;
        float radius = 1.5f;
        float x = radius * Mathf.Cos(angle * Mathf.Deg2Rad);
        float y = radius * Mathf.Sin(angle * Mathf.Deg2Rad);

        newFireball.transform.localPosition = new Vector2(x, y);
    }
}
