using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.SmartFormat.Extensions;
using UnityEngine.UI;

public class CharacterSelection : MonoBehaviour
{
    [SerializeField] private CharacterDataAsset _characterDataAsset;
    [SerializeField] private SceneLoader _sceneLoader;

    [SerializeField] private Image _image;
    [SerializeField] private LocalizeStringEvent _name;
    [SerializeField] private LocalizeStringEvent _lastName;
    [SerializeField] private LocalizeStringEvent _address;
    [SerializeField] private LocalizeStringEvent _description;
    

    private int _currentIndex;

    void Start()
    {
        _currentIndex = 0;
        SetInformation(_currentIndex);
    }

    public void Left()
    {
        var index = _currentIndex - 1;
        if (index < 0) index = _characterDataAsset.Data.Count - 1;
        _currentIndex = index;
        
        SetInformation(_currentIndex);
    }

    public void Right()
    {
        var index = _currentIndex + 1;
        if (index >= _characterDataAsset.Data.Count) index = 0;
        _currentIndex = index;
        
        SetInformation(_currentIndex);
    }

    public void Submit()
    {
        var dontDestroyData = FindObjectOfType<DontDestroyOnLoadData>();
        dontDestroyData.CharacterIndex = _currentIndex;
        
        PlayerPrefs.SetInt("CharacterIndex", _currentIndex);

        var source = LocalizationSettings.StringDatabase.SmartFormatter.GetSourceExtension<PersistentVariablesSource>();
        var fnameVariable = source["global"]["fname"] as LocalizedString;
        fnameVariable.TableReference = _characterDataAsset.Data[_currentIndex].FirstName.TableReference;
        fnameVariable.TableEntryReference = _characterDataAsset.Data[_currentIndex].FirstName.TableEntryReference;
        
        var lnameVariable = source["global"]["lname"] as LocalizedString;
        lnameVariable.TableReference = _characterDataAsset.Data[_currentIndex].LastName.TableReference;
        lnameVariable.TableEntryReference = _characterDataAsset.Data[_currentIndex].LastName.TableEntryReference;

        _sceneLoader.LoadScene("Level_01");
    }

    private void SetInformation(int index)
    {
        var character = _characterDataAsset.Data[index];

        _image.sprite = character.ImageCharacterSelection;
        _name.StringReference = character.FirstName;
        _name.RefreshString();
        _lastName.StringReference = character.LastName;
        _lastName.RefreshString();
        _address.StringReference = character.Place;
        _address.RefreshString();
        _description.StringReference = character.Description;
        _description.RefreshString();
        
    }

}
