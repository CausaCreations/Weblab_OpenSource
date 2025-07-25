using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ResetToStartAfterXSeconds : MonoBehaviour
{
    [SerializeField] private bool _autoReset;
    
    public UnityEvent OnResetManually = new UnityEvent();
    public UnityEvent OnResetAutomatically = new UnityEvent();
    
    [SerializeField] private float _resetTime;
    [SerializeField] private string _startSceneName;
    [SerializeField] private SceneLoader _sceneLoader;

    private float _timer = 0;
    private bool _reset = false;

    private void OnEnable()
    {
        InputSystem.onEvent += (eventPtr, device) => ResetTimer();
    }
    
    private void OnDisable()
    {
        InputSystem.onEvent -= (eventPtr, device) => ResetTimer();
    }

    private void Update()
    {
        if (!_autoReset) return;
        
        _timer += Time.deltaTime;
        if (!_reset && _timer >= _resetTime)
        {
            _reset = true;
            OnResetAutomatically.Invoke();
            _sceneLoader.LoadScene(_startSceneName);
        }
    }

    public void Reset()
    {
        OnResetManually.Invoke();
        _sceneLoader.LoadScene(_startSceneName);
    }

    private void ResetTimer()
    {
        _timer = 0;
    }

}
