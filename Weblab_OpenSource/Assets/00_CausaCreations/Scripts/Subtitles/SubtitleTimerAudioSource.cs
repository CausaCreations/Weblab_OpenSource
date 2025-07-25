using UnityEngine;

public class SubtitleTimerAudioSource : SubtitleTimer
{
    [SerializeField] private AudioSource _audioSource;

    protected override void UpdateTimer()
    {
        if (_audioSource.isPlaying && !_isPlaying)
        {
            _isPlaying = true;
            OnPlay.Invoke();
        }

        _timer = _audioSource.time;

        if (!_audioSource.isPlaying && _isPlaying)
        {
            _isPlaying = false;
            OnStop.Invoke();
        }
    }
}
