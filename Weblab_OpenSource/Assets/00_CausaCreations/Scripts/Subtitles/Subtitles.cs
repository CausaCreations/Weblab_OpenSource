using System;
using UnityEngine;

public abstract class Subtitles : MonoBehaviour
{
    [SerializeField] protected SubtitleTimer _timer;
    [SerializeField] protected bool _enableAndDisableTextGameObject;

    protected abstract void Parse();
    protected abstract void UpdateText();

    protected abstract void OnPlaybackStarted();

    protected abstract void OnPlaybackStopped();

    protected bool _isInitiated = false;
    protected bool _isPlaying = false;

    private void Awake()
    {
        Parse();
    }

    private void OnEnable()
    {
        _timer.OnPlay.AddListener(OnPlaybackStarted);
        _timer.OnStop.AddListener(OnPlaybackStopped);
    }

    private void OnDisable()
    {
        _timer.OnPlay.RemoveListener(OnPlaybackStarted);
        _timer.OnStop.RemoveListener(OnPlaybackStopped);
    }

    private void Update()
    {
        if (_timer.IsPlaying && _isInitiated)
        {
            UpdateText();
        }
    }
}
