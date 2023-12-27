using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerRageEffect : MonoBehaviour
{
    public float rageDamageMultiplier = 2f;
    public float rageSpeedMultiplier = 1.5f;
    [SerializeField] GameObject rageEffectLight;
    [SerializeField] private float rageTime = 10f;
    [SerializeField] private int targetDamageToActivate = 1000;
    //private int defaultRageBarValue = 100;
    private int currentRageBarValue = 0;
    private int currentDamage = 0;
    private bool isRageEffect = false;
    public bool IsRageEffect { get =>  isRageEffect; }

    public int CurrentRageBarValue 
    {
        get => currentRageBarValue;
    }

    private bool canRage = false;
    public void IncreaseDamage(int damage)
    {
        if (currentDamage < targetDamageToActivate)
        {
            currentDamage += damage;
            currentRageBarValue += damage * 100 / targetDamageToActivate;
        }
        else
        {
            currentDamage = targetDamageToActivate;
        }

        if (currentDamage >= targetDamageToActivate)
        {
            print("CAN RAGE!");
            canRage = true;
        }
    }

    public void OnRageEffect()
    {
        print("RAGE");
        if (canRage)
        {
            print("startRage");
            isRageEffect= true; 
            rageEffectLight.SetActive(true);
            DOTween.To(() => currentRageBarValue, value => currentRageBarValue = value, 0, rageTime)
                .OnComplete(() => { isRageEffect = false; canRage = false; rageEffectLight.SetActive(false); currentDamage = 0; print("endRage"); });
        }
    }
}
