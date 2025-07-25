using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererManager : MonoBehaviour
{
    [SerializeField] private Transform _targetStart;
    [SerializeField] private Transform _targetEnd;
    

    private LineRenderer _lr;
    
    private void Awake()
    {
        _lr = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        _lr.SetPosition(0, _targetStart.position);
        _lr.SetPosition(1, _targetEnd.position);
    }
}
 