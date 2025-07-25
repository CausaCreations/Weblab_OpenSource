using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

public class Subtitles_StringTable : Subtitles
{
    [SerializeField] private TableReference _stringTableReference;
    [SerializeField] private string _commonKey;
    [SerializeField] private LocalizeStringEvent _subtitleStringEvent;
    
    private double _nextEndTime = 0;
    private List<SubtitleBlock> _subtitles = new List<SubtitleBlock>();
    
    private void OnDisable()
    {
        if(_isPlaying && _enableAndDisableTextGameObject) _subtitleStringEvent.gameObject.SetActive(false);
    }

    protected override void Parse()
    {
        _subtitles = new List<SubtitleBlock>();
        var table = LocalizationSettings.StringDatabase.GetTable(_stringTableReference);
        foreach (var entry in table)
        {
            if (entry.Value.Key.Contains(_commonKey))
            {
                var subtitleBlock = new SubtitleBlock();
                subtitleBlock.Text = new LocalizedString();
                subtitleBlock.Text.SetReference(_stringTableReference, entry.Value.Key);
               
                string timeString = entry.Value.SharedEntry.Metadata.MetadataEntries[0].ToString();
                timeString = timeString.Replace(',', '.');
                var parts = timeString.Split(new[] { "-->" }, StringSplitOptions.RemoveEmptyEntries);

                // Parse the timestamps
                if (parts.Length == 2)
                {
                    TimeSpan fromTime;
                    if (TimeSpan.TryParse(parts[0], out fromTime))
                    {
                        TimeSpan toTime;
                        if (TimeSpan.TryParse(parts[1], out toTime))
                        {
                            subtitleBlock.Start = fromTime.TotalSeconds;
                            subtitleBlock.End = toTime.TotalSeconds;
                        }
                        else
                        {
                            Debug.LogError("Error while parsing timestamps. Please ensure that they are in the correct format. (e.g. 00:00:00,000 --> 00:00:02,500)");
                        }
                    }
                    else
                    {
                        Debug.LogError("Error while parsing timestamps. Please ensure that they are in the correct format. (e.g. 00:00:00,000 --> 00:00:02,500)");
                    }
                }
                
                _subtitles.Add(subtitleBlock);
            }
        }

        _subtitles.OrderBy(x => x.End);
        
        if(_enableAndDisableTextGameObject) _subtitleStringEvent.gameObject.SetActive(false);
        _isInitiated = true;
    }

    protected override void UpdateText()
    {
        //if (_timer.Timer < _nextEndTime) return; // Removed so backwards scrubbing is possible
        
        var nextSubtitles = _subtitles.Find(x => _timer.Timer >= x.Start && _timer.Timer <= x.End);
        if (nextSubtitles != null)
        {
            _subtitleStringEvent.StringReference = nextSubtitles.Text;
            _subtitleStringEvent.RefreshString();
            _nextEndTime = nextSubtitles.End;
            if(_enableAndDisableTextGameObject) _subtitleStringEvent.gameObject.SetActive(true);
        }
        else
        {
            if(_enableAndDisableTextGameObject) _subtitleStringEvent.gameObject.SetActive(false);
        }

        if (_timer is SubtitleTimerStandalone)
        {
            if (_timer.Timer > _subtitles[^1].End) (_timer as SubtitleTimerStandalone)?.Stop();
        }
    }

    protected override void OnPlaybackStarted()
    {
        _nextEndTime = 0;
        if(_enableAndDisableTextGameObject) _subtitleStringEvent.gameObject.SetActive(true);
        _isPlaying = true;
    }
    
    protected override void OnPlaybackStopped()
    {
        if(_enableAndDisableTextGameObject) _subtitleStringEvent.gameObject.SetActive(false);
        _isPlaying = false;
    }

    private class SubtitleBlock
    {
        public LocalizedString Text;
        public double Start;
        public double End;
    }
}
