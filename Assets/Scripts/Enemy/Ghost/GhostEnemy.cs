using Assets.Scripts.Enemy.Ghost;
using System.Collections;
using UnityEngine;
using DG.Tweening;
[RequireComponent(typeof(Animator), typeof(Rigidbody2D), typeof(DamagableGhost))]
public class GhostEnemy : Enemy
{
    [Header("Смещение по осям x и y")]
    [SerializeField] private float offsetX;
    [SerializeField] private float offsetY;
    [Space]
    [Header("Время простоя и движения")]
    [SerializeField] private float idleTime;
    [SerializeField] private float moveTime;
    [Header("Вверхняя и нижняя граница")]
    [SerializeField] private float minY;
    [SerializeField] private float maxY;
    [Header("Расстояние для остановки при обнаружении игрока")]
    [SerializeField] private float stopDistance = 1.5f;
    [SerializeField] private float timeToStartFollow = 2f;
    [SerializeField] private float moveSpeed = 3.0f;
    [Header("Время между атаками")]
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float attackDistance = 2f;
    [SerializeField] private float sizeBullet = 0.75f;
    [Space]
    [SerializeField] private GameObject bulletPrefab;
    [Header("Возможность исчезнуть во время атаки игрока")]
    [SerializeField] private bool canVanish;
    [SerializeField] private float chanceVanish;
    [SerializeField] private float timeToWaitReloadAbility;
    [SerializeField] private float rangeAppearance;
    [SerializeField] private float coefficientHeightOnPlayer;
    [SerializeField] private float timeToAppear = 3f;
    [Header("Призрак во время спуска вниз")]
    [SerializeField] private float timeToWaitBeforeLift;
    [SerializeField] private float timeToWaitAfterLift;
    [SerializeField] private float slowdownAttack = 1.5f;
    private GameObject ghostShieldPrefab;
    private bool isAttacking = false;
    private bool canKnock = true;
    private float slowAttackCooldown;
    private float lastAttackTime;
    private float distanceToPlayer;
    private DamagableGhost damagable;
    Vector2 tempPosition;
    SpriteRenderer spriteRenderer;
    private bool shouldStopRandomMovement = false;
    private bool shouldStopMoveToPosition = false;
    public bool AppearVelocity { get => animator.GetBool(AnimationStrings.appearVelocity);}

    protected override void Start()
    {
        base.Start();
        slowAttackCooldown = slowdownAttack * attackCooldown;
        ghostShieldPrefab = transform.Find("GhostShield").gameObject;
        if (ghostShieldPrefab != null)
        {
            Debug.Log("Exist shield!");
        }
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (canVanish)
        {
            damagable = GetComponent<DamagableGhost>();
            damagable.SetVanish = new Vanish(true, chanceVanish, timeToWaitReloadAbility);
        }
            
        CheckWaypointBoundaries();
        StartCoroutine(UpdateCoroutine());
    }

    private bool isDeadHero = false;
    private void Update()
    {
        if (!isDeadHero && IsAlive)
        { 
            CheckWaypointBoundaries();
            ChangeDirectionToWaypoints();

            if (playerController.IsAlive)
            {
                distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
                if (CharacterSpotted && playerVisibility)
                    WalkDirection = (player.transform.position.x > transform.position.x) ? WalkableDirection.Left : WalkableDirection.Right;
                IsPlayerVisible();

                if (distanceToPlayer <= attackDistance && CharacterSpotted && playerVisibility && transform.position.y > player.transform.position.y && AppearVelocity)
                {

                    AttackPlayer();
                    if (!isAttacking)
                    {
                        StartCoroutine(AttackPlayerCoroutine());
                    }
                }
            }
            else
            {
                damagable.IsShieldActive = false;
                ghostShieldPrefab.SetActive(false);
                playerVisibility = false;
                isDeadHero = true;
                SettingsSpotted(false);
                StopAllCoroutinesExceptRandomMovement();
            }
            
            if (animator.GetBool(AnimationStrings.dodgeTrigger))
            {
                damagable.IsShieldActive = false;
                ghostShieldPrefab.SetActive(false);
            }
            

        }
        else if (!IsAlive)
        {
            damagable.IsShieldActive = false;
            ghostShieldPrefab.SetActive(false);
            StopAllCoroutines();
        }
    }

    private void StopAllCoroutinesExceptRandomMovement()
    {
        StopAllCoroutines();
        StartCoroutine(RandomMovement());
    }

    private void IsPlayerVisible()
    {
        if (player != null && playerVisibility)
        {
            float distanceX = Mathf.Abs(player.transform.position.x - transform.position.x);
            float distanceY = Mathf.Abs(player.transform.position.y - transform.position.y);
            bool isPlayerInRangeX = distanceX <= detectionRange;
            bool isPlayerInRangeY = distanceY <= detectionRange;

            if (isPlayerInRangeX && isPlayerInRangeY)
            {
                SettingsSpotted(true);
            }
            else
            {
                SettingsSpotted(false);
            }
        }
        else
        {
            SettingsSpotted(false);
        }
        
    }

    private void SettingsSpotted(bool flag)
    {
        CharacterSpotted = flag;
        shouldStopRandomMovement = flag;
        shouldStopMoveToPosition = flag;
    }

