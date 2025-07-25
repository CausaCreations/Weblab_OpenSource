using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorToStartingPointOfLineRenderer : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer;
    
    void LateUpdate()
    {
        transform.position = _lineRenderer.GetPosition(0);
    }
}
