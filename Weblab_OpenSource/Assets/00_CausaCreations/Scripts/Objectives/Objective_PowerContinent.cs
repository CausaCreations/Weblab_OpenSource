using System;
using AssetKits.ParticleImage;
using UnityEngine;
using UnityEngine.UI;

public class Objective_PowerContinent : Objective
{
    [SerializeField] private PowerManager _continentPowerManager;
    [SerializeField] private ParticleImage _particles;
    [SerializeField] private Image _icon;
    [SerializeField] private GameObject _fulfilledIcon;
    [SerializeField] private GameObject _unfulfilledIcon;

    private int _previousProgress;
    private int _progress;

    private void Start()
    {
        _fulfilledIcon.SetActive(false);
        _unfulfilledIcon.SetActive(true);
    }

    private void OnEnable()
    {
        _continentPowerManager.OnPowerPercentageChanged.AddListener(UpdateProgress);
    }

    private void OnDisable()
    {
        _continentPowerManager.OnPowerPercentageChanged.RemoveListener(UpdateProgress);
    }

    private void UpdateProgress(int progress)
    {
        if(progress != _previousProgress)
        {
            _previousProgress = progress;
            UpdateProgress();
        }
    }

    protected override void UpdateProgress()
    {
        _progress = (_previousProgress >= 100) ? 100 : 0;

        if (_progress == 100)
        {
            //_particles.transform.position = _icon.transform.position;
            //_particles.Play();
            
            _fulfilledIcon.SetActive(true);
            _unfulfilledIcon.SetActive(false);
        }
        else
        {
            _fulfilledIcon.SetActive(false);
            _unfulfilledIcon.SetActive(true);
        }
        
        OnProgressChanged.Invoke();
    }

    public override int GetProgressPercentage()
    {
        return _progress;
    }

    public override void OnSetActive()
    {
    }

    public override void OnSetInactive()
    {
    }
}