    private IEnumerator UpdateCoroutine()
    {
        while (true)
        {
            Vector2 playerPosition = player.transform.position;
            if (CharacterSpotted && playerVisibility && AppearVelocity)
            {
                if (distanceToPlayer > stopDistance && !isAttacking)
                {
                    
                    yield return StartCoroutine(FollowPlayer(playerPosition));
                }
            }
            else if (AppearVelocity)
            {
                yield return StartCoroutine(RandomMovement());
            }
            else if (!AppearVelocity)
            {
                StopCoroutine(FollowPlayer(playerPosition));
                StopCoroutine(RandomMovement());
            }

            yield return null;
        }
    }

    private IEnumerator RandomMovement()
    {
        while (!playerVisibility && !CharacterSpotted)
        {
            if (shouldStopRandomMovement)
            {
                break;
            }
            yield return new WaitForSeconds(idleTime);
            if (shouldStopRandomMovement)
            {
                break;
            }
            Vector2 randomOffset = new Vector2(
                Random.Range(-offsetX, offsetX),
                Random.Range(-offsetY, offsetY)
            );

            Vector2 newPosition = (Vector2)transform.position + randomOffset;
            newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
            newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

            yield return StartCoroutine(MoveToPosition(newPosition));
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
                playerPosition.y + stopDistance
            );

            targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

            float step = moveSpeed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, step);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }


    private IEnumerator MoveToPosition(Vector2 targetPosition)
    {
        float elapsedTime = 0f;
        Vector2 startingPos = transform.position;

        while (elapsedTime < moveTime)
        {
            if (shouldStopMoveToPosition)
            {
                yield break;
            }
            transform.position = Vector2.Lerp(startingPos, targetPosition, elapsedTime / moveTime);
            elapsedTime += Time.deltaTime;
            WalkDirection = (targetPosition.x > transform.position.x) ? WalkableDirection.Left : WalkableDirection.Right;

            yield return null;
        }

        transform.position = targetPosition; 
    }

    private IEnumerator AttackPlayerCoroutine()
    {
        isAttacking = true;
        canKnock = true;
        yield return new WaitForSeconds(timeToWaitBeforeLift);
        float targetY = Mathf.Clamp(player.transform.position.y, minY, maxY);
        canKnock = false;
        yield return StartCoroutine(LiftGhostToPlayerLevel(targetY));
        yield return new WaitForSeconds(timeToWaitAfterLift);
        isAttacking = false;
        shouldStopMoveToPosition = true;
        damagable.IsShieldActive = false;
        ghostShieldPrefab.SetActive(false);
        StartCoroutine(FollowPlayer(player.transform.position));
    }

    private IEnumerator LiftGhostToPlayerLevel(float targetY)
    {
        StopCoroutine(FollowPlayer(player.transform.position));
        shouldStopMoveToPosition = false;
        damagable.IsShieldActive = true;
        ghostShieldPrefab.SetActive(true);
        
        float startY = transform.position.y;

        float elapsedTime = 0f;
        float liftDuration = 1.0f; 

        while (elapsedTime < liftDuration)
        {
            float newY = Mathf.Lerp(startY, targetY, elapsedTime / liftDuration);
            transform.position = new Vector2(transform.position.x, newY);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.position = new Vector2(transform.position.x, targetY);
    }

    private void AttackPlayer()
    {
        float attackCooldownTemp = attackCooldown;
        if (damagable.IsShieldActive)
        {
            attackCooldownTemp = slowAttackCooldown;
        }
        if (Time.time - lastAttackTime >= attackCooldownTemp)
        {
            animator.SetTrigger(AnimationStrings.hasTarget);
            lastAttackTime = Time.time;
        }
    }


    public void AttackAfterAnimationDelay()
    {
        GameObject bulletObject = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

        if (bulletObject != null)
        {
            Bullet bullet = bulletObject.GetComponent<Bullet>();

            if (bullet != null)
            {
                bullet.SetTarget(player.transform);
                bulletObject.transform.localScale = new Vector3(sizeBullet, WalkDirection == WalkableDirection.Left ? sizeBullet : -sizeBullet, 0f);
            }
        }
    }

    public void GhostVanishEvent()
    {
        if (canVanish && damagable != null && damagable.SetVanish != null)
        {
            
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = false;
            }
            Invoke("GhostVanishEventDelayed", timeToAppear);
        }
    }

    private void GhostVanishEventDelayed()
    {
        WalkDirection = player.transform.localScale.x > 0 ? WalkableDirection.Left : WalkableDirection.Right;
        float teleportX = player.transform.position.x + (WalkDirection == WalkableDirection.Left ? -rangeAppearance : rangeAppearance);
        float teleportY = Mathf.Clamp(player.transform.position.y + stopDistance * coefficientHeightOnPlayer, minY, maxY);

        teleportX = Mathf.Clamp(teleportX, minX, maxX);
        transform.position = new Vector2(teleportX, teleportY);
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }

        damagable.SetVanish.IsActive = true;
        animator.SetTrigger(AnimationStrings.vanishTrigger);
    }

    public override void OnHit(int damage, Vector2 knockback)
    {
        if (canKnock)
        {
            Vector2 knockbackDirection = knockback.normalized;
            Vector2 targetPosition = (Vector2)transform.position + knockbackDirection * knockback.magnitude;
            targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);
            transform.DOMove(targetPosition, 0.5f).SetEase(Ease.OutQuad);
        }
    }
}
