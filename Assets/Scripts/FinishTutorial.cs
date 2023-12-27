using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishTutorial : MonoBehaviour
{
    private bool isEntered = false;

    private void Awake()
    {
        if (SettingsManager.Instance == null)
        {
            GameObject settingsManagerObject = new GameObject("SettingsManager");
            SettingsManager.Instance = settingsManagerObject.AddComponent<SettingsManager>();
        }
    }

    private void Update()
    {
        if (isEntered) 
        {
            if (Input.GetKeyDown(KeyCode.Return) && Time.timeScale != 0) 
            {
                SettingsManager.Instance.tutorialFinished = true;
                SceneManager.LoadScene("GameplayScene");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) 
        {
            isEntered = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isEntered = false;
        }
    }
}
