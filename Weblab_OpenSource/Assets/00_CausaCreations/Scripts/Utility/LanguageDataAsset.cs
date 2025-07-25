using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "LanguageDataAsset", menuName = "InternetBuilder/LanguageDataAsset", order = 1)]
public class LanguageDataAsset : ScriptableObject
{
    public List<LanguageData> Data;
}

[Serializable]
public struct LanguageData
{
    public Locale Locale;
}
