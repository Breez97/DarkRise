using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBottles : MonoBehaviour
{
    [SerializeField] private int amountOfHeal;
    [SerializeField] private int amountOfBottles;
    [SerializeField] private TMP_Text amountOfBottlesText;
    [SerializeField] private Sprite fullBottle;
    [SerializeField] private Sprite emptyBottle;
    [SerializeField] private Animator playerAnimator;

    Damageable playerHealed;

    public int AmountOfBottles
    {
        get { return amountOfBottles; }
        set { amountOfBottles = value; }
    }

    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerHealed = player.GetComponent<Damageable>();
    }

    void Update()
    {
        if (amountOfBottles > 0)
        {
            if (Input.GetKeyDown(KeyCode.F) && playerHealed.IsAlive) 
            {
                playerAnimator.SetTrigger(AnimationStrings.healTrigger);
                playerHealed.Heal(amountOfHeal);
                amountOfBottles--;
            }
            gameObject.GetComponent<Image>().sprite = fullBottle;
        }
        else 
        {
            gameObject.GetComponent<Image>().sprite = emptyBottle;
        }
        amountOfBottlesText.text = amountOfBottles.ToString();
    }
}
