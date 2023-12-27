using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AbilityBar : MonoBehaviour
{
    [SerializeField] private Slider abilitySlider;
    [SerializeField] private Image barImage;
    private float amountOfAbility = 0f;

    [SerializeField] private TMP_Text text;
    [SerializeField] private Color textColor;
    [SerializeField] private Color invisibleColor;
    [SerializeField] private float colorChangeDuration = 1.0f;

    private PlayerRageEffect playerRageEffect;
    private Animator barAnimator;

    private void Awake()
    {
        barAnimator = barImage.GetComponent<Animator>();
        playerRageEffect = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerRageEffect>();
        text.color = invisibleColor;
    }

    private void Start()
    {
        abilitySlider.value = 0f;
    }

    private void Update()
    {
        ChangeAmountOfAbility();
        ChangeAnimationRage();
        ShowTextWithTween();
    }

    private void ChangeAmountOfAbility()
    {
        amountOfAbility = playerRageEffect.CurrentRageBarValue / 100f;

        if (amountOfAbility > 1f)
        {
            amountOfAbility = 1f;
        }

        abilitySlider.value = amountOfAbility;
    }

    private void ChangeAnimationRage()
    {
        if (playerRageEffect.IsRageEffect)
        {
            if (!barAnimator.GetBool(AnimationStrings.isRage))
            {
                barAnimator.SetBool(AnimationStrings.isRage, true);
            }
        }
        else
        {
            if (barAnimator.GetBool(AnimationStrings.isRage))
            {
                barAnimator.SetBool(AnimationStrings.isRage, false);
            }
        }
    }

    private void ShowTextWithTween()
    {
        if (abilitySlider.value == 1f)
        {
            text.DOFade(1f, colorChangeDuration);
        }
        else
        {
            text.DOFade(0f, colorChangeDuration);
        }
    }
}
