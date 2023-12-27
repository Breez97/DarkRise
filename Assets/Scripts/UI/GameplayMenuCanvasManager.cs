using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameplayMenuCanvasManager : MonoBehaviour
{
    [SerializeField] private Canvas pauseCanvas;
    [SerializeField] private Canvas optionsCanvas;
    [SerializeField] private Canvas playerDeath;

    [SerializeField] private SettingsOptions settingsOptions;

    private bool isPaused = false;
    private bool isDeath = false;

    private PlayerInput playerInput;

    private Damageable playerDamageable;

    private void Awake()
    {
        Time.timeScale = 1;

        pauseCanvas.enabled = false;
        optionsCanvas.enabled = false;
        playerDeath.enabled = false;

        playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
        playerDamageable = GameObject.FindGameObjectWithTag("Player").GetComponent<Damageable>();
    }

    private void Update()
    {
        if (!playerDamageable.IsAlive) 
        {
            isDeath = true;
        }

        SetOnPause();
    }

    private void SetOnPause() 
    {
        if (!isDeath)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (isPaused)
                {
                    if (pauseCanvas.enabled)
                    {
                        isPaused = false;
                        Time.timeScale = 1;

                        pauseCanvas.enabled = false;
                        playerInput.enabled = true;
                    }
                    else
                    {
                        if (optionsCanvas.enabled)
                        {
                            optionsCanvas.enabled = false;
                            pauseCanvas.enabled = true;
                        }
                    }
                }
                else
                {
                    isPaused = true;
                    Time.timeScale = 0;

                    pauseCanvas.enabled = true;
                    playerInput.enabled = false;
                }
            }
        }
        else 
        {
            Time.timeScale = 0;
            playerDeath.enabled = true;
        }
    }

    public void ReloadGame() 
    {
        StopAllCoroutines();
        SceneManager.LoadScene("GameplayScene");
    }

    public void OptionsClicked() 
    {
        settingsOptions.SetVolume();
        pauseCanvas.enabled = false;
        optionsCanvas.enabled = true;
    }

    public void BackToPauseMenu() 
    {
        if (!isDeath) 
        {
            pauseCanvas.enabled = true;
        }
        else
        {
            playerDeath.enabled = true;
        }
        optionsCanvas.enabled = false;
    }

    public void BackToMainMenu() 
    {
        SceneManager.LoadScene("MainMenu");
    }
}
