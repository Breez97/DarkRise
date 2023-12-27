using Assets.Scripts.Enemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagableKnight : Damageable
{
    protected Dodge dodgeMode = null;

    public Dodge ActiveDodges { get => dodgeMode; set => dodgeMode = value; }

    public override bool Hit(int damage, Vector2 knockback)
    {
    
        if (IsAlive && !isInvincible) 
        {
            if (dodgeMode != null)
            {
                Dodge dodgeInfo = dodgeMode;

                if (dodgeInfo.IsActiveMode && Random.value < dodgeInfo.DodgeChance)
                {
                    //StartCoroutine(DodgeCoroutine());
                    animator.SetTrigger(AnimationStrings.dodgeTrigger);
                    if (animator!= null)
                    {
                        Debug.Log("WE THERE");
                    }
                    return false;
                }
            }
            Health -= damage;
            isInvincible = true;
           
            animator.SetTrigger(AnimationStrings.hitTrigger);
            
            damageableHit?.Invoke(damage, knockback);

            CharacterEvents.characterDamaged.Invoke(gameObject, damage);

            return true;
        }
        return false;
    }

}
