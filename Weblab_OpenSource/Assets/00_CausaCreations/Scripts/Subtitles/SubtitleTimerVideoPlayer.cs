using UnityEngine;
using UnityEngine.Video;

public class SubtitleTimerVideoPlayer : SubtitleTimer
{
    [SerializeField] private VideoPlayer _videoPlayer;

    protected override void UpdateTimer()
    {
        if (_videoPlayer.isPlaying && !_isPlaying)
        {
            _isPlaying = true;
            OnPlay.Invoke();
        }

        _timer = _videoPlayer.time;

        if (!_videoPlayer.isPlaying && _isPlaying)
        {
            _isPlaying = false;
            OnStop.Invoke();
        }
    }
}
