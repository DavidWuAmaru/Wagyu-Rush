using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class settingMenu : MonoBehaviour
{
    [SerializeField] private Slider SESlider, MusicSlider;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    Resolution[] resolutions;

    private void Start()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex = 1;
        for (int i = 0;i < resolutions.Length; ++i)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if(resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        options.Add("Full Screen");

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }
    private void Awake()
    {
        SESlider.value = FindObjectOfType<AudioManager>().getSEAmplifier();
        MusicSlider.value = FindObjectOfType<AudioManager>().getMusicAmplifier();
    }

    public void updateSEAmplifier(float val)
    {
        FindObjectOfType<AudioManager>().updateSEVolume(val);
    }
    public void updateMusicAmplifier(float val)
    {
        FindObjectOfType<AudioManager>().updateMusicVolume(val);
    }

    public void SetResolution(int resolutionIndex)
    {
        if(resolutionIndex + 1 == resolutions.Length)
        {
            Screen.fullScreen = false;
        }
        else
        {
            Screen.fullScreen = true;
            Resolution resolution = resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }
    }
}
