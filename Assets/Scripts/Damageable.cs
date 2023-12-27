using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    public UnityEvent<int, Vector2> damageableHit;
    public UnityEvent damageableDeath;
    public UnityEvent<int, int> healthChanged;

    protected Animator animator;

    [SerializeField] protected int maxHealth = 100;
    [SerializeField] protected int health = 100;

    protected float timeSinceHit = 0;
    [SerializeField] protected float invincibilityTimer = 0.25f;
    [SerializeField] protected bool isInvincible = false;

    public int MaxHealth 
    {
        get { return maxHealth; }
        set { maxHealth = value; }
    }

    public int Health 
    {
        get { return health; }
        set
        {
            health = value;

            healthChanged?.Invoke(health, MaxHealth);
            if (health <= 0) 
            {
                IsAlive = false;
            }
        }
    }
    private bool isAlive = true;
    public bool IsAlive
    {
        get { return isAlive; }
        set 
        {
            isAlive = value;
            animator.SetBool(AnimationStrings.isAlive, value);

            if (value == false) 
            {
                damageableDeath.Invoke();
            }
        }
    }

    protected virtual void Awake() 
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isInvincible) 
        {
            if (timeSinceHit > invincibilityTimer) 
            {
                isInvincible = false;
                timeSinceHit = 0;
            }

            timeSinceHit += Time.deltaTime;
        }
    }

    public virtual bool Hit(int damage, Vector2 knockback)
    {
        
        if (IsAlive && !isInvincible) 
        {
            Health -= damage;
            isInvincible = true;
           
            animator.SetTrigger(AnimationStrings.hitTrigger);
            
            damageableHit?.Invoke(damage, knockback);

            CharacterEvents.characterDamaged.Invoke(gameObject, damage);

            return true;
        }
        return false;
    }

    public bool Heal(int healthRestore) 
    {
        if (IsAlive && Health < MaxHealth) 
        {
            int maxHeal = Mathf.Max(MaxHealth - Health, 0);
            int actualHealth = Mathf.Min(maxHeal, healthRestore);
            Health += actualHealth;

            CharacterEvents.characterHealed(gameObject, actualHealth);

            return true;
        }
        return false;
    }
}
