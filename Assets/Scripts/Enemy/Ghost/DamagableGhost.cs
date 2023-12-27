using Assets.Scripts.Enemy.Ghost;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagableGhost : Damageable
{
    private Vanish vanish;

    public Vanish SetVanish
    {
        get => vanish;
        set => vanish = value;
    }

    private bool isShieldActive = false;
    [SerializeField] private int coefficientDefendWithShield = 2;
    public bool IsShieldActive { get => isShieldActive; set => isShieldActive = value; }

    public override bool Hit(int damage, Vector2 knockback)
    {
        if (IsAlive && !isInvincible && animator.GetBool(AnimationStrings.appearVelocity))
        {
            if (vanish != null && vanish.IsActive && vanish.CanVanish())
            {
                if (Random.value < vanish.Chance)
                {
                    animator.SetTrigger(AnimationStrings.dodgeTrigger);
                    vanish.UseVanish();

                    return false;
                }
            }
            
            if (IsShieldActive)
            {
                damage = damage / coefficientDefendWithShield;
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

