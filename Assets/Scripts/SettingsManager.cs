using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    public Resolution currentResolution;
    public bool isFullScreen = true;
    public float commonVolume = 1.0f;
    public float effectsVolume = 1.0f;
    public float musicVolume = 1.0f;
    public bool isFirstLoad = true;
    public string nextScene = "TutorialScene";
    public bool tutorialFinished = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}