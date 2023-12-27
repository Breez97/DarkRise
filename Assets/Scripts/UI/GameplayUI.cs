using System.Collections;
using System.Collections.Generic;
using System.Resources;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameplayUI : MonoBehaviour
{
    private PlayerInput playerController;

    [Header("Canvases")]
    [SerializeField] private Canvas pauseMenu;
    [SerializeField] private Canvas optionsMenu;
    private bool isPaused = false;

    private void Awake()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();

        pauseMenu.enabled = false;
        optionsMenu.enabled = false;
        Time.timeScale = 1;

        if (SettingsManager.Instance == null)
        {
            GameObject settingsManagerObject = new GameObject("SettingsManager");
            SettingsManager.Instance = settingsManagerObject.AddComponent<SettingsManager>();
        }
    }

    private void Start()
    {
        Debug.Log(SettingsManager.Instance.isFullScreen);
        Screen.SetResolution(SettingsManager.Instance.currentResolution.width, SettingsManager.Instance.currentResolution.height, SettingsManager.Instance.isFullScreen);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
            {
                pauseMenu.enabled = true;
                isPaused = true;
                Time.timeScale = 0;
                playerController.enabled = false;
            }
            else
            {
                if (pauseMenu.enabled)
                {
                    pauseMenu.enabled = false;
                    isPaused = false;
                    Time.timeScale = 1;
                    playerController.enabled = true;
                }
                if (optionsMenu.enabled)
                {
                    pauseMenu.enabled = true;
                    optionsMenu.enabled = false;
                }
            }
        }
    }

    public void OpenOptionsMenu()
    {
        pauseMenu.enabled = false;
        optionsMenu.enabled = true;
    }

    public void BackToPauseMenu()
    {
        pauseMenu.enabled = true;
        optionsMenu.enabled = false;
    }

    public void RestartLevel()
    {
        StopAllCoroutines();
        SceneManager.LoadScene("GameplayScene");
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
