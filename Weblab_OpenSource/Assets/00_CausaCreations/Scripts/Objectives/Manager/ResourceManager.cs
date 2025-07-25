using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ResourceManager : MonoBehaviour
{
    private TileGrid _tileGrid;

    public UnityEvent<int> OnProgressUpdated = new UnityEvent<int>();
    
    [Header("References")]
    [SerializeField] private List<ResourceData> _resourceData;
    [SerializeField] private List<GameObject> _resourceIconContainers;
    [SerializeField] private ResourceBarColorChange _colorChange;
    [SerializeField] private RectTransform _threshold;


    [FormerlySerializedAs("_amountOfResourcesToStayAbove")]
    [Header("Progress Settings")] 
    [SerializeField] private int _amountOfResourcesToStayBelow;
    [SerializeField] private int _currentResourcesInUse;

    [Header("UI Settings")]
    [SerializeField] private HorizontalLayoutGroup _horizontalLayoutGroup;
    [SerializeField] private float _iconWidth;
    [SerializeField] private float _offsetFromRight;
    
    [Header("NEW")] 
    [SerializeField] private GameObject _starsBarActive;
    [SerializeField] private GameObject _starsBarInactive;
    [SerializeField] private Slider[] _sliders;
    
    private Dictionary<TileData.Resources, int> _resourcesUsed = new Dictionary<TileData.Resources, int>();
    private int _maxResourceAmount;
    private int _resourcesUsedAmount;
    private List<float> _widthsPerResource = new List<float>();

    private int _previousProgress = 0;

    private void Awake()
    {
        _tileGrid = FindAnyObjectByType<TileGrid>();
    }

    private void Start()
    {
        StartCoroutine(WaitForUnityUIToUpdate());
    }

    private IEnumerator WaitForUnityUIToUpdate()
    {
        yield return new WaitForNextFrameUnit();
        
        var tileSpawners = FindObjectsByType<TileSpawner>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        var maxAmount = 0;

        foreach (var tileSpawner in tileSpawners)
        {
            var tile = tileSpawner.Prefab.GetComponent<TileData>();
            foreach (var resource in tile.ResourceUse)
            {
                maxAmount += (resource.Amount * tile.AmountAvailableInLevel);
                
                // if (_resourcesUsed.ContainsKey(resource.Resource))
                //     _resourcesUsed[resource.Resource] += resource.Amount * tile.AmountAvailableInLevel;
                // else _resourcesUsed.Add(resource.Resource, resource.Amount * tile.AmountAvailableInLevel);
            }
        }
        _maxResourceAmount = maxAmount;

        // _widthsPerResource.Clear();
        // foreach (var container in _resourceIconContainers)
        // {
        //     var rectTransform = container.GetComponent<RectTransform>();
        //     var rectWidth = rectTransform.rect.width;
        //     _widthsPerResource.Add(rectWidth / _maxResourceAmount);
        // }

        // if (_threshold)
        // {
        //     // _threshold.anchoredPosition = new Vector2(1 / _maxResourceAmount * (_amountOfResourcesToStayBelow+3),
        //     //     _threshold.anchoredPosition.y);
        // }

        // var overhang = ((_maxResourceAmount) * _iconWidth) - rectWidth;
        //
        // float spacing = 0;
        // if (overhang > 0) spacing = 0 - (overhang / _maxResourceAmount);
        // else spacing = (overhang / _maxResourceAmount);
        //
        // _horizontalLayoutGroup.spacing = spacing;
        
        UpdateUI();
        UpdateProgress();
    }

    public void Refresh()
    {
        StartCoroutine(WaitForUnityUIToUpdate());
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
        foreach (var resourceUse in tile.ResourceUse)
        {
            if (_resourcesUsed.ContainsKey(resourceUse.Resource))
                _resourcesUsed[resourceUse.Resource] += resourceUse.Amount;
            else _resourcesUsed.Add(resourceUse.Resource, resourceUse.Amount);

            
            // if (_resourcesUsed.ContainsKey(resourceUse.Resource))
            // {
                // _resourcesUsed[resourceUse.Resource] -= resourceUse.Amount;
                //     
                // if(_resourcesUsed[resourceUse.Resource] <= 0)
                //     _resourcesUsed.Remove(resourceUse.Resource);
            // }
        }
        
        UpdateUI();
        UpdateProgress();
    }

    private void OnTileUnregistered(TileData tile)
    {
        foreach (var resourceUse in tile.ResourceUse)
        {
            // if (_resourcesUsed.ContainsKey(resourceUse.Resource))
            //     _resourcesUsed[resourceUse.Resource] += resourceUse.Amount;
            // else _resourcesUsed.Add(resourceUse.Resource, resourceUse.Amount);

            if (_resourcesUsed.ContainsKey(resourceUse.Resource))
            {
                _resourcesUsed[resourceUse.Resource] -= resourceUse.Amount;

                if (_resourcesUsed[resourceUse.Resource] <= 0)
                    _resourcesUsed.Remove(resourceUse.Resource);
            }
        }
        
        UpdateUI();
        UpdateProgress();
    }
    
    private void UpdateUI()
    {
        _resourcesUsedAmount = 0;
        foreach (var resource in _resourcesUsed)
        {
                for (int i = 0; i < resource.Value; i++)
                {
                    _resourcesUsedAmount++;
                }
        }

        _currentResourcesInUse = _resourcesUsedAmount;
        foreach (var slider in _sliders)
        {
            if (_maxResourceAmount == 0)
            {
                slider.value = 0;
                continue;
            }
            slider.value = 100.0f / _maxResourceAmount * _resourcesUsedAmount / 100.0f;
        }
        
        // foreach (var _resourceIconContainer in _resourceIconContainers)
        // {
        //     for (int i = 0; i < _resourceIconContainer.transform.childCount; i++)
        //     {
        //         Destroy(_resourceIconContainer.transform.GetChild(i).gameObject);
        //     }
        //
        //     // var resourcesLeft = _resourcesUsed.ToList();
        //     // int resourcesLeftAmount = 0;
        //     // foreach (var resourceLeft in resourcesLeft)
        //     // {
        //     //     for (int i = 0; i < resourceLeft.Value; i++)
        //     //     {
        //     //         resourcesLeftAmount++;
        //     //         Instantiate(_resourceData.Find(x => x.Resource == resourceLeft.Key).Prefab, _resourceIconContainer.transform);
        //     //     }
        //     // }
        //     // _resourcesLeftAmount = resourcesLeftAmount;
        //     
        //     _resourcesUsedAmount = 0;
        //
        //     if (_widthsPerResource.Count == 0) return;
        //     
        //     var _widthPerResource = _widthsPerResource[_resourceIconContainers.IndexOf(_resourceIconContainer)];
        //     foreach (var resource in _resourcesUsed)
        //     {
        //         for (int i = 0; i < resource.Value; i++)
        //         {
        //             _resourcesUsedAmount++;
        //             var GO = Instantiate(_resourceData.Find(x => x.Resource == resource.Key).Prefab, _resourceIconContainer.transform);
        //             GO.GetComponent<RectTransform>().sizeDelta = new Vector2(_widthPerResource, GO.GetComponent<RectTransform>().sizeDelta.y);
        //             _resourceIconContainer.SetActive(false);
        //             LayoutRebuilder.ForceRebuildLayoutImmediate(_resourceIconContainer.GetComponent<RectTransform>());
        //             _resourceIconContainer.SetActive(true);
        //         }
        //     }
        // }
    }

    public void UpdateProgress()
    {
        var maxAmountOfResources = _maxResourceAmount;
        var currentAmountOfResourcesUsed = _resourcesUsedAmount;
        var amountToStayBelow = _amountOfResourcesToStayBelow;
        
        //Debug.Log(currentAmountOfResourcesUsed);
        
        int progress;
        if (currentAmountOfResourcesUsed >= amountToStayBelow) progress = 0;
        else progress = 100;

        if (progress == 100 && _previousProgress != 100)
        {
            FindObjectOfType<AudioManager>()._resourceThresholdExceeded.Invoke();
        }

        if (progress != 100 && _previousProgress == 100)
        {
            FindObjectOfType<AudioManager>()._resourceThresholdRestored.Invoke();
        }

        _previousProgress = progress;

        //var progressInverted = Mathf.InverseLerp(0, amountToStayAbove, currentAmountOfResourcesLeft);
        //progressInverted = Mathf.Lerp(0, 1, progressInverted);
        
        //var progress = (int)  (progressInverted * 100);
        
        // _colorChange.SetToColorA();
        // if(progress < 100) _colorChange.SetToColorB();

        if (progress >= 100)
        {
            _starsBarActive.SetActive(true);
            _starsBarInactive.SetActive(false);
        }
        else
        {
            _starsBarActive.SetActive(false);
            _starsBarInactive.SetActive(true);
        }

        OnProgressUpdated.Invoke(progress);
    }

    [Serializable]
    public struct ResourceData
    {
        public TileData.Resources Resource;
        public GameObject Prefab;
    }
}
