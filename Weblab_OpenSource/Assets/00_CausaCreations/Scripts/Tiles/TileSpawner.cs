using System;
using System.Collections.Generic;
using CausaCreations.RisingTide.UIPanels;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TileSpawner : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    // TUTORIAL UNITY EVENTS
    public UnityEvent OnTileOfThisTypeDragged = new UnityEvent();
    public UnityEvent OnTileOfThisTypeReleased = new UnityEvent();
    public UnityEvent OnTileSpawnedFirst = new UnityEvent();
    public UnityEvent OnTilePlacedOnGrid = new UnityEvent();
    
    [SerializeField] private GameObject _tilePrefab;
    [SerializeField] private GameObject _tileContainer;
    [SerializeField] private float _holdTime;

    [SerializeField] private List<GridTile> _requiredGridTiles;
    
    [Header("UI")] 
    [SerializeField] private Image _icon;
    [SerializeField] private Image _iconUnavailable;
    [SerializeField] private GameObject _counterGO;
    [SerializeField] private TextMeshProUGUI _counter;
    [SerializeField] private bool _useCounter;
    
    [SerializeField] private GameObject _unavailableStack;
    [SerializeField] private List<GameObject> _availableStacks;

    private DragAndDrop _dragAndDrop;
    private TileGrid _tileGrid;
    private TileDeletionArea _tileDeletionArea;

    private int _maxAmountAvailable;
    private int _currentAmountAvailable;

    public GameObject Prefab => _tilePrefab;

    private bool _isPointerDown = false;
    private float _timer;
    private bool _onHoldTriggered;

    private bool _firstTileSpawned = false;
    public bool PlacedOnGridInvoked = false;

    private void Start()
    {
        _dragAndDrop = FindAnyObjectByType<DragAndDrop>();
        _tileGrid = FindAnyObjectByType<TileGrid>();
        _tileDeletionArea = FindAnyObjectByType<TileDeletionArea>(FindObjectsInactive.Include);

        var data = _tilePrefab.GetComponent<TileData>();
        _maxAmountAvailable = data.AmountAvailableInLevel;
        _currentAmountAvailable = data.AmountAvailableInLevel;
        UpdateStack(_currentAmountAvailable);

        _icon.sprite = data.Icon;
        _iconUnavailable.sprite = data.Icon;
        
        if (data.SetTileColor)
        {
            _icon.color = data.TileColor;
            _iconUnavailable.color = data.TileColor;
        }
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        _timer = 0;
        _onHoldTriggered = false;
        _isPointerDown = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isPointerDown = false;
        if(_timer < _holdTime) OnTap();
    }
    
    private void Update()
    {
        if (_isPointerDown)
        {
            _timer += Time.deltaTime;

            if (!_onHoldTriggered && _timer >= _holdTime)
            {
                _onHoldTriggered = true;
                OnHold();
            }
        }
    }
    
    private void OnHold()
    {
        if (_currentAmountAvailable <= 0) return;
        OnTileSpawn();
    }

    private void OnTap()
    {
        //OpenInformationUI();
    }

    public void OnTileSpawn()
    {
        var position = _dragAndDrop.GetFingerPosition();
        var tileTransform = Instantiate(_tilePrefab, position,  Quaternion.identity, _tileContainer.transform).transform;

        var tilePlacement = tileTransform.gameObject.GetComponent<TilePlacement>();
        tilePlacement.SetSpawner(this, _requiredGridTiles);
        tilePlacement.SetTileGrid(_tileGrid);
        
        var tileDeletion = tileTransform.gameObject.GetComponent<TileDeletion>();
        tileDeletion.SetSpawner(this);
        tileDeletion.SetTileDeletionArea(_tileDeletionArea);
        tileDeletion.SetDragAndDrop(_dragAndDrop);
        
        _dragAndDrop.SetDragging(tileTransform);
        _currentAmountAvailable -= 1;

        if (!_firstTileSpawned)
        {
            _firstTileSpawned = true;
            OnTileSpawnedFirst.Invoke();
        }
        
        UpdateStack(_currentAmountAvailable);

        switch (_tilePrefab.GetComponent<TileData>().ConnectionType)
        {
            case TileData.TileConnectionType.FullConnection:
                FindObjectOfType<AudioManager>()._playCableSpawned.Invoke();
                break;
            case TileData.TileConnectionType.HalfConnection:
                FindObjectOfType<AudioManager>()._playBuildingSpawned.Invoke();
                break;
            case TileData.TileConnectionType.NoConnection:
                FindObjectOfType<AudioManager>()._playPowerSpawned.Invoke();
                break;
        }
    }

    public void OnTileDeleted()
    {
        _currentAmountAvailable += 1;
        if (_currentAmountAvailable > _maxAmountAvailable)
            _currentAmountAvailable = _maxAmountAvailable;
        
        UpdateStack(_currentAmountAvailable);
    }

    public void UpdateStack(int currentAmountAvailable)
    {
        if (_useCounter)
        {
            _counter.text = currentAmountAvailable.ToString();
            _counterGO.SetActive(currentAmountAvailable > 1);

            if (currentAmountAvailable == 0)
            {
                _unavailableStack.SetActive(true);
                _icon.gameObject.SetActive(false);
                
                // for (int i = 0; i < _availableStacks.Count; i++)
                // {
                //     _availableStacks[i].SetActive( false );
                // }
            }
            else
            {
                _unavailableStack.SetActive(false);
                _icon.gameObject.SetActive(true);
                
                // for (int i = 0; i < _availableStacks.Count; i++)
                // {
                //     _availableStacks[i].SetActive( i == 0 );
                // }
            }
        }
        else
        {
            for (int i = 1; i <= _availableStacks.Count; i++)
            {
                _availableStacks[i-1].SetActive( i <= currentAmountAvailable );
            }
        
            _unavailableStack.SetActive(currentAmountAvailable == 0);
        }
    }
}
