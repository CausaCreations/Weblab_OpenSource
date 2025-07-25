using System;
using System.Collections.Generic;
using AssetKits.ParticleImage;
using DG.Tweening;
using UnityEngine;

public class Objective_PointConnected : Objective
{
    [SerializeField] private TileData _point;
    [SerializeField] private GameObject _icon;
    [SerializeField] private GameObject _iconConnected;
    [SerializeField] private ParticleImage _particles;

    [SerializeField] private GameObject _containerCheckmark;
    [SerializeField] private GameObject _exclamationPoint;
    [SerializeField] private GameObject _checkmark;
    
    private TileConnections _tileConnections;
    private bool _connected = false;
    
    private void OnEnable()
    {
        _tileConnections = FindAnyObjectByType<TileConnections>();
        _tileConnections.OnNetworkChanged.AddListener(UpdateProgress);
        if(_icon) _icon.SetActive(true);
        if(_iconConnected) _iconConnected.SetActive(false);
        
        if(_exclamationPoint) _exclamationPoint.SetActive(true);
        if(_checkmark) _checkmark.SetActive(false);
    }

    private void OnDisable()
    {
        _tileConnections.OnNetworkChanged.RemoveListener(UpdateProgress);
    }

    protected override void UpdateProgress()
    {
        var connected = _tileConnections.IsCurrentlyConnected(_point);

        if (connected != _connected)
        {
            if (connected)
            {
                //_particles.transform.position = _icon.transform.position;
                //_particles.Play();
            }

            if(_icon) _icon.SetActive(!connected);
            if(_iconConnected) _iconConnected.SetActive(connected);
            
            if(_exclamationPoint) _exclamationPoint.SetActive(!connected);
            if(_checkmark) _checkmark.SetActive(connected);
            
            _connected = connected;
            OnProgressChanged.Invoke();
        }
    }

    public override int GetProgressPercentage()
    {
        return _connected ? 100 : 0;
    }

    public override void OnSetActive()
    {
        if(_containerCheckmark) _containerCheckmark.SetActive(true);
    }

    public override void OnSetInactive()
    {
        if(_containerCheckmark) _containerCheckmark.SetActive(false);
    }
}
