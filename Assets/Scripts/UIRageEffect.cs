using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIRageEffect : MonoBehaviour
{
    [SerializeField] private Image barImage;

    private Animator barAnimator;
    private PlayerRageEffect playerRageEffect;

    private void Awake()
    {
        barAnimator = barImage.GetComponent<Animator>();
        playerRageEffect = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerRageEffect>();
    }

    private void FixedUpdate()
    {
        if (playerRageEffect.IsRageEffect)
        {
            barAnimator.SetBool(AnimationStrings.isRage, true);
        }
        else 
        {
            barAnimator.SetBool(AnimationStrings.isRage, false);
        }
    }
}
