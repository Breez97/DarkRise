using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FlyingEye : MonoBehaviour
{
    [SerializeField] private float flightSpeed = 2f;
    [SerializeField] private float waypointReachedDistance = 0.1f;

    int waypointNum = 0;
    Transform nextWaypoint;
    private bool foundPlayer = false;
    int countWaypoint = 0;

    [SerializeField] private DetectionZone biteDetectionZone;
    [SerializeField] private DetectionZone wallDetectionZone;

    [SerializeField] private Collider2D deathCollider;
    [SerializeField] private List<Transform> waypoints;

    private bool canFly = true;

    Animator animator;
    Rigidbody2D rb;
    Damageable damageable;
    GameObject player;
    Damageable playerDamageable;

    private bool _hasTarget = false;
    public bool HasTarget
    {
        get { return _hasTarget; }
        private set
        {
            if (!playerDamageable.IsAlive)
            {
                value = false;
            }
            _hasTarget = value;
            animator.SetBool(AnimationStrings.hasTarget, value);
            damageable = GetComponent<Damageable>();
        }
    }

    public bool EnteringWallCollider 
    {
        get => wallDetectionZone.DetectedColliders.Count > 0;
    }

    public bool CanMove
    {
        get => animator.GetBool(AnimationStrings.canMove);
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        damageable = GetComponent<Damageable>();
        player = GameObject.FindWithTag("Player");
        playerDamageable = player.GetComponent<Damageable>();
    }

    private void Start()
    {
        nextWaypoint = waypoints[waypointNum];
    }

    private void OnEnable()
    {
        damageable.damageableDeath.AddListener(OnDeath);
    }

    void Update()
    {
        HasTarget = biteDetectionZone.DetectedColliders.Count > 0;
        if (HasTarget && playerDamageable.IsAlive)
        {
            foundPlayer = true;
            countWaypoint = 1;
        }

        if (animator.GetBool(AnimationStrings.hitTrigger))
        {
            foundPlayer = true;
            canFly = false;
            StartCoroutine(WaitAfterAttack());
        }
    }

    private void FixedUpdate()
    {
        if (damageable.IsAlive)
        {
            if (CanMove) 
            {
                Flight();
            }
            else
            {
                rb.velocity = Vector3.zero;
            }
        }
        if (playerDamageable.IsAlive == false) 
        {
            foundPlayer = false;
            if (countWaypoint == 1) 
            {
                float minDistance = 0f;
                int numMinWaypoint = 0;
                for (int i = 0; i < waypoints.Count; i++)
                {
                    float distance = Vector2.Distance(waypoints[i].position, transform.position);
                    if (minDistance > distance)
                    {
                        minDistance = distance;
                        numMinWaypoint = i;
                    }
                }
                waypointNum = numMinWaypoint;
                countWaypoint--;
            }
        }
    }

    private IEnumerator WaitAfterAttack() 
    {
        yield return new WaitForSeconds(0.3f);
        canFly = true;
    }

    private void Flight() 
    {
        if (foundPlayer == false)
        {
            Vector2 directionToWaypoint = (nextWaypoint.position - transform.position).normalized;

            float distance = Vector2.Distance(nextWaypoint.position, transform.position);

            if (canFly) 
            {
                rb.velocity = directionToWaypoint * flightSpeed;
                UpdateDirection();
            }

            if (distance <= waypointReachedDistance)
            {
                waypointNum++;
                if (waypointNum >= waypoints.Count)
                {
                    waypointNum = 0;
                }
                nextWaypoint = waypoints[waypointNum];
            }
        }
        else 
        {
            Vector2 playerPosition = player.transform.position;

            playerPosition.y += 0.5f;

            Vector2 directionToPlayer = (playerPosition - (Vector2)transform.position).normalized;

            if (canFly) 
            {
                rb.velocity = directionToPlayer * flightSpeed;
            }

            if (Vector2.Distance(playerPosition, (Vector2)transform.position) > 0.2f)
            {
                if (canFly) 
                {
                    UpdateDirection();
                }
            }
            else
            {
                rb.velocity = Vector2.zero;
            }

            if (Vector2.Distance(playerPosition, (Vector2)transform.position) > 10f) 
            {
                foundPlayer = false;
            }
        }
    }

    private void UpdateDirection() 
    {
        Vector3 locScale = transform.localScale;

        if (transform.localScale.x > 0)
        {
            if(rb.velocity.x < 0) 
            {
                transform.localScale = new Vector3(-1 * locScale.x, locScale.y, locScale.z);
            }
        }
        else 
        {
            if (rb.velocity.x > 0)
            {
                transform.localScale = new Vector3(-1 * locScale.x, locScale.y, locScale.z);
            }
        }
    }

    public void OnDeath() 
    {
        rb.gravityScale = 2f;
        rb.velocity = new Vector2(0, rb.velocity.y);
        deathCollider.enabled = true;
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
    }
}
