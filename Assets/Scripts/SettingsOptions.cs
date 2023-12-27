using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;

public class SettingsOptions : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullScreenMode;
    [SerializeField] private TMP_Text toggleStatusText;

    [Header("Sounds")]
    [SerializeField] private AudioClip music;
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private Slider commonVolumeSlider;
    [SerializeField] private Slider effectsVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;

    private Resolution[] resolutions;
    private List<Resolution> filteredResolutions;
    private int currentResolutionIndex = 0;

    private void Awake()
    {
        if (SettingsManager.Instance == null) 
        {
            GameObject settingsManagerObject = new GameObject("SettingsManager");
            SettingsManager.Instance = settingsManagerObject.AddComponent<SettingsManager>();
        }
    }

    private void Start()
    {
        fullScreenMode.isOn = SettingsManager.Instance.isFullScreen;

        // SetVolume();

        ChangeScreenResolution();

        FillDropdown();

        ChangeOptions();
    }

    public void SetVolume() 
    {
        commonVolumeSlider.value = SettingsManager.Instance.commonVolume;
        effectsVolumeSlider.value = SettingsManager.Instance.effectsVolume;
        musicVolumeSlider.value = SettingsManager.Instance.musicVolume;
    }

    private void ChangeScreenResolution()
    {
        if (SettingsManager.Instance.isFirstLoad)
        {
            SettingsManager.Instance.isFirstLoad = false;

            Screen.SetResolution(1920, 1080, true);
            SettingsManager.Instance.currentResolution = Screen.currentResolution;
            SettingsManager.Instance.isFullScreen = true;
        }
        else
        {
            Screen.SetResolution(SettingsManager.Instance.currentResolution.width, SettingsManager.Instance.currentResolution.height, SettingsManager.Instance.isFullScreen);
        }
    }

    private void FillDropdown() 
    {
        resolutions = Screen.resolutions;
        filteredResolutions = new List<Resolution>();
        resolutionDropdown.ClearOptions();

        for (int i = resolutions.Length - 1; i >= 0; i--)
        {
            if (i == resolutions.Length - 1)
            {
                filteredResolutions.Add(resolutions[i]);
            }
            else
            {
                bool isAdded = false;
                for (int j = 0; j < filteredResolutions.Count; j++)
                {
                    if (filteredResolutions[j].width == resolutions[i].width && filteredResolutions[j].height == resolutions[i].height)
                    {
                        isAdded = true;
                        break;
                    }
                }
                if (!isAdded)
                {
                    filteredResolutions.Add(resolutions[i]);
                }
            }
        }

        List<string> options = new List<string>();
        for (int i = 0; i < filteredResolutions.Count; i++)
        {
            string resolutionOption = filteredResolutions[i].width + "x" + filteredResolutions[i].height;
            options.Add(resolutionOption);
            if (filteredResolutions[i].width == Screen.width && filteredResolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    private void ChangeOptions() 
    {
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);

        fullScreenMode.onValueChanged.AddListener(OnFullScreenChanged);

        UpdateToggleStatusText();
    }

    private void OnResolutionChanged(int value)
    {
        currentResolutionIndex = value;
    }

    private void OnFullScreenChanged(bool value)
    {
        SettingsManager.Instance.isFullScreen = value;
        UpdateToggleStatusText();
    }

    private void UpdateToggleStatusText()
    {
        toggleStatusText.text = SettingsManager.Instance.isFullScreen ? "включено" : "выключено";
    }

    public void SaveSettings() 
    {
        SettingsManager.Instance.currentResolution = filteredResolutions[currentResolutionIndex];
        SettingsManager.Instance.commonVolume = commonVolumeSlider.value;
        SettingsManager.Instance.musicVolume = musicVolumeSlider.value;
        SettingsManager.Instance.effectsVolume = effectsVolumeSlider.value;

        Screen.SetResolution(SettingsManager.Instance.currentResolution.width, SettingsManager.Instance.currentResolution.height, SettingsManager.Instance.isFullScreen);
    }
}
