using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class LanguagePicker : MonoBehaviour
{
    public List<Locale> Data;

    public void OnButtonPressed()
    {
        var currentLocaleIndex = Data.FindIndex(x => x.Identifier == LocalizationSettings.SelectedLocale.Identifier);
        
        var nextIndex = (currentLocaleIndex + 1);
        if (nextIndex >= Data.Count) nextIndex = 0;

        LocalizationSettings.SelectedLocale = Data[nextIndex];
    }
}
