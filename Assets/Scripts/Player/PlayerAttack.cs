using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : Attack
{
    //[SerializeField] protected int attackDamage = 10;
    //[SerializeField] protected Vector2 knockback = Vector2.zero;
    //protected int knockbackDirection;
    private PlayerRageEffect playerRageEffect;
    private PlayerAudioManager playerAudioManager;

    private void Awake()
    {
        playerRageEffect = GetComponentInParent<PlayerRageEffect>();
        playerAudioManager = GetComponentInParent<PlayerAudioManager>();    
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        Damageable damageable = collision.GetComponent<Damageable>();
        if (damageable != null)
        {
            playerAudioManager.PlayAttackHit();
            knockbackDirection = transform.parent.localScale.x >= 0 ? 1 : -1;
            if (playerRageEffect.IsRageEffect)
            {
                damageable.Hit((int)(attackDamage * playerRageEffect.rageDamageMultiplier), knockback * new Vector2(knockbackDirection, 1));
            }
            else
            {
                playerRageEffect.IncreaseDamage(attackDamage);  
                damageable.Hit(attackDamage, knockback * new Vector2(knockbackDirection, 1));
            }
        }
    }
}
