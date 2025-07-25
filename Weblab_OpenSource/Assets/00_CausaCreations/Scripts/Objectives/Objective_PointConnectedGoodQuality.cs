using System;
using System.Collections;
using System.Collections.Generic;
using AssetKits.ParticleImage;
using UnityEngine;

public class Objective_PointConnectedGoodQuality : Objective
{
    [SerializeField] private TileConnectionQuality _connectionQuality;
    [SerializeField] private GameObject _icon;
    [SerializeField] private GameObject _iconDone;
    [SerializeField] private ParticleImage _particles;

    private TileConnectionQuality.ConnectionQuality _currentQuality;
    
    private void OnEnable()
    {
        _connectionQuality.OnStatusChanged.AddListener(UpdateProgress);
    }

    private void OnDisable()
    {
        _connectionQuality.OnStatusChanged.RemoveListener(UpdateProgress);
    }

    protected override void UpdateProgress() { }
    private void UpdateProgress(TileConnectionQuality.ConnectionQuality quality)
    {
        if (quality != _currentQuality)
        {
            OnProgressChanged.Invoke();
        }

        _currentQuality = quality;
        _icon.SetActive(quality != TileConnectionQuality.ConnectionQuality.Good);
        _iconDone.SetActive(quality == TileConnectionQuality.ConnectionQuality.Good);
    }


    public override int GetProgressPercentage()
    {
        return _currentQuality == TileConnectionQuality.ConnectionQuality.Good ? 100 : 0;
    }

    public override void OnSetActive()
    {
    }

    public override void OnSetInactive()
    {
    }
}
