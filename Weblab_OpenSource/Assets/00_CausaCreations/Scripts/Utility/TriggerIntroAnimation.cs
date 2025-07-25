using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

public class TriggerIntroAnimation : MonoBehaviour
{
    public UnityEvent OnAnimationFinished = new UnityEvent();
    [SerializeField] private VideoPlayer _videoPlayer;

    private void Awake()
    {
        _videoPlayer.loopPointReached += TriggerOnAnimationFinished;
    }

    public void TriggerIntro()
    {
        if(_videoPlayer.clip == null) TriggerOnAnimationFinished(_videoPlayer);
        else
        {
            _videoPlayer.Play();
        }
    }

    public void TriggerOnAnimationFinished(VideoPlayer player)
    {
        OnAnimationFinished.Invoke();
    }
}
