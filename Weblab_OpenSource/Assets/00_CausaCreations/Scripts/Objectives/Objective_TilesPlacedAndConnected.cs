using System;
using AssetKits.ParticleImage;
using DG.Tweening;
using UnityEngine;

public class Objective_TilesPlacedAndConnected : Objective
{
    [SerializeField] private TileData.TileType _tileType;
    [SerializeField] private TileGridContinent _continent;
    [SerializeField] private GameObject _iconToPlace;
    [SerializeField] private GameObject _iconToConnect;
    [SerializeField] private GameObject _iconConnected;
    [SerializeField] private bool _ignoreConnectedInProgressPercentage = false;

    [SerializeField] private ParticleImage _particles;

    private bool _placed = false;
    private bool _connected = false;

    private int _progress = 0;

    private TileConnections _tileConnections;
    private TileGrid _tileGrid;

    private void OnEnable()
    {
        _tileConnections = FindAnyObjectByType<TileConnections>();
        _tileGrid = FindAnyObjectByType<TileGrid>();
        _tileGrid.OnAfterTileRegistered.AddListener(UpdateProgress);
        _tileGrid.OnAfterTileUnregistered.AddListener(UpdateProgress);
        
        if(_iconToPlace) _iconToPlace.SetActive(true);
        if(_iconToConnect) _iconToConnect.SetActive(false);
        if(_iconConnected) _iconConnected.SetActive(false);
    }

    private void OnDisable()
    {
        _tileGrid.OnAfterTileRegistered.RemoveListener(UpdateProgress);
        _tileGrid.OnAfterTileUnregistered.RemoveListener(UpdateProgress);
    }

    protected override void UpdateProgress()
    {
        var tilesOfThisTypeOnContinent = _continent.GetTilesOfTypeOnContinent(_tileType);
        
        var placed = false;
        var connected = false;
        var progress = 0;
        
        if (tilesOfThisTypeOnContinent.Count > 0)
        {
            placed = true;
            connected = false;
            progress = 50;

            if (_ignoreConnectedInProgressPercentage) progress = 100;

            foreach (var tile in tilesOfThisTypeOnContinent)
            {
                if (_tileConnections.IsCurrentlyConnected(tile))
                {
                    connected = true;
                    progress = 100;
                }
            }
        }

        if (placed != _placed || connected != _connected || progress != _progress)
        {
            if (progress > _progress)
            {
                //_particles.transform.position = _iconToPlace.transform.position;
                //_particles.Play();
            }

            if(_iconToPlace) _iconToPlace.SetActive(!placed);
            if(_iconToConnect) _iconToConnect.SetActive(placed && !connected);
            if(_iconConnected) _iconConnected.SetActive(placed && connected);

            _placed = placed;
            _connected = connected;
            _progress = progress;
            
            OnProgressChanged.Invoke();
        }
    }

    public override int GetProgressPercentage()
    {
        return _progress;
    }

    public override void OnSetActive()
    {
    }

    public override void OnSetInactive()
    {
    }
}
