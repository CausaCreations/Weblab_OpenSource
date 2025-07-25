using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class TileData : MonoBehaviour
{
    public TileType Type;
    public TileConnectionType ConnectionType;
    public Sprite Icon;
    public List<SpriteRenderer> Visuals;

    public LineRenderer lineRenderer;

    public UnityEvent OnConnectedToNetwork = new UnityEvent();
    public UnityEvent OnDisconnectedFromNetwork = new UnityEvent();
    public UnityEvent OnIsInMiddleOfNetwork = new UnityEvent();

    [Header("Settings")] 
    public int AmountAvailableInLevel;
    public int CO2Emission;
    public int Power;
    public List<ResourceUsage> ResourceUse;
    public bool IsNetworkPoint;

    [Header("Tile Color")] 
    public bool SetTileColor;
    public Color TileColor;

    [Header("Bounds")]
    public int TileWidth;
    public int TileHeight;

    private void Start()
    {
        if (SetTileColor)
        {
            foreach (var visual in Visuals)
            {
                visual.color = TileColor;

                if (lineRenderer)
                {
                    lineRenderer.startColor = TileColor;
                    lineRenderer.endColor = TileColor;
                }
            }
        }
        
        OnDisconnectedFromNetwork.Invoke();
    }

    public void OnPlaced(int yPosition)
    {
        foreach (var visual in Visuals)
        {
            visual.sortingOrder = -yPosition + Visuals.IndexOf(visual);
            
            if (lineRenderer)
            {
                lineRenderer.sortingOrder = -yPosition;
            }
        }
    }

    public enum TileType
    {
        None,
        GasPower,
        WindPower,
        CableVerticalTopLeft,
        CableVerticalTopRight,
        CableVerticalBottomLeft,
        CableVerticalBottomRight,
        CableHorizontalTopLeft,
        CableHorizontalTopRight,
        CableHorizontalBottomLeft,
        CableHorizontalBottomRight,
        CableUndersea,
        CableWireless,
        InternetExchangePoint,
        DataCenter,
        NetworkStartingPoint,
        NetworkEndPoint,
        OptionalNetworkEndPoint,
        Router,
    }

    public enum TileConnectionType
    {
        NoConnection,
        HalfConnection,
        FullConnection,
    }

    public enum Resources
    {
        Resource1,
        Resource2,
        Resource3,
    }
    
    [Serializable]
    public struct ResourceUsage
    {
        public Resources Resource;
        public int Amount;
    }

    public void ConnectSound()
    {
        FindObjectOfType<AudioManager>()._playLineRendererReachesBuilding.Invoke();
    }
}
