using UnityEngine;

public class TilePart : MonoBehaviour
{
    [SerializeField] private Transform _center;
    
    [Header("Settings")] 
    [SerializeField] private LayerMask _playTileLayerMask;
    [SerializeField] private LayerMask _gridTileLayerMask;
    
    private Collider2D _collider;
    
    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }
    
    public bool CheckIsPlacedOnGrid()
    {
        var hit = Physics2D.Raycast(_center.position, Vector2.zero, 100, _gridTileLayerMask);
        return hit;
    }

    public bool CheckIsPlacedOnOtherTile()
    {
        _collider.enabled = false;
        var hit = Physics2D.Raycast(_center.position, Vector2.zero, 100, _playTileLayerMask);
        _collider.enabled = true;
        return hit;
    }

    public Vector3 GetCenter()
    {
        return _center.position;
    }

    public GridTile GetGridTile()
    {
        var hit = Physics2D.Raycast(_center.position, Vector2.zero, 100, _gridTileLayerMask);
        return hit.transform.gameObject.GetComponent<GridTile>();
    }
}
