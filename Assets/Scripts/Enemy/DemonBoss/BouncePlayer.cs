using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncePlayer : MonoBehaviour
{
    [SerializeField] private float wallBounceForce = 100f;
    private PlayerController playerController;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    private bool IsRageVelocity
    {
        get => animator.GetBool(AnimationStrings.isRageVelocity);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            BouncePlayerMethod(collision);
        }
    }

    private void BouncePlayerMethod(Collision2D collision)
    {
        playerController.enabled = false;

        if (!IsRageVelocity)
        {
            animator.SetTrigger(AnimationStrings.hitTrigger);
            playerController.Animator.SetTrigger(AnimationStrings.hitTrigger);
        }

        Vector2 bounceDirection = (collision.transform.position - transform.position).normalized;
        playerController.GetComponent<Rigidbody2D>().velocity = new Vector2(bounceDirection.x * wallBounceForce, playerController.GetComponent<Rigidbody2D>().velocity.y);

        playerController.OnHit(0, bounceDirection * wallBounceForce);
        StartCoroutine(EnablePlayerControllerAfterDelay());
    }


    private IEnumerator EnablePlayerControllerAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);
        playerController.enabled = true;
    }
}
