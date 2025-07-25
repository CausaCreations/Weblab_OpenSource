using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorToEndingPointOfLineRenderer : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer;
    
    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = _lineRenderer.GetPosition(_lineRenderer.positionCount - 1);
    }
}
