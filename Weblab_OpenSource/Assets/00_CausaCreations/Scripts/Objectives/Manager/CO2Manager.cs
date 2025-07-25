using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CO2Manager : MonoBehaviour
{
    private TileGrid _tileGrid;
    
    [SerializeField] private List<Image> _fillImages;
    [SerializeField] private float _fillAnimationDuration;
    [SerializeField] private Ease _fillAnimationEase;
    
    private int _co2InUse;
    private int _maxCo2InUse;

    private void Awake()
    {
        _tileGrid = FindAnyObjectByType<TileGrid>();
        var tileSpawners = FindObjectsByType<TileSpawner>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var tileSpawner in tileSpawners)
        {
            var tile = tileSpawner.Prefab.GetComponent<TileData>();
            _maxCo2InUse += tile.CO2Emission * tile.AmountAvailableInLevel;
        }
        
        float normalizedValue = Mathf.InverseLerp(0, _maxCo2InUse, _co2InUse);

        foreach (var fillImage in _fillImages)
        {
            fillImage.DOFillAmount(normalizedValue, _fillAnimationDuration).SetEase(_fillAnimationEase).Play();
        }
    }

    private void OnEnable()
    {
        _tileGrid.OnTileRegistered.AddListener(OnTileRegistered);
        _tileGrid.OnTileUnregistered.AddListener(OnTileUnregistered);
    }

    private void OnDisable()
    {
        _tileGrid.OnTileRegistered.RemoveListener(OnTileRegistered);
        _tileGrid.OnTileUnregistered.RemoveListener(OnTileUnregistered);
    }

    private void OnTileRegistered(TileData tile)
    {
        _co2InUse += tile.CO2Emission;
        
        float normalizedValue = Mathf.InverseLerp(0, _maxCo2InUse, _co2InUse);
        foreach (var fillImage in _fillImages)
        {
            fillImage.DOFillAmount(normalizedValue, _fillAnimationDuration).SetEase(_fillAnimationEase).Play();
        }
    }

    private void OnTileUnregistered(TileData tile)
    {
        _co2InUse -= tile.CO2Emission;
        
        float normalizedValue = Mathf.InverseLerp(0, _maxCo2InUse, _co2InUse);
        foreach (var fillImage in _fillImages)
        {
            fillImage.DOFillAmount(normalizedValue, _fillAnimationDuration).SetEase(_fillAnimationEase).Play();
        }
    }
}
