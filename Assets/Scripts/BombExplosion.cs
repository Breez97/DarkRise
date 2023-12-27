using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BombExplosion : MonoBehaviour
{
    [SerializeField] private AnimationCurve curve;

    [SerializeField] private float duration = 1f;
    [SerializeField] private float heightY = 3.0f;

    [SerializeField] private DetectionZone bombCollider; 
    private float distance;

    private GameObject player;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private Animator animator;
    private Rigidbody2D rb;

    public bool EnteringBomb
    {
        get => bombCollider.DetectedColliders.Count > 0;
    }

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        startPosition = transform.position;
        endPosition = new Vector3(player.transform.position.x, player.transform.position.y - 1f, transform.position.z);
        endPosition.x = (startPosition.x - endPosition.x > 0) ? endPosition.x - 2f : endPosition.x + 2f;
        distance = Vector3.Distance(startPosition, endPosition) * 0.15f;
        duration = Mathf.Abs(player.transform.position.x - transform.position.x) / (9.5f * distance);
        heightY = Mathf.Abs(player.transform.position.x - transform.position.x) / (4.5f * distance);
    }

    private void Start()
    {
        StartCoroutine(MoveBomb());
    }

    private void FixedUpdate()
    {
        if (EnteringBomb) 
        {
            AnimatorClipInfo[] currentClipInfo = animator.GetCurrentAnimatorClipInfo(0);
            if (currentClipInfo.Length > 0 && currentClipInfo[0].clip.name == "bomb_flying")
            {
                animator.Rebind();
            }
            animator.SetBool(AnimationStrings.isExplosion, true);
            StopAllCoroutines();
            rb.velocity = Vector2.zero;
        }
    }

    private IEnumerator MoveBomb()
    {
        float timePassed = 0f;

        while (timePassed < duration)
        {
            float linearT = timePassed / duration;
            float heightT = curve.Evaluate(linearT);

            float height = Mathf.Lerp(0f, heightY, heightT);

            Vector3 targetPosition = Vector3.Lerp(startPosition, endPosition, linearT) + Vector3.up * height;
            transform.position = targetPosition;

            timePassed += Time.deltaTime;
            yield return null;
        }
    }
}
