using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonBorder : MonoBehaviour
{
    [SerializeField] private float timeWaitToStart = 6f;
    [SerializeField] private float timeDuration = 10f;
    [SerializeField] private float wallMoveSpeed = 3f;
    [SerializeField] private float offsetPlayerWall = 5f;
    [SerializeField] private float timeToBack = 10f;
 
    private Animator animator;
    private PlayerController playerController;
    private Vector3 originalPosition;
    private bool isMovingWall = false;

    private void Start()
    {
        originalPosition = transform.position;
    }

    private void OnEnable()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
        StartCoroutine(TornadoEffectCoroutine());
    }

    private bool IsRageVelocity
    {
        set => animator.SetBool(AnimationStrings.isRageVelocity, value);
    }

    private IEnumerator TornadoEffectCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeWaitToStart);
            IsRageVelocity = true;
            isMovingWall = true;
            animator.SetTrigger(AnimationStrings.rageAttack);

            yield return new WaitForSeconds(timeDuration);
            isMovingWall = false;
            IsRageVelocity = false;
            ReturnWallToOriginalPosition();
        }
    }

    private void Update()
    {
        if (isMovingWall)
        {
            MoveWallTowardsPlayer();
        }
        if (!playerController.IsAlive)
        {
            Destroy(gameObject);
        }
    }

    private void ReturnWallToOriginalPosition()
    {
        StartCoroutine(MoveWallBack());
    }

    private IEnumerator MoveWallBack()
    {
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        IsRageVelocity = true;
        animator.SetTrigger(AnimationStrings.rageAttack);
        while (elapsedTime < timeToBack)
        {
            float t = elapsedTime / timeToBack; 
            transform.position = Vector3.Lerp(startPosition, originalPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        IsRageVelocity = false;
        transform.position = originalPosition;
    }

    private void MoveWallTowardsPlayer()
    {
        Vector3 directionX = (playerController.transform.position - transform.position).normalized;
        directionX.y = 0f;

        transform.Translate(directionX * wallMoveSpeed * Time.deltaTime);

        if (Mathf.Abs(transform.position.x - playerController.transform.position.x) < offsetPlayerWall)
        {
            isMovingWall = false;
        }
    }
}
