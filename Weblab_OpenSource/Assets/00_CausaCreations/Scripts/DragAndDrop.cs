using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class DragAndDrop : MonoBehaviour
{
       [Header("Events")] 
       public UnityEvent OnStartDragging = new UnityEvent();
       public UnityEvent<Transform> OnEndDragging = new UnityEvent<Transform>();
       public UnityEvent FirstTimePlacementOutsideMap = new UnityEvent();
       public UnityEvent FirstTimePlacementOnOtherTile = new UnityEvent();

       [Header("Settings")]
       [SerializeField] private LayerMask _movableTileLayerMask;
       [SerializeField] private TileDeletionArea _tileDeletionArea;
       [SerializeField] private Grid _grid;
       [SerializeField] private InputAction _touchStart;
       [SerializeField] private InputAction _touchEnd;
       [SerializeField] private InputAction _position;
       [SerializeField] private bool _showAndHideGridDynamically;

       private bool _allowDragAndDrop = true;
       private bool _isDragging;
       public bool IsDragging => _isDragging;

       private Transform _currentTileTransform;
       private Vector3 _offset;
       
       private void OnEnable()
       {
              _touchStart.Enable();
              _touchEnd.Enable();
              _position.Enable();
              _touchStart.performed += OnTouchStart;
              _touchEnd.performed += OnTouchEnd;
       }

       private void OnDisable()
       {
              _touchStart.Disable();
              _touchEnd.Disable();
              _position.Disable();
              _touchStart.performed -= OnTouchStart;
              _touchEnd.performed -= OnTouchEnd;
       }

       public void OnTouchStart(InputAction.CallbackContext ctx)
       {
              if (_isDragging) return;

              if (!_allowDragAndDrop) return;

              if (Input.touches.Length > 1) return;

              RaycastHit2D hit = Physics2D.Raycast(GetFingerPosition(), Vector2.zero, 100, _movableTileLayerMask);
              if (hit)
              {
                     ShowOrHideGrid(true);
                     _isDragging = true;
                     _currentTileTransform = hit.rigidbody.transform;
                     _offset = GetFingerPosition() - _currentTileTransform.position;
                     
                     var sprites = _currentTileTransform.GetComponentsInChildren<SpriteRenderer>();
                     foreach (var sprite in sprites)
                     {
                            sprite.sortingLayerID = SortingLayer.NameToID("MovingTile");
                     }

                     _currentTileTransform.GetComponent<TilePlacement>().OnDrag();
                     _currentTileTransform.GetComponent<TilePlacement>().RemovedFromGrid();
                     OnStartDragging.Invoke();
              }
       }
       
       public void SetDragging(Transform tileTransform)
       {
              _isDragging = true;
              _currentTileTransform = tileTransform;
              
              ShowOrHideGrid(true);
              
              var tileData = _currentTileTransform.GetComponent<TileData>();
              _offset = Vector3.zero;
              _offset.x += tileData.TileWidth / 2;
              _offset.y += tileData.TileHeight / 2;

              var sprites = _currentTileTransform.GetComponentsInChildren<SpriteRenderer>();
              foreach (var sprite in sprites)
              {
                     sprite.sortingLayerID = SortingLayer.NameToID("MovingTile");
              }
              
              _currentTileTransform.GetComponent<TilePlacement>().OnDrag();
              _currentTileTransform.GetComponent<TilePlacement>().RemovedFromGrid();
              OnStartDragging.Invoke();
       }

       private void Update()
       {
              if (_isDragging)
              {
                     // Calculate the new position of the tile 
                     var positionOnScreenToWorld = GetFingerPosition();
                     var position = positionOnScreenToWorld - _offset;
                     var positionSaved = position;
                     
                     var tilePlacement = _currentTileTransform.GetComponent<TilePlacement>();
                     position.x += tilePlacement.GetPlacementOffset().x;
                     position.y += tilePlacement.GetPlacementOffset().y;
                     if (tilePlacement.IsPlacementValid())
                     {
                            position = _grid.CellToWorld(_grid.WorldToCell(position));
                     }
                     else
                     {
                            position = positionSaved;

                            if (tilePlacement.WrongPlacementOutsideOfMap)
                            {
                                   var firstTime = !PlayerPrefs.HasKey("WrongPlacementOutside") ||
                                                   PlayerPrefs.GetInt("WrongPlacementOutside") == 0;

                                   if (firstTime)
                                   {
                                          PlayerPrefs.SetInt("WrongPlacementOutside", 1);
                                          FirstTimePlacementOutsideMap.Invoke();
                                          FindObjectOfType<AudioManager>()._playTutorialPopUp.Invoke();
                                   }
                            }
                            else if (tilePlacement.WrongPlacementOntopOfOtherTile)
                            {
                                   var firstTime = !PlayerPrefs.HasKey("WrongPlacementTile") ||
                                                   PlayerPrefs.GetInt("WrongPlacementTile") == 0;

                                   if (firstTime)
                                   {
                                          PlayerPrefs.SetInt("WrongPlacementTile", 1);
                                          FirstTimePlacementOutsideMap.Invoke();
                                          FindObjectOfType<AudioManager>()._playTutorialPopUp.Invoke();
                                   }  
                            }
                     }

                     // Clamp to screen
                     var zeroX = Camera.main.ScreenToWorldPoint(Vector3.zero).x;
                     var zeroY = Camera.main.ScreenToWorldPoint(Vector3.zero).y;
                     Vector3 widthHeight = new Vector3(Screen.width, Screen.height, 0);
                     var width = Camera.main.ScreenToWorldPoint(widthHeight).x;
                     var height = Camera.main.ScreenToWorldPoint(widthHeight).y;

                     var tileData = _currentTileTransform.GetComponent<TileData>();
                     position.x = math.clamp(position.x, zeroX, width - tileData.TileWidth);
                     position.y = math.clamp(position.y, zeroY, height - tileData.TileHeight);
            
                     // Set tile to new position
                     _currentTileTransform.position = position;
              }
       }

       public void OnTouchEnd(InputAction.CallbackContext ctx)
       {
              if (!_isDragging) return;
              _isDragging = false;
              
              ShowOrHideGrid(false);
              
              var sprites = _currentTileTransform.GetComponentsInChildren<SpriteRenderer>();
              foreach (var sprite in sprites)
              {
                     sprite.sortingLayerID = SortingLayer.NameToID("Tiles");
              } 

              if (!_tileDeletionArea.GetPointerIsInDeletionArea())
              {
                     var tilePlacement = _currentTileTransform.gameObject.GetComponent<TilePlacement>();
                     tilePlacement.OnReleasedFromDragAndDrop();
              }
              
              _currentTileTransform.GetComponent<TilePlacement>().OnEndDrag();
              OnEndDragging.Invoke(_currentTileTransform);
              
              _currentTileTransform = null;
       }

       public void AllowDragAndDrop(bool allow)
       {
              _allowDragAndDrop = allow;
       }

       public Vector3 GetFingerPosition()
       {
              var positionOnScreen = _position.ReadValue<Vector2>();
              var position =  Camera.main.ScreenToWorldPoint(positionOnScreen);
              position.z = 0;
              return position;
       }

       public void ShowOrHideGrid(bool show)
       {
              if(!_showAndHideGridDynamically) return;
              
              foreach (var gridTile in _grid.gameObject.GetComponentsInChildren<GridTile>())
              {
                     if(show) gridTile.ShowGrid();
                     else gridTile.HideGrid();
              }
       }
}
