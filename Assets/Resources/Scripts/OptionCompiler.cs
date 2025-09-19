using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class OptionCompiler : MonoBehaviour
{
    public TMP_Dropdown languageDropdown;
    public Slider soundtrackVolumeSlider;
    public Slider soundEffectVolumeSlider;
    public TMP_Dropdown resolutionDropdown;

    public void CaricaOpzioni()
    {
        var AudioManagerInstance = AudioManager.instance;
        var LocaleSelectorInstance = LocaleSelector.instance;
        var SceneManagerScriptInstance = SceneManagerScript.instance;
        
        //Volumi 
        float savedSoundtrackVolume = PlayerPrefs.GetFloat("SoundtrackVolume", 0.5f);
        float savedSfxVolume = PlayerPrefs.GetFloat("SoundEffectVolume", 0.5f);
        
        if (AudioManagerInstance != null)
        {
            soundtrackVolumeSlider.value = savedSoundtrackVolume;
            soundEffectVolumeSlider.value = savedSfxVolume;
            AudioManagerInstance.ChangeSoundtrackVolume(savedSoundtrackVolume);
            AudioManagerInstance.ChangeSoundEffectVolume(savedSfxVolume);
        }
        soundtrackVolumeSlider.onValueChanged.AddListener(AudioManagerInstance.ChangeSoundtrackVolume);
        soundEffectVolumeSlider.onValueChanged.AddListener(AudioManagerInstance.ChangeSoundEffectVolume);
        
        
        // Mingue
        if (LocaleSelectorInstance != null)
        {
            languageDropdown.ClearOptions();
            List<string> options = new List<string>();
            foreach (var locale in LocalizationSettings.AvailableLocales.Locales)
            {
                options.Add(locale.LocaleName);
            }
            languageDropdown.AddOptions(options);

            int savedLangIndex = PlayerPrefs.GetInt("LocaleID", 0);
            savedLangIndex = Mathf.Clamp(savedLangIndex, 0, options.Count - 1);
            languageDropdown.value = savedLangIndex;
            languageDropdown.RefreshShownValue();
            // Setta il valore attuale (l'indice del SelectedLocale)
            //int selectedIndex = LocalizationSettings.AvailableLocales.Locales.IndexOf(LocalizationSettings.SelectedLocale);
            //languageDropdown.value = selectedIndex >= 0 ? selectedIndex : 0;
            //languageDropdown.RefreshShownValue(); // importante aggiornare la visualizzazione
            languageDropdown.onValueChanged.AddListener(LocaleSelectorInstance.ChangeLocale);
        }

        
        // Risoluzione
        if (SceneManagerScriptInstance != null)
        {
            resolutionDropdown.ClearOptions();
            resolutionDropdown.AddOptions(new List<string> { "1280x720", "Fullscreen" });
            int savedResIndex = PlayerPrefs.GetInt("RisoluzioneIndex", 0);
            savedResIndex = Mathf.Clamp(savedResIndex, 0, 1);
            resolutionDropdown.value = savedResIndex;
            resolutionDropdown.RefreshShownValue();
            resolutionDropdown.onValueChanged.AddListener(SceneManagerScriptInstance.ChangeResolution);
            SceneManagerScriptInstance.ChangeResolution(savedResIndex);
        }
    }

    public void Awake()
    {
        CaricaOpzioni();
    }

    public void Start()
    {
        CaricaOpzioni();
    }
}
