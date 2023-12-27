using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagableBoss : Damageable
{
    public void ReturnToSpawn()
    {
        Health = MaxHealth;
    }
}
