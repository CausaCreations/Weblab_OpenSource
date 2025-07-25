using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class TileGridContinent : MonoBehaviour
{
    public UnityEvent<TileData> OnTileRegistered = new UnityEvent<TileData>();
    public UnityEvent<TileData> OnTileUnregistered = new UnityEvent<TileData>();

    [SerializeField] private Transform _gridTileContainer;
    
    private TileGrid _tileGrid;
    private Grid _grid;
    
    private List<Vector2Int> _tileIndexesOfContinent = new List<Vector2Int>();
    private List<TileData> _tilesOnContinent = new List<TileData>();
    
    
    private void Awake()
    {
        _grid = FindAnyObjectByType<Grid>();
        _tileGrid = FindAnyObjectByType<TileGrid>();
        
        foreach (var gridTile in _gridTileContainer.GetComponentsInChildren<GridTile>())
        {
            var position = gridTile.GetCenter();
            var positionInGrid = _grid.WorldToCell(position);
            _tileIndexesOfContinent.Add(new Vector2Int(positionInGrid.x, positionInGrid.y));
        }
    }
    
    private void OnEnable()
    {
        _tileGrid.OnTileRegistered.AddListener(RegisterTileOnContinent);
        _tileGrid.OnTileUnregistered.AddListener(UnRegisterTileOnContinent);
    }

    private void OnDisable()
    {
        _tileGrid.OnTileRegistered.RemoveListener(RegisterTileOnContinent);
        _tileGrid.OnTileUnregistered.RemoveListener(UnRegisterTileOnContinent);
    }
    
    public void RegisterTileOnContinent(TileData tile)
    {
        if (!_tilesOnContinent.Contains(tile))
        {
            bool isOnContinent = false;
            foreach (var tilePart in tile.GetComponentsInChildren<TilePart>())
            {
                var position = tilePart.GetCenter();
                var positionInGrid = _grid.WorldToCell(position);
                if (_tileIndexesOfContinent.Contains(new Vector2Int(positionInGrid.x, positionInGrid.y)))
                {
                    isOnContinent = true;
                }
            }

            if (isOnContinent)
            {
                _tilesOnContinent.Add(tile);
                OnTileRegistered.Invoke(tile);

                //DebugPrintAllTilesOnContinent();
            }
        }
    }
    
    public void UnRegisterTileOnContinent(TileData tile)
    {
        if (_tilesOnContinent.Contains(tile))
        {
            _tilesOnContinent.Remove(tile);
            OnTileUnregistered.Invoke(tile);
            
            //DebugPrintAllTilesOnContinent();
        }
    }

    public void DebugPrintAllTilesOnContinent()
    {
        var output = gameObject.name;
        foreach (var tileOnContinent in _tilesOnContinent)
        {
            output += "\n" + tileOnContinent.gameObject.name;
        }
        
        Debug.Log(output);
    }

    public List<TileData> GetTilesOfTypeOnContinent(TileData.TileType tileType)
    {
        return _tilesOnContinent.Where(x => x.Type == tileType).ToList();
    }
}
