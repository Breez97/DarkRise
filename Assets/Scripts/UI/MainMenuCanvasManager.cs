using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuCanvasManager : MonoBehaviour
{
    [Header("Canvases")]
    [SerializeField] private Canvas menuCanvas;
    [SerializeField] private Canvas optionsCanvas;

    [SerializeField] private SettingsOptions settingsOptions;

    private void Awake()
    {
        menuCanvas.enabled = true;
        optionsCanvas.enabled = false;

        if (SettingsManager.Instance == null)
        {
            GameObject settingsManagerObject = new GameObject("SettingsManager");
            SettingsManager.Instance = settingsManagerObject.AddComponent<SettingsManager>();
        }
    }

    private void Start()
    {
        if (SettingsManager.Instance.tutorialFinished)
        {
            SettingsManager.Instance.nextScene = "GameplayScene";
        }
        else 
        {
            SettingsManager.Instance.nextScene = "TutorialScene";
            SettingsManager.Instance.tutorialFinished = true;
        }
    }

    public void StartGame() 
    {
        SceneManager.LoadScene(SettingsManager.Instance.nextScene);
    }

    public void OptionsClicked() 
    {
        settingsOptions.SetVolume();
        menuCanvas.enabled = false;
        optionsCanvas.enabled = true;
    }

    public void BackToMenu() 
    {
        menuCanvas.enabled = true;
        optionsCanvas.enabled = false;
    }

    public void ExitGame() 
    {
        Application.Quit();
    }
}
