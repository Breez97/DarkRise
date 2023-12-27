using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TutorialCanvasManager : MonoBehaviour
{
    [Header("Canvases")]
    [SerializeField] private Canvas pauseCanvas;
    [SerializeField] private Canvas optionsCanvas;

    [SerializeField] private SettingsOptions settingsOptions;

    private PlayerInput playerInput;

    private bool isPaused = false;

    private void Awake()
    {
        Time.timeScale = 1;

        pauseCanvas.enabled = false;
        optionsCanvas.enabled = false;

        playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();

        if (SettingsManager.Instance == null)
        {
            GameObject settingsManagerObject = new GameObject("SettingsManager");
            SettingsManager.Instance = settingsManagerObject.AddComponent<SettingsManager>();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            if (isPaused)
            {
                if (pauseCanvas.enabled) 
                {
                    isPaused = false;
                    Time.timeScale = 1;
                    playerInput.enabled = true;

                    pauseCanvas.enabled = false;
                }
            }
            else 
            {
                isPaused = true;
                Time.timeScale = 0;
                playerInput.enabled = false;

                pauseCanvas.enabled = true;
            }
        }
    }

    public void ReloadTutorial() 
    {
        SceneManager.LoadScene("TutorialScene");
    }

    public void SkipTutorial() 
    {
        SettingsManager.Instance.tutorialFinished = true;
        SettingsManager.Instance.nextScene = "GameplayScene";
        SceneManager.LoadScene(SettingsManager.Instance.nextScene);
    }

    public void OptionsClicked() 
    {
        pauseCanvas.enabled = false;
        optionsCanvas.enabled = true;
    }

    public void MainMenuClicked() 
    {
        StopAllCoroutines();
        SettingsManager.Instance.nextScene = "TutorialScene";
        SettingsManager.Instance.tutorialFinished = false;
        SceneManager.LoadScene("MainMenu");
    }

    public void BackToPauseMenu() 
    {
        pauseCanvas.enabled = true;
        optionsCanvas.enabled = false;
    }
}
