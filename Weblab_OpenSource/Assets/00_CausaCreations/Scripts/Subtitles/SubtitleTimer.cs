using System;
using UnityEngine;
using UnityEngine.Events;

public abstract class SubtitleTimer : MonoBehaviour
{
    public UnityEvent OnPlay = new UnityEvent();
    public UnityEvent OnStop = new UnityEvent();
    
    public bool IsPlaying => _isPlaying;
    protected bool _isPlaying = false;

    public double Timer => _timer;
    protected double _timer = 0;

    protected abstract void UpdateTimer();

    private void Update()
    {
        UpdateTimer();
    }
}
