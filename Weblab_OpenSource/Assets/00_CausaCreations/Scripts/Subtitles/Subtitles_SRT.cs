using System;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;

public class Subtitles_SRT : Subtitles
{
    [SerializeField] private LocalizedAsset<TextAsset> _localizedSRT;
    [SerializeField] private TextMeshProUGUI _subtitleTMP;
    
    private double _nextEndTime = 0;
    private List<SubtitleBlock> _subtitles = new List<SubtitleBlock>();
    
    private void OnEnable()
    {
        _localizedSRT.AssetChanged += OnAssetChanged;
    }

    private void OnDisable()
    {
        _localizedSRT.AssetChanged -= OnAssetChanged;
        if(_isPlaying && _enableAndDisableTextGameObject) _subtitleTMP.gameObject.SetActive(false);
    }

    private void OnAssetChanged(TextAsset newAsset)
    {
        _isInitiated = true;
        
        Parse(newAsset);
        ForceUpdateText();
    }
    
    protected void Parse(TextAsset textAsset)
    {
        var lines = textAsset.text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        var currentState = srtReadState.Index; 
        
        _subtitles.Clear();
        _subtitles = new List<SubtitleBlock>();

        string currentText = "";
        double currentStart = 0;
        double currentEnd = 0;

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            
            switch (currentState)
            {
                case srtReadState.Index:

                    currentState = srtReadState.Time;
                    break;
                
                case srtReadState.Time:

                    line = line.Replace(',', '.');
                    var parts = line.Split(new[] { "-->" }, StringSplitOptions.RemoveEmptyEntries);

                    // Parse the timestamps
                    if (parts.Length == 2)
                    {
                        TimeSpan fromTime;
                        if (TimeSpan.TryParse(parts[0], out fromTime))
                        {
                            TimeSpan toTime;
                            if (TimeSpan.TryParse(parts[1], out toTime))
                            {
                                currentStart = fromTime.TotalSeconds;
                                currentEnd = toTime.TotalSeconds;
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
                    
                    currentState = srtReadState.Text;
                    break;
                
                case srtReadState.Text:
                    if (line == "" || i == lines.Length - 1)
                    {
                        var newSubtitleBlock = new SubtitleBlock();
                        newSubtitleBlock.Text = currentText;
                        newSubtitleBlock.Start = currentStart;
                        newSubtitleBlock.End = currentEnd;
                        _subtitles.Add(newSubtitleBlock);

                        currentText = "";
                        currentEnd = 0;
                        currentStart = 0;
                        
                        currentState = srtReadState.Index;
                    }
                    else
                    {
                        currentText += line;
                    }
                    break;
            }
        }
        
        
        _isInitiated = true;
    }

    protected override void Parse(){}
    
    protected override void UpdateText()
    {
        // if (_timer.Timer < _nextEndTime) return; // Removed so backwards scrubbing is possible
        
        var nextSubtitles = _subtitles.Find(x => _timer.Timer >= x.Start && _timer.Timer <= x.End);
        if (nextSubtitles != null)
        {
            _subtitleTMP.text = nextSubtitles.Text;
            _nextEndTime = nextSubtitles.End;
            if(_enableAndDisableTextGameObject) _subtitleTMP.gameObject.SetActive(true);
        }
        else
        {
            if(_enableAndDisableTextGameObject) _subtitleTMP.gameObject.SetActive(false);
        }

        if (_timer is SubtitleTimerStandalone)
        {
            if (_timer.Timer > _subtitles[^1].End) (_timer as SubtitleTimerStandalone)?.Stop();
        }
    }

    private void ForceUpdateText()
    {
        var nextSubtitles = _subtitles.Find(x => _timer.Timer >= x.Start && _timer.Timer <= x.End);
        if (nextSubtitles != null)
        {
            _subtitleTMP.text = nextSubtitles.Text;
            _nextEndTime = nextSubtitles.End;
            if(_enableAndDisableTextGameObject) _subtitleTMP.gameObject.SetActive(true);
        }
        else
        {
            if(_enableAndDisableTextGameObject) _subtitleTMP.gameObject.SetActive(false);
        }
    }

    protected override void OnPlaybackStarted()
    {
        _nextEndTime = 0;
        if(_enableAndDisableTextGameObject) _subtitleTMP.gameObject.SetActive(true);
        _isPlaying = true;
    }
    
    protected override void OnPlaybackStopped()
    {
        if(_enableAndDisableTextGameObject) _subtitleTMP.gameObject.SetActive(false);
        _isPlaying = false;
    }

    private class SubtitleBlock
    {
        public string Text;
        public double Start;
        public double End;
    }
    
    private enum srtReadState
    {
        Index,
        Time,
        Text
    }
}
