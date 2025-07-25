using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class TilePlacement : MonoBehaviour
{
    [SerializeField] private Vector2 _placementOffset;
    [SerializeField] private bool _isPreplaced;
    
    private List<GridTile> _requiredGridTiles;
    
    [Header("Animation Settings")] 
    [SerializeField] private float _animationMoveDuration;
    [SerializeField] private float _animationShakeDuration;
    [SerializeField] private Ease _animationMoveEase;
    
    private Vector3 SHAKESTRENGTH = new Vector3(0, 0, 10);
    private int SHAKEVIBRATO = 50;
    private float SHAKERANDOMNESS = 90f;
    
    private TileSpawner _spawner;
    private TileGrid _tileGrid;
    private Grid _grid;
    
    private bool _previousPositionSet = false;
    private Vector3 _previousPosition;

    public bool WrongPlacementOutsideOfMap => _wrongPlacementOutsideOfMap;
    private bool _wrongPlacementOutsideOfMap;

    public bool WrongPlacementOntopOfOtherTile => _wrongPlacementOntopOfOtherTile;
    private bool _wrongPlacementOntopOfOtherTile;
    
    public void SetSpawner(TileSpawner spawner, List<GridTile> requiredGridTiles)
    {
        _spawner = spawner;
        _requiredGridTiles = requiredGridTiles;
    }

    public void SetTileGrid(TileGrid tileGrid)
    {
        _tileGrid = tileGrid;
        _grid = FindAnyObjectByType<Grid>();
    }
    
    private void Start()
    {
        if (_isPreplaced)
        {
            _tileGrid = FindAnyObjectByType<TileGrid>();
            _tileGrid.RegisterTile(gameObject.GetComponent<TileData>());
        }
    }

    public void OnReleasedFromDragAndDrop()
    {
        // Snap to grid
        var position = transform.position + new Vector3(_placementOffset.x, _placementOffset.y, 0);
        var cellPosition = _grid.WorldToCell(position);
        transform.position = _grid.CellToWorld(cellPosition);
        
        // Check if tile is placed above other tile or not in grid
        bool placementIsValid = true;
        
        foreach (var playTilePart in transform.GetComponentsInChildren<TilePart>())
        {
            bool isPartPlacedOnGrid = playTilePart.CheckIsPlacedOnGrid();
            bool isPartPlacedOnOtherTile = playTilePart.CheckIsPlacedOnOtherTile();

            _wrongPlacementOutsideOfMap = !isPartPlacedOnGrid;
            _wrongPlacementOntopOfOtherTile = !isPartPlacedOnOtherTile;

            if (!isPartPlacedOnGrid || isPartPlacedOnOtherTile) placementIsValid = false;
        }

        if (_requiredGridTiles.Count != 0 && placementIsValid)
        {
            foreach (var playTilePart in transform.GetComponentsInChildren<TilePart>())
            {
                var gridTile = playTilePart.GetGridTile();
                if (!_requiredGridTiles.Contains(gridTile)) placementIsValid = false;
            }
        }

        if (placementIsValid)
        {
            SetPlacement();
            if (!_spawner.PlacedOnGridInvoked)
            {
                _spawner.PlacedOnGridInvoked = true;
                _spawner.OnTilePlacedOnGrid.Invoke();
            }

            _wrongPlacementOutsideOfMap = false;
            _wrongPlacementOntopOfOtherTile = false;
            
            switch (GetComponent<TileData>().ConnectionType)
            {
                case TileData.TileConnectionType.FullConnection:
                    FindObjectOfType<AudioManager>()._playDropCableCorrectly.Invoke();
                    break;
                case TileData.TileConnectionType.HalfConnection:
                    FindObjectOfType<AudioManager>()._playDropBuildingCorrectly.Invoke();
                    break;
                case TileData.TileConnectionType.NoConnection:
                    FindObjectOfType<AudioManager>()._playDropPowerCorrectly.Invoke();
                    break;
            }
        }
        else
        {
            
            HandleFalsePlacementToSpawner();
            
            // if (_previousPositionSet)
            // {
            //     HandleFalsePlacementToPrevious();
            // }
            // else
            // {
            //     HandleFalsePlacementToSpawner();
            // }
        }

        // var dontDestroyData = FindAnyObjectByType<DontDestroyOnLoadData>();
        // var tileData = GetComponent<TileData>();
        // if (!dontDestroyData.TileInformationAlreadyShown.Contains(tileData.Type))
        // {
        //     if (tileData.Type == TileData.TileType.CableHorizontalBottomLeft ||
        //         tileData.Type == TileData.TileType.CableHorizontalBottomRight ||
        //         tileData.Type == TileData.TileType.CableHorizontalTopLeft ||
        //         tileData.Type == TileData.TileType.CableHorizontalTopRight ||
        //         tileData.Type == TileData.TileType.CableVerticalBottomLeft ||
        //         tileData.Type == TileData.TileType.CableVerticalBottomRight ||
        //         tileData.Type == TileData.TileType.CableVerticalTopLeft ||
        //         tileData.Type == TileData.TileType.CableVerticalTopRight)
        //     {
        //         if (dontDestroyData.TileInformationAlreadyShown.Contains(TileData.TileType.CableHorizontalBottomLeft) ||
        //             dontDestroyData.TileInformationAlreadyShown.Contains(TileData.TileType.CableHorizontalBottomRight) ||
        //             dontDestroyData.TileInformationAlreadyShown.Contains(TileData.TileType.CableHorizontalTopLeft) ||
        //             dontDestroyData.TileInformationAlreadyShown.Contains(TileData.TileType.CableHorizontalTopRight) ||
        //             dontDestroyData.TileInformationAlreadyShown.Contains(TileData.TileType.CableVerticalBottomLeft) ||
        //             dontDestroyData.TileInformationAlreadyShown.Contains(TileData.TileType.CableVerticalBottomRight) ||
        //             dontDestroyData.TileInformationAlreadyShown.Contains(TileData.TileType.CableVerticalTopLeft) ||
        //             dontDestroyData.TileInformationAlreadyShown.Contains(TileData.TileType.CableVerticalTopRight))
        //         {
        //             dontDestroyData.TileInformationAlreadyShown.Add(GetComponent<TileData>().Type);
        //         }
        //         else
        //         {
        //             if(_spawner) _spawner.OpenInformationUI();
        //             dontDestroyData.TileInformationAlreadyShown.Add(GetComponent<TileData>().Type);
        //         }
        //     }
        //     else
        //     {
        //         if(_spawner) _spawner.OpenInformationUI();
        //         dontDestroyData.TileInformationAlreadyShown.Add(GetComponent<TileData>().Type);
        //     }
        // }
    }
    
    private void HandleFalsePlacementToPrevious()
    {
        DisableAllColliderInChildren();
        transform.DOShakeRotation(_animationShakeDuration, SHAKESTRENGTH, SHAKEVIBRATO, SHAKERANDOMNESS, true, ShakeRandomnessMode.Harmonic).OnComplete(() =>
        {
            transform.DOMove(_previousPosition, _animationMoveDuration).SetEase(_animationMoveEase).OnComplete(() =>
            {
                EnableAllColliderInChildren();
                SetPlacement();
            }).Play();
        }).Play();
    }

    private void HandleFalsePlacementToSpawner()
    {
        FindObjectOfType<AudioManager>()._playDropAnythingWrong.Invoke();
        
        DisableAllColliderInChildren();
        transform.DOShakeRotation(_animationShakeDuration, SHAKESTRENGTH, SHAKEVIBRATO, SHAKERANDOMNESS, true, ShakeRandomnessMode.Harmonic).OnComplete(() =>
        {
            transform.DOMove(_spawner.transform.position, _animationMoveDuration).SetEase(_animationMoveEase).OnComplete(() =>
            {
                GetComponent<TileDeletion>().DeleteTile();
            }).Play();
        }).Play();
    }
    
    private void DisableAllColliderInChildren()
    {
        foreach (var collider in GetComponentsInChildren<Collider2D>())
        {
            collider.enabled = false;
        }
    }
    
    private void EnableAllColliderInChildren()
    {
        foreach (var collider in GetComponentsInChildren<Collider2D>())
        {
            collider.enabled = true;
        }
    }
    
    private void SetPlacement()
    {
        _previousPosition = transform.position;
        _previousPositionSet = true;
        
        _tileGrid.RegisterTile(this.GetComponent<TileData>());
        GetComponent<TileData>().OnPlaced((int) _previousPosition.y);
    }
    
    public void RemovedFromGrid()
    {
        _tileGrid.UnregisterTile(this.GetComponent<TileData>());
    }

    public bool IsPlacementValid()
    {
        bool placementIsValid = true;
        
        foreach (var playTilePart in transform.GetComponentsInChildren<TilePart>())
        {
            bool isPartPlacedOnGrid = playTilePart.CheckIsPlacedOnGrid();
            bool isPartPlacedOnOtherTile = playTilePart.CheckIsPlacedOnOtherTile();

            if (!isPartPlacedOnGrid || isPartPlacedOnOtherTile) placementIsValid = false;
        }

        return placementIsValid;
    }

    public Vector2 GetPlacementOffset()
    {
        return _placementOffset;
    }

    public void OnDrag()
    {
        _spawner.OnTileOfThisTypeDragged.Invoke();
    }

    public void OnEndDrag()
    {
        _spawner.OnTileOfThisTypeReleased.Invoke();
    }
}
