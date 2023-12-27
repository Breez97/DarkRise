using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireAttack : MonoBehaviour
{
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private Vector2 knockback = Vector2.zero;
    [SerializeField] private float attackDelay = 0.5f;
    private int knockbackDirection;
    private Coroutine currentAttackCoroutine;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Damageable damageable = collision.GetComponent<Damageable>();

        currentAttackCoroutine = StartCoroutine(Attack(damageable));
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(currentAttackCoroutine != null)
            StopCoroutine(currentAttackCoroutine);
    }
    private IEnumerator Attack(Damageable damageable)
    {
        if(damageable != null)
        {
            while (true)
            {

                knockbackDirection = transform.parent.localScale.x >= 0 ? 1 : -1;

                Debug.Log(knockbackDirection);
                Debug.Log(transform.parent.localScale.x);
                damageable.Hit(attackDamage, knockback * new Vector2(knockbackDirection, 1));
                yield return new WaitForSeconds(attackDelay);
            }
        }
        else { yield return null; } 
        
    }

}
