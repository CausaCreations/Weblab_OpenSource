using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PowerManager : MonoBehaviour
{
    public UnityEvent<int> OnPowerPercentageChanged = new UnityEvent<int>();
    
    [SerializeField] private TileGridContinent _tileGrid;
    [SerializeField] private Image _fillImage;
    [SerializeField] private float _fillAnimationDuration;
    [SerializeField] private Ease _fillAnimationEase;

    private int _powerNeeded;
    private int _powerSupplied;
    
    public int PowerPercentage => _continentPowerPercentage;
    private int _continentPowerPercentage = 0;
    private int _previousPowerPercentage = 0;

    private void Awake()
    {
        _fillImage.DOFillAmount(0, _fillAnimationDuration).SetEase(_fillAnimationEase).Play();
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
        if (tile.Type == TileData.TileType.GasPower || tile.Type == TileData.TileType.WindPower)
        {
            _powerSupplied += tile.Power;
        }
        else
        {
            _powerNeeded += tile.Power;
        }
        float normalizedValue = Mathf.InverseLerp(0, _powerNeeded, _powerSupplied);
        if (_powerNeeded == 0 && _powerSupplied > 0) normalizedValue = 1;
        _previousPowerPercentage = _continentPowerPercentage;
        _continentPowerPercentage = (int) (normalizedValue * 100);

        if (_continentPowerPercentage == 100 && _previousPowerPercentage != 100)
        {
            FindObjectOfType<AudioManager>()._playPowerSufficient.Invoke();
        }

        if (_continentPowerPercentage != 100 && _previousPowerPercentage == 100)
        {
            FindObjectOfType<AudioManager>()._playPowerNotSufficientAnymore.Invoke();
        }

        _fillImage.DOFillAmount(normalizedValue, _fillAnimationDuration).SetEase(_fillAnimationEase).Play();
        OnPowerPercentageChanged.Invoke(PowerPercentage);
    }

    private void OnTileUnregistered(TileData tile)
    {
        if (tile.Type == TileData.TileType.GasPower || tile.Type == TileData.TileType.WindPower)
        {
            _powerSupplied -= tile.Power;
        }
        else
        {
            _powerNeeded -= tile.Power;
        }
        
        float normalizedValue = Mathf.InverseLerp(0, _powerNeeded, _powerSupplied);
        if (_powerNeeded == 0 && _powerSupplied > 0) normalizedValue = 1;
        _previousPowerPercentage = _continentPowerPercentage;
        _continentPowerPercentage = (int) (normalizedValue * 100);
        
        if (_continentPowerPercentage == 100 && _previousPowerPercentage != 100)
        {
            FindObjectOfType<AudioManager>()._playPowerSufficient.Invoke();
        }

        if (_continentPowerPercentage != 100 && _previousPowerPercentage == 100)
        {
            FindObjectOfType<AudioManager>()._playPowerNotSufficientAnymore.Invoke();
        }
        
        _fillImage.DOFillAmount(normalizedValue, _fillAnimationDuration).SetEase(_fillAnimationEase).Play();
        OnPowerPercentageChanged.Invoke(PowerPercentage);
    }
}
