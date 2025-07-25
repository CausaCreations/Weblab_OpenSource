using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Sequence = DG.Tweening.Sequence;

public class TileConnections : MonoBehaviour
{
    public UnityEvent OnNetworkChanged = new UnityEvent();
    
    [SerializeField] private GameObject _connectionPrefab;
    [SerializeField] private GameObject _networkPrefab;
    
    [SerializeField] private GameObject _connectionContainer;
    [SerializeField] private GameObject _networkContainer;
    
    [SerializeField] private TileData _networkStartingPoint;
    
    private TileGrid _tileGrid;
    
    private Dictionary<(TileData, TileData), (TilePart, TilePart, GameObject)> _connections 
        = new Dictionary<(TileData, TileData), (TilePart, TilePart, GameObject)>();
    
    private List<List<(TilePart, TilePart)>> _paths = new List<List<(TilePart, TilePart)>>();

    private List<TileData> _tilesInNetwork = new List<TileData>();
    private List<TileData> _networkConnectionPointsInNetwork = new List<TileData>();
    
    private List<LineRenderer> _lineRenderers = new List<LineRenderer>();
    [SerializeField] private float _animationDurationPerPart;

    private Sequence _sequence;
    
    private void Awake()
    {
        _tileGrid = FindAnyObjectByType<TileGrid>();
    }

    private void OnEnable()
    {
        _tileGrid.OnTileRegistered.AddListener(TileRegisteredOnGrid);
        _tileGrid.OnTileUnregistered.AddListener(TileUnregisteredOnGrid);
    }

    private void OnDisable()
    {
        _tileGrid.OnTileRegistered.RemoveListener(TileRegisteredOnGrid);
        _tileGrid.OnTileUnregistered.RemoveListener(TileUnregisteredOnGrid);
    }

    private void TileRegisteredOnGrid(TileData tile)
    {
        CreateConnections(tile);
        EstablishConnectionNetwork();
    }
    
    private void TileUnregisteredOnGrid(TileData tile)
    {
        RemoveConnections(tile);
        EstablishConnectionNetwork();
    }

