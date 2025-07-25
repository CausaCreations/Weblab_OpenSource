using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class SetCharacterVideo : MonoBehaviour
{
    [SerializeField] private int _levelNumber;
    [SerializeField] private CharacterDataAsset _characterDataAsset;
    [SerializeField] private VideoPlayer _videoPlayer;

    public void Start()
    {
        string name = "";
        switch (_levelNumber)
        {
            case 1:
                _videoPlayer.clip =
                    _characterDataAsset.Data[PlayerPrefs.GetInt("CharacterIndex")].Level_01_Animation;
                name = _characterDataAsset.Data[PlayerPrefs.GetInt("CharacterIndex")].Level_01_Animation.name;
                _videoPlayer.url = System.IO.Path.Combine (Application.streamingAssetsPath, name + ".mp4");  
                break;
            case 2:
                _videoPlayer.clip =
                    _characterDataAsset.Data[PlayerPrefs.GetInt("CharacterIndex")].Level_02_Animation;
                name = _characterDataAsset.Data[PlayerPrefs.GetInt("CharacterIndex")].Level_02_Animation.name;
                _videoPlayer.url = System.IO.Path.Combine (Application.streamingAssetsPath, name + ".mp4"); 
                break;
            case 3:
                _videoPlayer.clip =
                    _characterDataAsset.Data[PlayerPrefs.GetInt("CharacterIndex")].Level_03_Animation;
                name = _characterDataAsset.Data[PlayerPrefs.GetInt("CharacterIndex")].Level_03_Animation.name;
                _videoPlayer.url = System.IO.Path.Combine (Application.streamingAssetsPath, name + ".mp4"); 
                break;
        }
    }
}
