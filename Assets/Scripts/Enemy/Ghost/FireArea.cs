using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireArea : MonoBehaviour
{
    [SerializeField] private float lifetime = 1f; 
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(DestroyAfterLifetime());
    }

    private IEnumerator DestroyAfterLifetime()
    {
        yield return new WaitForSeconds(lifetime);

        animator.SetTrigger(AnimationStrings.hitTrigger);
        
    }

    public void DestroyAfterEndAnimation()
    {
        Destroy(gameObject);
    }
}
