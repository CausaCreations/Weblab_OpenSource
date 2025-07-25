using System;
using AssetKits.ParticleImage;
using UnityEngine;

public class Objective_Resources : Objective
{
    [SerializeField] private GameObject _icon;
    [SerializeField] private GameObject _iconReached;
    
    [SerializeField] private ParticleImage _particles;

    private int _progressPercentage;
    private ResourceManager _resourceManager;
    
    private void Awake()
    {
        _resourceManager = FindAnyObjectByType<ResourceManager>();
        
        _icon.SetActive(true);
        _iconReached.SetActive(false);
    }

    private void OnEnable()
    {
        _resourceManager.OnProgressUpdated.AddListener(UpdateProgress);
    }

    private void OnDisable()
    {
        _resourceManager.OnProgressUpdated.RemoveListener(UpdateProgress);
    }

    private void UpdateProgress(int progress)
    {
        if (_progressPercentage != progress)
        {
            _progressPercentage = progress;

            if (_progressPercentage >= 100)
            {
                bool wasActive = _icon.activeInHierarchy;
                _icon.SetActive(false);
                _iconReached.SetActive(true);

                if (wasActive)
                {
                    //_particles.transform.position = _icon.transform.position;
                    //_particles.Play();
                }
            }
            else
            {
                _icon.SetActive(true);
                _iconReached.SetActive(false);
            }
            
            OnProgressChanged.Invoke();
        }
    }

    protected override void UpdateProgress() { }

    public override int GetProgressPercentage()
    {
        return _progressPercentage;
    }

    public override void OnSetActive()
    {
    }

    public override void OnSetInactive()
    {
    }
}
