using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StartTutorial : MonoBehaviour
{
    private bool playerTouched = false;

    [SerializeField] private TMP_Text[] text;
    [SerializeField] private Color textColor;
    [SerializeField] private Color invisibleColor;
    [SerializeField] private float colorChangeDuration = 1.0f;

    private void Start()
    {
        for (int i = 0; i < text.Length; i++)
        {
            text[i].color = invisibleColor;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerTouched = true;
            StartCoroutine(ChangeColorOverTime(textColor));
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
        List<Coroutine> coroutines = new List<Coroutine>();

        for (int i = 0; i < text.Length; i++)
        {
            TMP_Text currentText = text[i];
            coroutines.Add(StartCoroutine(ChangeColorOverTimeForText(currentText, targetColor)));
        }
        foreach (var coroutine in coroutines)
        {
            yield return coroutine;
        }
    }

    private IEnumerator ChangeColorOverTimeForText(TMP_Text currentText, Color targetColor)
    {
        Color startColor = currentText.color;
        float elapsedTime = 0f;
        while (elapsedTime < colorChangeDuration)
        {
            currentText.color = Color.Lerp(startColor, targetColor, elapsedTime / colorChangeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        currentText.color = targetColor;
    }
}
