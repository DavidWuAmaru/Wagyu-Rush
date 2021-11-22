using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class settingMenu : MonoBehaviour
{
    [SerializeField] private Slider SESlider, MusicSlider;
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
}
