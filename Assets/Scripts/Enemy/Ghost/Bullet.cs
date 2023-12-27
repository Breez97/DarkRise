using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] protected float speed = 5f;
    [SerializeField] protected float maxFlyTime = 3f;
    [SerializeField] protected float maxFollowTime = 0.3f;
    [SerializeField] private GameObject effectAfterDestroyPrefab;

    protected Transform target;
    protected Vector2 lastDirection;
    protected float currentFollowTime;
    protected Animator animator;
    protected bool collided;

    private void Start()
    {
        collided = false;
        StartCoroutine(DisableBulletAfterTime());
        animator = GetComponent<Animator>();
        if (animator != null)
        {
            Debug.Log("Exist");
        }
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    protected virtual void Update()
    {
        if (target != null && !collided)
        {
            Vector2 direction = (target.position - transform.position).normalized;
            if (currentFollowTime < maxFollowTime)
            {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
                currentFollowTime += Time.deltaTime;
                lastDirection = direction;
            }
            else
            {
                transform.position += (Vector3)lastDirection * speed * Time.deltaTime;
            }
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (!collided)
        {
            animator.SetTrigger(AnimationStrings.hitTrigger);
            collided = true;
            StopCoroutine(DisableBulletAfterTime());
            if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                Vector2 bulletPosition = transform.position;

                float groundYCoordinate = other.ClosestPoint(bulletPosition).y;

                Vector2 collisionPoint = new Vector2(bulletPosition.x, groundYCoordinate);
                Instantiate(effectAfterDestroyPrefab, collisionPoint, Quaternion.identity);
            }
                
        }
    }

    public void DestroyBullet()
    {
        Destroy(gameObject);
    }

    protected virtual IEnumerator DisableBulletAfterTime()
    {
        yield return new WaitForSeconds(maxFlyTime);
        if (!collided)
        {
            animator.SetTrigger(AnimationStrings.hitTrigger);
        }
    }
}
