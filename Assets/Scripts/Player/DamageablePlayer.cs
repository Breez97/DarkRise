using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DamageablePlayer : Damageable
{
    private float damageDecreaseMultiplier;
    private bool isShieldActivate = false;
    private PlayerMagicSpells playerMagicSpells;

    protected override void Awake()
    {
        base.Awake();   
        playerMagicSpells = GetComponent<PlayerMagicSpells>();
    }
    public override bool Hit(int damage, Vector2 knockback)
    {

        if (isShieldActivate) 
        {
            damage = (int) (damage * damageDecreaseMultiplier);
        }
        return base.Hit(damage, knockback);
    }

    public void OnShieldActivate()
    {
        damageDecreaseMultiplier = playerMagicSpells.DamageDecreaseMultiplier ;  
        isShieldActivate = true;    
    }
    public void OnShieldDeactivate()
    {
        isShieldActivate = false;
    }
}
