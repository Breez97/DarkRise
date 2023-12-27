using System.Collections;
using UnityEngine;
using TMPro;

public class HealRestore : MonoBehaviour
{
    private bool playerTouched = false;

    [SerializeField] private int maxAmountOfBottles;
    [SerializeField] private TMP_Text text;
    [SerializeField] private Color textColor;
    [SerializeField] private Color invisibleColor;
    [SerializeField] private float colorChangeDuration = 1.0f;
    private int textChangedState = 0;

    HealthBottles healthBottles;
    Damageable playerHealed;

    void Start()
    {
        GameObject bottles = GameObject.Find("HealthBottles");
        healthBottles = bottles.GetComponent<HealthBottles>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerHealed = player.GetComponent<Damageable>();

        text.color = invisibleColor;
    }

    void Update()
    {
        if (playerTouched && Input.GetKeyDown(KeyCode.E))
        {
            if (healthBottles.AmountOfBottles == maxAmountOfBottles && playerHealed.Health == playerHealed.MaxHealth)
            {
                textChangedState = 4;
            }
            else 
            {
                if (healthBottles.AmountOfBottles == maxAmountOfBottles && playerHealed.Health != playerHealed.MaxHealth) 
                {
                    textChangedState = 1;
                }
                if (healthBottles.AmountOfBottles != maxAmountOfBottles && playerHealed.Health == playerHealed.MaxHealth) 
                {
                    textChangedState = 2;
                }
                if (healthBottles.AmountOfBottles != maxAmountOfBottles && playerHealed.Health != playerHealed.MaxHealth) 
                {
                    textChangedState = 3;
                }
                healthBottles.AmountOfBottles = maxAmountOfBottles;
                playerHealed.Health = playerHealed.MaxHealth;
            }
        }

        switch (textChangedState)
        {
            case 0:
                text.text = "Нажмите Е для восстановления здоровья и бутылок";
                break;
            case 1:
                text.text = "Здоровье восстановлено";
                break;
            case 2:
                text.text = "Бутылки востановленны";
                break;
            case 3:
                text.text = "Здоровье и бутылки востановленны";
                break;
            case 4:
                text.text = "Здоровье и бутылки полные";
                break;
            default:
                text.text = "Нажмите Е для восстановления здоровья";
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerTouched = true;
            StartCoroutine(ChangeColorOverTime(textColor));
            textChangedState = 0;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerTouched = false;
            StartCoroutine(ChangeColorOverTime(invisibleColor));
        }
    }

    private IEnumerator ChangeColorOverTime(Color targetColor)
    {
        Color startColor = text.color;
        float elapsedTime = 0f;
        while (elapsedTime < colorChangeDuration)
        {
            text.color = Color.Lerp(startColor, targetColor, elapsedTime / colorChangeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        text.color = targetColor;
    }
}
