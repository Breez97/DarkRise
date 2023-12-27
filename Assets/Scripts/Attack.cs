using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] protected int attackDamage = 10;
    [SerializeField] protected Vector2 knockback = Vector2.zero;
    protected int knockbackDirection;
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        Damageable damageable = collision.GetComponent<Damageable>();
        if (damageable != null)
        {
            knockbackDirection = transform.parent.localScale.x >= 0 ? 1 : -1;
            bool gotHit = damageable.Hit(attackDamage, knockback * new Vector2(knockbackDirection, 1));
        }
    }
}
