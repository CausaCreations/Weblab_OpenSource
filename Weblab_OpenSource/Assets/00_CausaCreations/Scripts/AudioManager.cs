using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource _backgroundMusic;
    private float _backgroundMusicVolume;
    
    public UnityEvent _playUIButtonClick = new UnityEvent();
    
    public UnityEvent _playCharacterSpeechProgress = new UnityEvent();
    public UnityEvent _playCharacterSpeechEnd = new UnityEvent();
    public UnityEvent _playCharacterAppearOnMap = new UnityEvent();

    public UnityEvent _playCableSpawned = new UnityEvent();
    public UnityEvent _playBuildingSpawned = new UnityEvent();
    public UnityEvent _playPowerSpawned = new UnityEvent();

    public UnityEvent _playDropCableCorrectly = new UnityEvent();
    public UnityEvent _playDropBuildingCorrectly = new UnityEvent();
    public UnityEvent _playDropPowerCorrectly = new UnityEvent();
    public UnityEvent _playDropAnythingWrong = new UnityEvent();
    
    public UnityEvent _playPowerSufficient = new UnityEvent();
    public UnityEvent _playPowerNotSufficientAnymore = new UnityEvent();
    
    public UnityEvent _playLineRendererStarts = new UnityEvent();
    public UnityEvent _playLineRendererReachesBuilding = new UnityEvent();

    public UnityEvent _playOneStarReached = new UnityEvent();
    public UnityEvent _playTwoStarReached = new UnityEvent();
    public UnityEvent _playThreeStarReached = new UnityEvent();

    public UnityEvent _resourceThresholdExceeded = new UnityEvent();
    public UnityEvent _resourceThresholdRestored = new UnityEvent();

    public UnityEvent _playTutorialPopUp = new UnityEvent();

    private void Awake()
    {
        _backgroundMusicVolume = _backgroundMusic.volume;
    }


    public void FadeInBackgroundMusic()
    {
        _backgroundMusic.volume = 0;
        _backgroundMusic.Play();
        _backgroundMusic.DOFade(_backgroundMusicVolume, 0.5f).Play();
    }

    public void FadeOutBackgroundMusic()
    {
        _backgroundMusic.DOFade(0, 0.5f).Play().OnComplete(()=>
        {
            _backgroundMusic.Stop();
        });
    }
}
