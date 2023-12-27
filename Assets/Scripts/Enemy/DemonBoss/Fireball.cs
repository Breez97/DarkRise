using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : Bullet
{
    private bool isActive = false;

    public bool IsActive { get => isActive; set => isActive = value; }
    [SerializeField] private float offSet;
    [SerializeField] private GameObject enemyPortal;
    [SerializeField] private float chanceToCreate;
    private Transform waypointA; 
    private Transform waypointB;
    private EnemyDemonBoss enemyDemonBoss;
    private BossFightManager manager;
    void Start()
    {        
        enemyDemonBoss = FindObjectOfType<EnemyDemonBoss>();

        if (enemyDemonBoss != null)
        {
            
            manager = enemyDemonBoss.Manager;
            waypointA = enemyDemonBoss.WaypointA;
            waypointB = enemyDemonBoss.WaypointB;
        }
        animator = GetComponent<Animator>();
    }

    protected override void Update()
    {
        if (IsActive)
        {
            base.Update();
        }
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        animator.SetTrigger(AnimationStrings.hitTrigger);
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.transform.localScale = new Vector3(0.2f, 0.2f, 1f);
        collided = true;
        StopCoroutine(DisableBulletAfterTime());
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            if (Random.value <= chanceToCreate && manager.CanCreate)
            {
                Vector2 portalPosition = transform.position;

                float groundYCoordinate = other.ClosestPoint(portalPosition).y + offSet + Random.Range(-2f, 2f);
                float randomX = Random.Range(waypointA.position.x + 2f, waypointB.position.x - 2f);

                Vector2 collisionPoint = new Vector2(randomX, groundYCoordinate);
                Instantiate(enemyPortal, collisionPoint, Quaternion.identity);
            }
        }
    }
}