    #region Connections
        private void CreateConnections(TileData tileData)
        {
            foreach (var keyValuePair in _tileGrid.GetTilePartsInGridOfTile(tileData))
            {
                var position = keyValuePair.Key;
                var data = tileData;
                var tilePart = keyValuePair.Value.Item2;

                for (int i = position.x - 1; i <= position.x + 1; i++)
                {
                    for (int j = position.y - 1; j <= position.y + 1; j++)
                    {
                        // only check connections for straight up, down, left and right of tile
                        if (i == position.x || j == position.y)
                        {
                            var index = new Vector2Int(i, j);
                            if (_tileGrid.Exits(index))
                            {
                                var neighborTileData = _tileGrid.GetTileAt(index);
                                var neighborTilePart = _tileGrid.GetTilePartAt(index);

                                if (neighborTileData != data && !ConnectionExists(data, neighborTileData) &&
                                    ((neighborTileData.ConnectionType == TileData.TileConnectionType.FullConnection &&
                                      data.ConnectionType == TileData.TileConnectionType.FullConnection) ||
                                     (neighborTileData.ConnectionType == TileData.TileConnectionType.FullConnection &&
                                      data.ConnectionType == TileData.TileConnectionType.HalfConnection) ||
                                     (neighborTileData.ConnectionType == TileData.TileConnectionType.HalfConnection &&
                                      data.ConnectionType == TileData.TileConnectionType.FullConnection)))
                                {
                                    Connect(data, neighborTileData, tilePart, neighborTilePart);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void Connect(TileData data, TileData neighborTileData, TilePart tilePart, TilePart neighborTilePart)
        {
            var connectionVisual = Instantiate(_connectionPrefab, _connectionContainer.transform).GetComponent<LineRenderer>();
            var positionA = tilePart.GetCenter();
            var positionB = neighborTilePart.GetCenter();
            connectionVisual.SetPosition(0, positionA);
            connectionVisual.SetPosition(1, positionB);

            connectionVisual.startColor = data.TileColor;
            connectionVisual.endColor = neighborTileData.TileColor;
            
            _connections.Add((data,neighborTileData), (tilePart, neighborTilePart, connectionVisual.gameObject));
        }

        private void RemoveConnections(TileData tile)
        {
            var connections = _connections.Where(x => x.Key.Item1 == tile || x.Key.Item2 == tile).ToArray();
            for (int i = 0; i < connections.Length; i++)
            {
                Destroy(connections[i].Value.Item3);
                _connections.Remove(connections[i].Key);
            }
        }

        private bool ConnectionExists(TileData tileA, TileData tileB)
        {
            return _connections.ContainsKey((tileA, tileB)) ||
                   _connections.ContainsKey((tileB, tileA));
        }
    #endregion

    #region Network

    private void EstablishConnectionNetwork()
    {
        var previousTilesInNetwork = new TileData[_networkConnectionPointsInNetwork.Count];
        _networkConnectionPointsInNetwork.CopyTo(previousTilesInNetwork);

        foreach (var tileInNetwork in _tilesInNetwork)
        {
            tileInNetwork.OnDisconnectedFromNetwork.Invoke();
            
            if(tileInNetwork.TryGetComponent<TileConnectionQuality>(out TileConnectionQuality tileConnectionQuality))
            {
                tileConnectionQuality.OnTileDisconnectedFromNetwork();
            }
        }
        
        _tilesInNetwork.Clear();
        _networkConnectionPointsInNetwork.Clear();
        _paths.Clear();

        Node startingNode = new Node();
        startingNode.Tile = _networkStartingPoint;

        SearchConnectionNetworkRecursive(startingNode);
        
        //if (AreAMatch(previousTilesInNetwork, _networkConnectionPointsInNetwork))
            //return;
        
        OnNetworkChanged.Invoke();
        
        // Remove duplicate path parts so every path is only drawn once and overlapping paths are not drawn
        _paths = _paths.OrderByDescending(x => x.Count).ToList();
        List<(TilePart, TilePart)> occuringConnections = new List<(TilePart, TilePart)>();
        foreach (var path in _paths)
        {
            for(int i = 0; i < path.Count; i++)
            {
                if (occuringConnections.Contains(path[i]))
                {
                    path.Remove(path[i]);
                    i--;
                }
                else
                {
                    occuringConnections.Add(path[i]);
                }
            }
        }
        
        for (int i = 0; i < _paths.Count; i++)
        {
            _paths[i].Reverse();
        }

        for (int i = 0; i < _lineRenderers.Count; i++)
        {
            Destroy(_lineRenderers[i].gameObject);
        }
        _lineRenderers.Clear();
        
        _sequence.Kill();
        _sequence = DOTween.Sequence();
        DrawPaths(0, _sequence);
        _sequence.Play();
        FindObjectOfType<AudioManager>()._playLineRendererStarts.Invoke();
    }

    private bool AreAMatch(TileData[] previousTilesInNetwork, List<TileData> tilesInNetwork)
    {
        if (previousTilesInNetwork.Length != tilesInNetwork.Count) return false;

        for (int i = 0; i < tilesInNetwork.Count; i++)
        {
            if (previousTilesInNetwork[i] != tilesInNetwork[i]) return false;
        }
        
        return true;
    }

    private void SearchConnectionNetworkRecursive(Node currentNode)
    {
        _tilesInNetwork.Add(currentNode.Tile);
        currentNode.Tile.OnConnectedToNetwork.Invoke();

        if (currentNode.Tile.IsNetworkPoint)
        {
            _networkConnectionPointsInNetwork.Add(currentNode.Tile);
            _paths.Add(currentNode.CalculatePathToStartingPoint(null, _tileGrid));
        }

        if (currentNode.Tile.TryGetComponent<TileConnectionQuality>(out TileConnectionQuality tileConnectionQuality))
        {
            tileConnectionQuality.OnTileConnectedToNetwork(currentNode);
        }

        var connectionsToCurrentTileData =
            _connections.Where(x => x.Key.Item1 == currentNode.Tile || x.Key.Item2 == currentNode.Tile).ToArray();

        foreach (var connection in connectionsToCurrentTileData)
        {
            var tileA = connection.Key.Item1;
            var tileB = connection.Key.Item2;

            if (tileA == currentNode.Tile)
            {
                // Tile B is Neighbor
                if (!_tilesInNetwork.Contains(tileB))
                {
                    var newNode = new Node();
                    newNode.Tile = tileB;
                    newNode.Parent = currentNode;
                    newNode.TilePartOfParent = connection.Value.Item1;
                    newNode.TilePartConnectingToParent = connection.Value.Item2;

                    SearchConnectionNetworkRecursive(newNode);
                }
            }
            else if (tileB == currentNode.Tile)
            {
                // Tile A is Neighbor
                if (!_tilesInNetwork.Contains(tileA))
                {
                    var newNode = new Node();
                    newNode.Tile = tileA;
                    newNode.Parent = currentNode;
                    newNode.TilePartOfParent = connection.Value.Item2;
                    newNode.TilePartConnectingToParent = connection.Value.Item1;

                    SearchConnectionNetworkRecursive(newNode);
                }
            }
        }
    }

    private void DrawPaths(int pathIndex, Sequence sequence)
    {
        if (pathIndex >= _paths.Count) return;
        
        var lineRenderer = Instantiate(_networkPrefab, _networkContainer.transform).GetComponent<LineRenderer>();
        lineRenderer.gameObject.SetActive(false);
        lineRenderer.positionCount = 0;
        _lineRenderers.Add(lineRenderer);
        
        DrawPath(pathIndex, 0, sequence, lineRenderer);
        DrawPaths(++pathIndex, sequence);
    }

    private void DrawPath(int pathIndex, int pathPartIndex, Sequence sequence, LineRenderer lR)
    {
        if(pathPartIndex >= _paths[pathIndex].Count) return;
        
        var pathPart = _paths[pathIndex][pathPartIndex];

        if (pathPartIndex == 0)
        {
            lR.positionCount++;
            lR.SetPosition(lR.positionCount-1, pathPart.Item1.GetCenter());
        }
        else
        {
            var previousPathPart = _paths[pathIndex][pathPartIndex - 1];
            if (previousPathPart.Item2 != pathPart.Item1)
            {
                lR = Instantiate(_networkPrefab, _networkContainer.transform).GetComponent<LineRenderer>();
                lR.gameObject.SetActive(false);
                
                lR.positionCount = 1;
                lR.SetPosition(lR.positionCount-1, pathPart.Item1.GetCenter());
                
                _lineRenderers.Add(lR);
            }
        }

        sequence.Append(DOVirtual.DelayedCall(0, () =>
        {
            lR.positionCount++;
            lR.SetPosition(lR.positionCount-1, pathPart.Item1.GetCenter());
            lR.gameObject.SetActive(true);
        }));

        sequence.Append(DOVirtual.Vector2(pathPart.Item1.GetCenter(), pathPart.Item2.GetCenter(), _animationDurationPerPart, value =>
        {
            lR.SetPosition(lR.positionCount-1, value);
        }));

        sequence.AppendCallback(() =>
        {
            pathPart.Item1.gameObject.GetComponentInParent<TileData>().OnIsInMiddleOfNetwork.Invoke();
            if (pathPart.Item1.gameObject.GetComponentInParent<TileData>().ConnectionType ==
                TileData.TileConnectionType.HalfConnection)
                pathPart.Item1.gameObject.GetComponentInParent<TileData>().ConnectSound();
        });
        
        DrawPath(pathIndex, ++pathPartIndex, sequence, lR);
    }

    public class Node
    {
        public TileData Tile;
        public Node Parent;
        public TilePart TilePartConnectingToParent;
        public TilePart TilePartOfParent;

        public List<(TilePart, TilePart)> CalculatePathToStartingPoint(TilePart tilePartConnectingFromChild, TileGrid grid)
        {
            List<(TilePart, TilePart)> path = new List<(TilePart, TilePart)>();
            if (Parent == null) return path;

            if (tilePartConnectingFromChild != null && !Tile.IsNetworkPoint)
            {
                var current = tilePartConnectingFromChild;
                var goal = TilePartConnectingToParent;

                while (current != goal)
                {
                    bool gap = false;
                    var nextClosestTilePart = grid.GetNextClosestTilePart(current, goal, ref gap);

                    if (!gap)
                    {
                        path.Add((nextClosestTilePart, current));
                    }

                    current = nextClosestTilePart;
                }
            }
            
            path.Add((TilePartOfParent, TilePartConnectingToParent));
            path.AddRange(Parent.CalculatePathToStartingPoint(TilePartOfParent, grid));

            return path;
        }
    }

    #endregion

    public bool IsCurrentlyConnected(TileData tile)
    {
        return _tilesInNetwork.Any(x => x == tile);
    }
}
