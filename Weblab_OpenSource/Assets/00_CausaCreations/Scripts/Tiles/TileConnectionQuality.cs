using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TileConnectionQuality : MonoBehaviour
{
    [SerializeField] private GameObject _iconNoConnection;
    [SerializeField] private GameObject _iconPoorConnection;
    [SerializeField] private GameObject _iconGoodConnection;
    
    public UnityEvent<ConnectionQuality> OnStatusChanged = new UnityEvent<ConnectionQuality>();
    public UnityEvent OnFirstTimeConnectedWithGoodQuality = new UnityEvent();
    
    private TileConnections _tileConnections;
    private ConnectionQuality _currentConnectionQuality = ConnectionQuality.None;
    private bool _firstTimeGoodQualityTriggered = false;

    public enum ConnectionQuality
    {
        None,
        Poor,
        Good,
    }

    private void Awake()
    {
        _tileConnections = FindObjectOfType<TileConnections>();
        _currentConnectionQuality = ConnectionQuality.None;
        OnTileDisconnectedFromNetwork();

    }

    public void OnTileConnectedToNetwork(TileConnections.Node node)
    {
        var newConnectionQuality = ConnectionQuality.None;
        
        //check if path contains wireless cables
        bool containsWireless = false;
        TileConnections.Node currentNode = node;
        while (currentNode.Parent != null)
        {
            if (currentNode.Parent.Tile.Type == TileData.TileType.CableWireless) containsWireless = true;
            
            currentNode = currentNode.Parent;
        }

        if (containsWireless)
        {
            newConnectionQuality = ConnectionQuality.Poor;
            _iconNoConnection.gameObject.SetActive(false);
            _iconPoorConnection.gameObject.SetActive(true);
            _iconGoodConnection.gameObject.SetActive(false);
        }
        else
        {
            newConnectionQuality = ConnectionQuality.Good;
            _iconNoConnection.gameObject.SetActive(false);
            _iconPoorConnection.gameObject.SetActive(false);
            _iconGoodConnection.gameObject.SetActive(true);

            if (!_firstTimeGoodQualityTriggered)
            {
                _firstTimeGoodQualityTriggered = true;
                OnFirstTimeConnectedWithGoodQuality.Invoke();
            }
        }

        if (_currentConnectionQuality != newConnectionQuality)
        {
            _currentConnectionQuality = newConnectionQuality;
            OnStatusChanged.Invoke(_currentConnectionQuality);
        }
    }

    public void OnTileDisconnectedFromNetwork()
    {
        if (_currentConnectionQuality != ConnectionQuality.None)
        {
            _currentConnectionQuality = ConnectionQuality.None;
            OnStatusChanged.Invoke(_currentConnectionQuality);
        }
        
        _iconNoConnection.gameObject.SetActive(true);
        _iconGoodConnection.gameObject.SetActive(false);
        _iconPoorConnection.gameObject.SetActive(false);
    }

    public ConnectionQuality GetCurrentConnectionQuality()
    {
        return _currentConnectionQuality;
    }
}
