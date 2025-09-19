using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Localization.Settings;

public class LocaleSelector : MonoBehaviour
{
    public static LocaleSelector instance;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        DontDestroyOnLoad(gameObject);
    }


    private bool active = false;
    
    IEnumerator SetLocale(int _localeID)
    {
        active = true;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_localeID];
        PlayerPrefs.SetInt("LocaleID", _localeID);
        PlayerPrefs.Save();
        active = false;
    }

    public void ChangeLocale(int _localeID)
    {
        if (active)
            return;
        StartCoroutine(SetLocale(_localeID));
    }
}