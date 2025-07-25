using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class TileGrid : MonoBehaviour
{
    public UnityEvent<TileData> OnTileRegistered = new UnityEvent<TileData>();
    public UnityEvent OnAfterTileRegistered = new UnityEvent();
    public UnityEvent<TileData> OnTileUnregistered = new UnityEvent<TileData>();
    public UnityEvent OnAfterTileUnregistered = new UnityEvent();
    public UnityEvent FirstTimePowerTilePlaced = new UnityEvent();
    
    private Grid _grid;
    
    private Dictionary<Vector2Int, (TileData, TilePart)> _playTilePartsOnGrid = new Dictionary<Vector2Int, (TileData, TilePart)>();
    private List<TileData> _playTilesOnGridDistinct = new List<TileData>();

    private void Awake()
    {
        _grid = FindAnyObjectByType<Grid>();
    }

    public void RegisterTile(TileData tile)
    {
        foreach (var playTilePart in tile.gameObject.GetComponentsInChildren<TilePart>())
        {
            var position = _grid.WorldToCell(playTilePart.GetCenter());
            _playTilePartsOnGrid.Add(new Vector2Int(position.x, position.y), (tile, playTilePart));
        }
        
        _playTilesOnGridDistinct.Add(tile);

        OnTileRegistered.Invoke(tile);
        OnAfterTileRegistered.Invoke();

        if (tile.Type == TileData.TileType.GasPower || tile.Type == TileData.TileType.WindPower)
        {
            var firstTime = !PlayerPrefs.HasKey("PowerPlacementTile") ||
                            PlayerPrefs.GetInt("PowerPlacementTile") == 0;

            if (firstTime)
            {
                PlayerPrefs.SetInt("PowerPlacementTile", 1);
                FirstTimePowerTilePlaced.Invoke();
                FindObjectOfType<AudioManager>()._playTutorialPopUp.Invoke();
            }  
        }
    }
    
    public void UnregisterTile(TileData tile)
    {
        if (_playTilePartsOnGrid.Where(x => x.Value.Item1 == tile).ToArray().Length > 0)
        {
            var keyValuePairs = _playTilePartsOnGrid.Where(x => x.Value.Item1 == tile).ToArray();
            for (int i = 0; i < keyValuePairs.Count(); i++)
            {
                _playTilePartsOnGrid.Remove(keyValuePairs[i].Key);
            }

            _playTilesOnGridDistinct.Remove(tile);
            
            OnTileUnregistered.Invoke(tile);
            OnAfterTileUnregistered.Invoke();
        }
    }

    public KeyValuePair<Vector2Int, (TileData, TilePart)>[] GetTilePartsInGridOfTile(TileData tileData)
    {
        return _playTilePartsOnGrid.Where(x => x.Value.Item1 == tileData).ToArray();
    }

    public bool Exits(Vector2Int index)
    {
        return _playTilePartsOnGrid.ContainsKey(index);
    }

    public TileData GetTileAt(Vector2Int index)
    {
        return _playTilePartsOnGrid[index].Item1;
    }
    
    public TilePart GetTilePartAt(Vector2Int index)
    {
        return _playTilePartsOnGrid[index].Item2;
    }

    public Vector2Int GetIndexOf(TilePart tilePart)
    {
        return _playTilePartsOnGrid.First(x => x.Value.Item2 == tilePart).Key;
    }

    public List<TilePart> GetTilePartsAroundTilePartOfSameTile(TilePart tilePart, int radius)
    {
        var list = new List<TilePart>();
        var position = GetIndexOf(tilePart);
        for (int i = position.x - radius; i <= position.x + radius; i++)
        {
            for (int j = position.y - radius; j <= position.y + radius; j++)
            {
                if (j == position.y && i == position.x) continue;

                var checkPosition = new Vector2Int(i, j);
                if (Exits(checkPosition))
                {
                    if (GetTileAt(checkPosition) == tilePart.GetComponentInParent<TileData>())
                    {
                        list.Add(GetTilePartAt(checkPosition));
                    }
                }
            }
        }
        return list;
    }

    public TilePart GetNextClosestTilePart(TilePart current, TilePart goal, ref bool gap)
    {
        var currentDistance = Vector2Int.Distance(GetIndexOf(current), GetIndexOf(goal));
        
        var dictionaryDistance = new Dictionary<TilePart, float>();
        var sortedDict = new List<KeyValuePair<TilePart,float>>();

        int radius = 1;
        gap = false;

        while (sortedDict.Count <= 0 || currentDistance < sortedDict[0].Value)
        {
            if (radius > 1) gap = true;
            
            var tilePartsAroundTilePart = GetTilePartsAroundTilePartOfSameTile(current, radius);
            dictionaryDistance.Clear();
                    
            foreach (var tilePartAroundTilePart in tilePartsAroundTilePart)
            {
                dictionaryDistance.Add(tilePartAroundTilePart, Vector2Int.Distance(GetIndexOf(tilePartAroundTilePart), GetIndexOf(goal)));
            }
            sortedDict = (from entry in dictionaryDistance orderby entry.Value ascending select entry).ToList();
            radius++;
        }

        return sortedDict[0].Key;
    }
}
