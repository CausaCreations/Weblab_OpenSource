using System;
using System.Collections;
using System.Collections.Generic;
using Shapes;
using UnityEngine;

public class DashedLineFlipbook : MonoBehaviour
{
    [SerializeField] private Line _line;
    [SerializeField] private float _flipEveryXSeconds;
    [SerializeField] private float _offsetValue;
    
    
    private float _offset = 0;
    private float _timer = 0;

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= _flipEveryXSeconds)
        {
            _timer = 0;
            _offset = _offset == 0 ? _offsetValue : 0;
            _line.DashOffset = _offset;
        }
    }
}
