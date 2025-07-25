using System;
using UnityEngine;

public class GridTile : MonoBehaviour
{
    [SerializeField] private Transform _center;
    [SerializeField] private Sprite _gridCenterMarker;
    [SerializeField] private SpriteRenderer _gridCenterMarkerRenderer;
    
    public Vector3 GetCenter()
    {
        return _center.position;
    }

    public void Awake()
    {
        if(_gridCenterMarker) _gridCenterMarkerRenderer.sprite = _gridCenterMarker;
        HideGrid();
    }

    public void ShowGrid()
    {
        _gridCenterMarkerRenderer.gameObject.SetActive(true);
    }

    public void HideGrid()
    {
        _gridCenterMarkerRenderer.gameObject.SetActive(false);
    }
}
