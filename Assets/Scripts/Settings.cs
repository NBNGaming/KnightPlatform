using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Dropdown resolutionDropdown;


    public Slider soundsSlider;
    public float soundsCurrentVolume;

    public Slider musicSlider;
    public float musicCurrentVolume;

    Resolution[] resolutions;

    void Start()
    {
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        resolutions = Screen.resolutions;
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height + " " + resolutions[i].refreshRate +
                            "Hz";
            options.Add(option);
            if (resolutions[i].width == Screen.currentResolution.width
                && resolutions[i].height == Screen.currentResolution.height)
                currentResolutionIndex = i;
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.RefreshShownValue();
        LoadSettings(currentResolutionIndex);
    }

    public void SetSounds(float volume)
    {
        audioMixer.SetFloat("Sounds", volume);
        soundsCurrentVolume = volume;
    }


    public void SetMusic(float volume)
    {
        audioMixer.SetFloat("Music", volume);
        musicCurrentVolume = volume;
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width,
            resolution.height, Screen.fullScreen);
    }


    public void SaveSettings()
    {
        PlayerPrefs.SetInt("ResolutionPreference", resolutionDropdown.value);

        PlayerPrefs.SetInt("FullscreenPreference", System.Convert.ToInt32(Screen.fullScreen));

        PlayerPrefs.SetFloat("SoundsPreference", soundsCurrentVolume);
        PlayerPrefs.SetFloat("VolumePreference", musicCurrentVolume);
    }

    public void LoadSettings(int currentResolutionIndex)
    {
        if (PlayerPrefs.HasKey("ResolutionPreference"))
            resolutionDropdown.value = PlayerPrefs.GetInt("ResolutionPreference");
        else
            resolutionDropdown.value = currentResolutionIndex;

        if (PlayerPrefs.HasKey("FullscreenPreference"))
            Screen.fullScreen = System.Convert.ToBoolean(PlayerPrefs.GetInt("FullscreenPreference"));
        else
            Screen.fullScreen = true;


        if (PlayerPrefs.HasKey("SoundsPreference"))
            soundsSlider.value = PlayerPrefs.GetFloat("SoundsPreference");
        else
            soundsSlider.value = PlayerPrefs.GetFloat("SoundsPreference");


        if (PlayerPrefs.HasKey("MusicPreference"))
            musicSlider.value = PlayerPrefs.GetFloat("MusicPreference");
        else
            musicSlider.value = PlayerPrefs.GetFloat("MusicPreference");
    }
}