using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    Resolution[] resolutions;
    [SerializeField] private GameObject PrevScreen;
    [SerializeField] private GameObject OptionsSCreen;
    [SerializeField] private AudioMixer audioMixer;

    [SerializeField] private Dropdown ResolutionDropdown;
    [SerializeField] private Toggle ToggleFulscreen;
    [SerializeField] private Slider SliderMasterVolume;
    [SerializeField] private Slider SliderMusicVolume; 
    [SerializeField] private Slider SliderSFXVolume;

    // Create it when object gets created
    public enum AudioGroup { Master, Music, SFX };
    private Dictionary<AudioGroup, string> audioMixerGroups = new Dictionary<AudioGroup, string>
    {
        {AudioGroup.Master, "MasterVolume"},
        {AudioGroup.Music, "MusicVolume"},
        {AudioGroup.SFX, "SfxVolume" }
    };
    
    public static float ConvertFloatToDB(float value) => 20f * Mathf.Log10(value);
    public static float ConvertDBToFloat(float value) => Mathf.Pow(10, value / 20f);
    
    private void Awake()
    {
        audioMixerGroups = new Dictionary<AudioGroup, string>()
        {
            {AudioGroup.Master, "MasterVolume"},
            {AudioGroup.Music, "MusicVolume"},
            {AudioGroup.SFX, "SfxVolume" }
        };
    }
    
    private void Start()
    {
        OptionsSCreen.SetActive(false);
        
        //resolutions
        resolutions = Screen.resolutions;
        System.Array.Reverse(resolutions);
        ResolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height + " " + resolutions[i].refreshRate + "Hz";
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height &&
                resolutions[i].refreshRate == Screen.currentResolution.refreshRate)
            {
                currentResolutionIndex = i;
            }
        }

        ResolutionDropdown.AddOptions(options);
        ResolutionDropdown.value = currentResolutionIndex;
        ResolutionDropdown.RefreshShownValue();
        
        //audio
        float masterVolume = PlayerPrefs.GetFloat(audioMixerGroups[AudioGroup.Master], 1);
        float musicVolume = PlayerPrefs.GetFloat(audioMixerGroups[AudioGroup.Music], 1);
        float sfxVolume = PlayerPrefs.GetFloat(audioMixerGroups[AudioGroup.SFX], 1);
        int fullscreen = PlayerPrefs.GetInt("Fullscreen", 1);

        SetMasterVolume(masterVolume);
        SetMusicVolume(musicVolume);
        SetSfxVolume(sfxVolume);
        SetFullscreen(fullscreen != 0);
        GetComponent<MainMenu>().enabled = true;
    }
    
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        ToggleFulscreen.isOn = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen, resolution.refreshRate);
        ResolutionDropdown.Hide();
    }
    
    public string GetMixerGroupKey(AudioGroup audioGroup)
    {
        return audioMixerGroups[audioGroup];
    }

    public void SetMasterVolume(float volumeDB)
    {
        SetVolumeDB(AudioGroup.Master, volumeDB);
        SliderMasterVolume.value = volumeDB;
    }
    
    public void SetMusicVolume(float volumeDB)
    {
        SetVolumeDB(AudioGroup.Music, volumeDB);
        SliderMusicVolume.value = volumeDB;
    }
    
    public void SetSfxVolume(float volumeDB)
    {
        SetVolumeDB(AudioGroup.SFX, volumeDB);
        SliderSFXVolume.value = volumeDB;
    }
    
    private void SetVolumeDB(AudioGroup audioGroup, float volumeDB)
    {
        audioMixer.SetFloat(audioMixerGroups[audioGroup], Mathf.Log10(volumeDB) * 20);
        PlayerPrefs.SetFloat(audioMixerGroups[audioGroup], volumeDB);
    }
    
    public void BackButton()
    {
        PrevScreen.SetActive(true);
        OptionsSCreen.SetActive(false);
        Selectable selectable = PrevScreen.GetComponentInChildren<Selectable>();
        EventSystem.current.SetSelectedGameObject(selectable.gameObject);

    }
}
