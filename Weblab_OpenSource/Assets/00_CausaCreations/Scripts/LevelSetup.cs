using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;
using UnityEngine.Video;

public class LevelSetup : MonoBehaviour
{
    [SerializeField] private int _levelNr;
    [SerializeField] private CharacterDataAsset _characterDataAsset;

    [SerializeField] private VideoPlayer _introAnimator;
    [SerializeField] private List<Image> _characterImages = new List<Image>();
    [SerializeField] private List<SpriteRenderer> _characterSprites;
    [SerializeField] private bool _enableGridVisualOnStart;

    private void Start()
    {
        var dontDestroyOnLoadData = FindAnyObjectByType<DontDestroyOnLoadData>();
        string name = "";
        
        switch (_levelNr)
        {
            case 1:
                _introAnimator.clip =
                    _characterDataAsset.Data[PlayerPrefs.GetInt("CharacterIndex")].Level_01_Animation;

                name = _characterDataAsset.Data[PlayerPrefs.GetInt("CharacterIndex")].Level_01_Animation.name;
                _introAnimator.url = System.IO.Path.Combine (Application.streamingAssetsPath, name + ".mp4");  

                break;
            case 2:
                _introAnimator.clip =
                    _characterDataAsset.Data[PlayerPrefs.GetInt("CharacterIndex")].Level_02_Animation;

                name = _characterDataAsset.Data[PlayerPrefs.GetInt("CharacterIndex")].Level_02_Animation.name;
                _introAnimator.url = System.IO.Path.Combine (Application.streamingAssetsPath, name + ".mp4"); 
                
                break;
            case 3:
                _introAnimator.clip =
                    _characterDataAsset.Data[PlayerPrefs.GetInt("CharacterIndex")].Level_03_Animation;

                name = _characterDataAsset.Data[PlayerPrefs.GetInt("CharacterIndex")].Level_03_Animation.name;
                _introAnimator.url = System.IO.Path.Combine (Application.streamingAssetsPath, name + ".mp4"); 
                
                break;
        }

        foreach (var image in _characterImages)
        {
            image.sprite = _characterDataAsset.Data[PlayerPrefs.GetInt("CharacterIndex")].Image;
        }

        foreach (var sprite in _characterSprites)
        {
            sprite.sprite = _characterDataAsset.Data[PlayerPrefs.GetInt(("CharacterIndex"))].Image;
        }

        if (_enableGridVisualOnStart)
        {
            foreach (var gridTile in GetComponentsInChildren<GridTile>())
            {
                gridTile.ShowGrid();
            }
        }
    }
}
