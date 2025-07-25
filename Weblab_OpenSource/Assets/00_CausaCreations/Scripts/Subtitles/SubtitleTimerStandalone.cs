using UnityEngine;

public class SubtitleTimerStandalone : SubtitleTimer
{
    [Button(nameof(Play))] public bool play;
    [Button(nameof(Pause))] public bool pause;
    [Button(nameof(Resume))] public bool resume;
    [Button(nameof(Stop))] public bool stop;
    
    protected override void UpdateTimer()
    {
        if (_isPlaying)
        {
            _timer += Time.deltaTime;
        }
    }

    public void Play()
    {
        _isPlaying = true;
        _timer = 0;
        OnPlay.Invoke();
    }

    public void Pause()
    {
        _isPlaying = false;
        OnStop.Invoke();
    }

    public void Resume()
    {
        _isPlaying = true;
        OnPlay.Invoke();
    }

    public void Stop()
    {
        _isPlaying = false;
        _timer = 0;
        OnStop.Invoke();
    }
}
