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

    private void Start()
    {
        var AudioManagerInstance = AudioManager.instance;
        var LocaleSelectorInstance = LocaleSelector.instance;
        var SceneManagerScriptInstance = SceneManagerScript.instance;
        
        if (AudioManagerInstance != null)
        {
            soundtrackVolumeSlider.value = AudioManagerInstance.soundtrackVolume;
            soundEffectVolumeSlider.value = AudioManagerInstance.soundEffectVolume;
        }
        soundtrackVolumeSlider.onValueChanged.AddListener(AudioManagerInstance.ChangeSoundtrackVolume);
        soundEffectVolumeSlider.onValueChanged.AddListener(AudioManagerInstance.ChangeSoundEffectVolume);
        
        
        if (LocaleSelectorInstance != null)
        {
            languageDropdown.ClearOptions();
            List<string> options = new List<string>();
            foreach (var locale in LocalizationSettings.AvailableLocales.Locales)
            {
                options.Add(locale.LocaleName);
            }
            languageDropdown.AddOptions(options);

            // Setta il valore attuale (l'indice del SelectedLocale)
            int selectedIndex = LocalizationSettings.AvailableLocales.Locales.IndexOf(LocalizationSettings.SelectedLocale);
            languageDropdown.value = selectedIndex >= 0 ? selectedIndex : 0;
            languageDropdown.RefreshShownValue(); // importante aggiornare la visualizzazione
            languageDropdown.onValueChanged.AddListener(LocaleSelectorInstance.ChangeLocale);
        }

        if (SceneManagerScriptInstance != null)
        {
            resolutionDropdown.ClearOptions();
            resolutionDropdown.AddOptions(new List<string> { "1280x720", "Fullscreen" });
            // Controlla lo stato fullscreen
            if (SceneManagerScriptInstance.isFullScreen)
            {
                // Se fullscreen, seleziona l'opzione "Fullscreen" (indice 1)
                resolutionDropdown.value = 1;
            }
            else
            {
                // Altrimenti seleziona 1280x720 (indice 0)
                resolutionDropdown.value = 0;
            }
            resolutionDropdown.RefreshShownValue();
            resolutionDropdown.onValueChanged.AddListener(SceneManagerScriptInstance.ChangeResolution);
        }
    }
}
