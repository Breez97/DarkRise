using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostAttack : Attack
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        Damageable damageable = collision.GetComponent<Damageable>();
        if (damageable != null)
        {
            knockbackDirection = transform.localScale.x >= 0 ? 1 : -1;
            bool gotHit = damageable.Hit(attackDamage, knockback * new Vector2(knockbackDirection, 1));
        }
    }
}
