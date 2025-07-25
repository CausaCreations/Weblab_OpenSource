using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public UnityEvent OnLoadStarting = new UnityEvent();
    public UnityEvent OnLoadCompleted = new UnityEvent();
    public UnityEvent OnThisSceneEnded = new UnityEvent();
    
    [SerializeField] private Color _fadeInColor;
    [SerializeField] private Color _fadeOutColor;
    [SerializeField] private float _fadeDuration;
    [SerializeField] private Image _fadeImage;
    
    private UnityEvent _fadeOutComplete = new UnityEvent();
    private string _nextScene;
    
    private void Awake()
    {
        FadeInScene();
    }

    private void OnEnable()
    {
        _fadeOutComplete.AddListener(LoadScene);
    }

    private void OnDisable()
    {
        _fadeOutComplete.RemoveListener(LoadScene);
    }

    public void LoadScene(string name)
    {
        _nextScene = name;
        FadeOutScene();
    }

    private void LoadScene()
    {
        OnThisSceneEnded.Invoke();
        SceneManager.LoadScene(_nextScene);
    }

    private void FadeInScene()
    {
        
        _fadeImage.color = _fadeInColor;
        _fadeImage.DOFade(1, 0).Play();
        _fadeImage.gameObject.SetActive(true);
        
        OnLoadStarting.Invoke();
        
        _fadeImage.DOFade(0, _fadeDuration).OnComplete(() =>
        {
            _fadeImage.gameObject.SetActive(false);
            OnLoadCompleted.Invoke();
        }).Play();
    }

    private void FadeOutScene()
    {
        _fadeImage.color = _fadeOutColor;
        _fadeImage.DOFade(0, 0).Play();
        _fadeImage.gameObject.SetActive(true);
        
        _fadeImage.DOFade(1, _fadeDuration).OnComplete(() =>
        {
            _fadeOutComplete.Invoke();
        }).Play();
    }
}
