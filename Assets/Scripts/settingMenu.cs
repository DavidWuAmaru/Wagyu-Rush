using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class settingMenu : MonoBehaviour
{
    [SerializeField] private Slider SESlider, MusicSlider;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Canvas MsgBoxCanvas;
    public static int currentResolutionIndex = -1;
    private List<Resolution> resolutions;
    
    private void Start()
    {
        resolutions = new List<Resolution>();
        Resolution[] _resolutions = Screen.resolutions;
        for (int i = 0; i < _resolutions.Length; ++i)
            if (resolutions.Count == 0
                   || resolutions[resolutions.Count - 1].width != _resolutions[i].width
                   || resolutions[resolutions.Count - 1].height != _resolutions[i].height) resolutions.Add(_resolutions[i]);

        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        for (int i = 0; i < resolutions.Count; ++i) options.Add(resolutions[i].width + " x " + resolutions[i].height);
        options.Add("Full Screen");

        if(currentResolutionIndex < 0) for (int i = 0; i < resolutions.Count; ++i)
        {
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        if (currentResolutionIndex < 0) currentResolutionIndex = 0;

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        SetResolution(currentResolutionIndex);
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
        if (resolutionIndex == resolutions.Count)
        {
            Screen.fullScreen = true;
        }
        else
        {
            Resolution resolution = resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, false);
        }
        currentResolutionIndex = resolutionIndex;
    }

    
    public void ResetPlayerDataMsg()
    {
        MsgBoxCanvas.gameObject.SetActive(true);
    }

    public void ResetPlayerData()
    {
        PlayerData.Reset();
        PlayerData.Save();
    }
}
