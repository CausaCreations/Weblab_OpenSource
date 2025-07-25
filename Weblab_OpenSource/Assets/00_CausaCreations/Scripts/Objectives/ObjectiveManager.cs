using System;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ObjectiveManager : MonoBehaviour
{
    [SerializeField] private Image _fillImage;
    [SerializeField] private List<Image> _levelEndScreenFillImages;
    
    [SerializeField] private float _fillImageAtOneStar;
    [SerializeField] private float _fillImageAtTwoStars;
    [SerializeField] private float _fillAnimationDuration;
    [SerializeField] private Ease _fillAnimationEase;

    [SerializeField] private GameObject _continueButton;

    [SerializeField] private GameObject _oneStarReachedImage;
    [SerializeField] private GameObject _twoStarReachedImage;
    [SerializeField] private GameObject _threeStarReachedImage;
    
    [SerializeField] private GameObject _oneStarUnreachedImage;
    [SerializeField] private GameObject _twoStarUnreachedImage;
    [SerializeField] private GameObject _threeStarUnreachedImage;

    [SerializeField] private List<Objective> _oneStarObjectives;
    [SerializeField] private List<Objective> _twoStarObjectives;
    [SerializeField] private List<Objective> _threeStarObjectives;

    public UnityEvent OneStarReached;
    public UnityEvent TwoStarsReached;
    public UnityEvent ThreeStarsReached;

    [SerializeField] private List<GameObject> _objectivesOneStar;
    [SerializeField] private List<GameObject> _objectivesTwoStar;
    [SerializeField] private List<GameObject> _objectivesThreeStar;

    [SerializeField] private Dialog _oneStarDialog;
    [SerializeField] private Dialog _twoStarDialog;
    [SerializeField] private Dialog _threeStarDialog;

    [SerializeField] private float _opacityOnceRemoved;
    [SerializeField] private float _opacityTwiceRemoved;

    [SerializeField] private List<RebuildUIOnStart> _forceRebuildOnChange;

    private int _previousReachedStar = 0;

    private bool oneStarObjectiveInvoked = false;
    private bool twoStarObjectiveInvoked = false;
    private bool threeStarObjectiveInvoked = false;

    private bool _updateOverallProgress = false;

    private void OnEnable()
    {
        foreach (var objective in _oneStarObjectives)
        {
            objective.OnProgressChanged.AddListener(UpdateOverallProgressTrigger);
        }
        
        foreach (var objective in _twoStarObjectives)
        {
            objective.OnProgressChanged.AddListener(UpdateOverallProgressTrigger);
        }
        
        foreach (var objective in _threeStarObjectives)
        {
            objective.OnProgressChanged.AddListener(UpdateOverallProgressTrigger);
        }
        UpdateOverallProgress();
    }

    private void OnDisable()
    {
        foreach (var objective in _oneStarObjectives)
        {
            objective.OnProgressChanged.RemoveListener(UpdateOverallProgressTrigger);
        }
        
        foreach (var objective in _twoStarObjectives)
        {
            objective.OnProgressChanged.RemoveListener(UpdateOverallProgressTrigger);
        }
        
        foreach (var objective in _threeStarObjectives)
        {
            objective.OnProgressChanged.RemoveListener(UpdateOverallProgressTrigger);
        }
    }

    private void Start()
    {
        _continueButton.SetActive(false);
        UpdateOverallProgress();
    }

    private void UpdateOverallProgressTrigger()
    {
        _updateOverallProgress = true;
    }

    private void LateUpdate()
    {
        if (_updateOverallProgress)
        {
            _updateOverallProgress = false;
            UpdateOverallProgress();
        }
    }

    private void UpdateOverallProgress()
    {
        bool hasOneStarObjectives = true;
        bool hasTwoStarObjectives = true;
        bool hasThreeStarObjectives = true;

        int oneStarPercentage = 0;
        if (_oneStarObjectives.Count > 0)
        {
            foreach (var objective in _oneStarObjectives)
            {
                oneStarPercentage += objective.GetProgressPercentage();
                //Debug.Log("Progress of " + objective.gameObject.name + ": " + objective.GetProgressPercentage());
            }
            oneStarPercentage = oneStarPercentage / _oneStarObjectives.Count;
        }
        else
        {
            hasOneStarObjectives = false;
        }
        oneStarPercentage = math.clamp(oneStarPercentage, 0, 100);
        
        
        int twoStarPercentage = 0;
        if (_twoStarObjectives.Count > 0)
        {
            foreach (var objective in _twoStarObjectives)
            {
                twoStarPercentage += objective.GetProgressPercentage();
            }
            twoStarPercentage = twoStarPercentage / _twoStarObjectives.Count;
        }
        else
        {
            hasTwoStarObjectives = false;
        }
        twoStarPercentage = math.clamp(twoStarPercentage, 0, 100);
        
        int threeStarPercentage = 0;
        if (_threeStarObjectives.Count > 0)
        {
            foreach (var objective in _threeStarObjectives)
            {
                threeStarPercentage += objective.GetProgressPercentage();
            }
            threeStarPercentage = threeStarPercentage / _threeStarObjectives.Count;
        }
        else
        {
            hasThreeStarObjectives = false;
        }
        threeStarPercentage = math.clamp(threeStarPercentage, 0, 100);
        
        
        // Has All Three Star Objectives
        if (hasOneStarObjectives && hasTwoStarObjectives && hasThreeStarObjectives)
        {
            if (oneStarPercentage != 100)
            {
                // No Stars Reached Yet
                var fillAmount = Mathf.Lerp(0, _fillImageAtOneStar, oneStarPercentage / 100f);
                _fillImage.DOFillAmount(fillAmount, _fillAnimationDuration).SetEase(_fillAnimationEase).Play();

                foreach (var image in _levelEndScreenFillImages)
                {
                    image.fillAmount = fillAmount;
                }
                
                //_continueButton.SetActive(false);
                
                SetReachedStarsUI(0);
                SetObjectives(0);
                _previousReachedStar = 0;
            }
            else if (oneStarPercentage == 100 && twoStarPercentage != 100)
            {
                // One Star Reached
                var fillAmount = Mathf.Lerp(_fillImageAtOneStar, _fillImageAtTwoStars, twoStarPercentage / 100f);
                _fillImage.DOFillAmount(fillAmount, _fillAnimationDuration).SetEase(_fillAnimationEase).Play();
                
                foreach (var image in _levelEndScreenFillImages)
                {
                    image.fillAmount = fillAmount;
                }
                
                _continueButton.SetActive(true);
                
                SetReachedStarsUI(1);
                SetObjectives(1);
                if (_previousReachedStar != 1)
                {
                    if(!oneStarObjectiveInvoked)
                    {
                        FindObjectOfType<AudioManager>()._playOneStarReached.Invoke();
                        
                        OneStarReached.Invoke();
                        oneStarObjectiveInvoked = true;
                    }
                }
                _previousReachedStar = 1;
            }
            else if (oneStarPercentage == 100 && twoStarPercentage == 100 && threeStarPercentage != 100)
            {
                // Two Stars Reached
                var fillAmount = Mathf.Lerp(_fillImageAtTwoStars, 1, threeStarPercentage / 100f);
                _fillImage.DOFillAmount(fillAmount, _fillAnimationDuration).SetEase(_fillAnimationEase).Play();
                
                foreach (var image in _levelEndScreenFillImages)
                {
                    image.fillAmount = fillAmount;
                }
                
                _continueButton.SetActive(true);
                
                SetReachedStarsUI(2);
                SetObjectives(2);
                if (_previousReachedStar != 2)
                {
                    if(!twoStarObjectiveInvoked)
                    {
                        // if (!oneStarObjectiveInvoked)
                        // {
                        //     _twoStarDialog.Prepend(_oneStarDialog.GetLines());
                        // }
                        
                        FindObjectOfType<AudioManager>()._playTwoStarReached.Invoke();
                        
                        TwoStarsReached.Invoke();
                        oneStarObjectiveInvoked = true;
                        twoStarObjectiveInvoked = true;
                    }
                }
                _previousReachedStar = 2;
            }
            else if (oneStarPercentage == 100 && twoStarPercentage == 100 && threeStarPercentage == 100)
            {
                // Three Stars Reached
                _fillImage.DOFillAmount(1, _fillAnimationDuration).SetEase(_fillAnimationEase).Play();
                
                foreach (var image in _levelEndScreenFillImages)
                {
                    image.fillAmount = 1;
                }
                
                _continueButton.SetActive(true);
                
                SetReachedStarsUI(3);
                SetObjectives(3);
                if (_previousReachedStar != 3)
                {
                    if (!threeStarObjectiveInvoked)
                    {
                        // if (!oneStarObjectiveInvoked)
                        // {
                        //     _threeStarDialog.Prepend(_oneStarDialog.GetLines());
                        // }
                        
                        FindObjectOfType<AudioManager>()._playThreeStarReached.Invoke();

                        ThreeStarsReached.Invoke();
                        oneStarObjectiveInvoked = true;
                        twoStarObjectiveInvoked = true;
                        threeStarObjectiveInvoked = true;
                    }
                }
                _previousReachedStar = 3;
            }
        }

        // Has only 1 Star Objectives
        if (hasOneStarObjectives && !hasTwoStarObjectives && !hasThreeStarObjectives)
        {
            if (oneStarPercentage != 100)
            {
                var fillAmount = Mathf.Lerp(0, 1, oneStarPercentage / 100f);
                _fillImage.DOFillAmount(fillAmount, _fillAnimationDuration).SetEase(_fillAnimationEase).Play();
                
                foreach (var image in _levelEndScreenFillImages)
                {
                    image.fillAmount = fillAmount;
                }
                
                //_continueButton.SetActive(false);
                
                SetObjectives(0);
                SetReachedStarsUI(0);
                _previousReachedStar = 0;
            }
            else
            {
                _fillImage.DOFillAmount(1, _fillAnimationDuration).SetEase(_fillAnimationEase).Play();
                
                foreach (var image in _levelEndScreenFillImages)
                {
                    image.fillAmount = 1;
                }
                
                _continueButton.SetActive(true);
                
                SetObjectives(3);
                SetReachedStarsUI(3);

                if (_previousReachedStar != 3)
                {
                    if(!oneStarObjectiveInvoked)
                    {
                        FindObjectOfType<AudioManager>()._playOneStarReached.Invoke();
                        
                        OneStarReached.Invoke();
                        oneStarObjectiveInvoked = true;
                    }
                }
                _previousReachedStar = 3;
            }
            
            // _oneStarReachedImage.SetActive(false);
            // _oneStarUnreachedImage.SetActive(false);
            // _twoStarReachedImage.SetActive(false);
            // _twoStarUnreachedImage.SetActive(false);
        }

        // Has only 2 Star Objectives
        if (!hasOneStarObjectives && hasTwoStarObjectives && !hasThreeStarObjectives)
        {
            if (twoStarPercentage != 100)
            {
                var fillAmount = Mathf.Lerp(0, 1, twoStarPercentage / 100f);
                _fillImage.DOFillAmount(fillAmount, _fillAnimationDuration).SetEase(_fillAnimationEase).Play();
                
                foreach (var image in _levelEndScreenFillImages)
                {
                    image.fillAmount = fillAmount;
                }
                
                //_continueButton.SetActive(false);
                
                SetObjectives(1);
                SetReachedStarsUI(0);
                _previousReachedStar = 0;
            }
            else
            {
                _fillImage.DOFillAmount(1, _fillAnimationDuration).SetEase(_fillAnimationEase).Play();
                
                foreach (var image in _levelEndScreenFillImages)
                {
                    image.fillAmount = 1;
                }
                
                _continueButton.SetActive(true);
                
                SetObjectives(3);
                SetReachedStarsUI(3);

                if (_previousReachedStar != 3)
                {
                    if(!twoStarObjectiveInvoked)
                    {
                        FindObjectOfType<AudioManager>()._playTwoStarReached.Invoke();
                        
                        TwoStarsReached.Invoke();
                        twoStarObjectiveInvoked = true;
                    }
                }
                _previousReachedStar = 3;
            }
            
            // _oneStarReachedImage.SetActive(false);
            // _oneStarUnreachedImage.SetActive(false);
            // _twoStarReachedImage.SetActive(false);
            // _twoStarUnreachedImage.SetActive(false);
        }
        
        // Has only 3 Star Objectives
        if (!hasOneStarObjectives && !hasTwoStarObjectives && hasThreeStarObjectives)
        {
            if (threeStarPercentage != 100)
            {
                var fillAmount = Mathf.Lerp(0, 1, threeStarPercentage / 100f);
                _fillImage.DOFillAmount(fillAmount, _fillAnimationDuration).SetEase(_fillAnimationEase).Play();
                
                foreach (var image in _levelEndScreenFillImages)
                {
                    image.fillAmount = fillAmount;
                }
                
                //_continueButton.SetActive(false);
                
                SetObjectives(2);
                SetReachedStarsUI(0);
                
                // QUICK FIX FOR LEVEL 01
                if(fillAmount >= 0.35) SetReachedStarsUI(1);
                if(fillAmount >= 0.6) SetReachedStarsUI(2);
                
                _previousReachedStar = 0;
            }
            else
            {
                _fillImage.DOFillAmount(1, _fillAnimationDuration).SetEase(_fillAnimationEase).Play();
                
                foreach (var image in _levelEndScreenFillImages)
                {
                    image.fillAmount = 1;
                }
                
                _continueButton.SetActive(true);
                
                SetObjectives(3);
                SetReachedStarsUI(3);

                if (_previousReachedStar != 3)
                {
                    if (!threeStarObjectiveInvoked)
                    {
                        FindObjectOfType<AudioManager>()._playThreeStarReached.Invoke();
                        
                        ThreeStarsReached.Invoke();
                        threeStarObjectiveInvoked = true;
                    }
                }
                _previousReachedStar = 3;
            }
            
            // _oneStarReachedImage.SetActive(false);
            // _oneStarUnreachedImage.SetActive(false);
            // _twoStarReachedImage.SetActive(false);
            // _twoStarUnreachedImage.SetActive(false);
        }
        
        // Has only 1 and 2 Star Objectives
        if (hasOneStarObjectives && hasTwoStarObjectives && !hasThreeStarObjectives)
        {
            if (oneStarPercentage != 100 && twoStarPercentage != 100)
            {
                var fillAmount = Mathf.Lerp(0, _fillImageAtTwoStars, oneStarPercentage / 100f);
                _fillImage.DOFillAmount(fillAmount, _fillAnimationDuration).SetEase(_fillAnimationEase).Play();
                
                foreach (var image in _levelEndScreenFillImages)
                {
                    image.fillAmount = fillAmount;
                }
                
                //_continueButton.SetActive(false);
                
                SetObjectives(0);
                SetReachedStarsUI(0);
                _previousReachedStar = 0;
            }
            else if (oneStarPercentage == 100 && twoStarPercentage != 100)
            {
                var fillAmount = Mathf.Lerp(_fillImageAtTwoStars, 1, twoStarPercentage / 100f);
                _fillImage.DOFillAmount(fillAmount, _fillAnimationDuration).SetEase(_fillAnimationEase).Play();
                
                foreach (var image in _levelEndScreenFillImages)
                {
                    image.fillAmount = fillAmount;
                }
                
                _continueButton.SetActive(true);
                
                SetObjectives(1);
                SetReachedStarsUI(2);
                
                if (_previousReachedStar != 2)
                {
                    if(!oneStarObjectiveInvoked)
                    {
                        FindObjectOfType<AudioManager>()._playOneStarReached.Invoke();
                        
                        OneStarReached.Invoke();
                        oneStarObjectiveInvoked = true;
                    }
                }
                _previousReachedStar = 2;
                
            }
            else if (oneStarPercentage == 100 && twoStarPercentage == 100)
            {
                _fillImage.DOFillAmount(1, _fillAnimationDuration).SetEase(_fillAnimationEase).Play();
                
                foreach (var image in _levelEndScreenFillImages)
                {
                    image.fillAmount = 1;
                }
                
                _continueButton.SetActive(true);
                
                SetObjectives(3);
                SetReachedStarsUI(3);
                
                if (_previousReachedStar != 3)
                {
                    if(!twoStarObjectiveInvoked)
                    {
                        // if (!oneStarObjectiveInvoked)
                        // {
                        //     _twoStarDialog.Prepend(_oneStarDialog.GetLines());
                        // }
                        
                        FindObjectOfType<AudioManager>()._playTwoStarReached.Invoke();
                        
                        TwoStarsReached.Invoke();
                        oneStarObjectiveInvoked = true;
                        twoStarObjectiveInvoked = true;
                    }
                }
                _previousReachedStar = 3;
            }
            
            _oneStarReachedImage.SetActive(false);
            _oneStarUnreachedImage.SetActive(false);
        }
        
        // Has only 1 and 3 Star Objectives
        if (hasOneStarObjectives && !hasTwoStarObjectives && hasThreeStarObjectives)
        {
            if (oneStarPercentage != 100 && threeStarPercentage != 100)
            {
                var fillAmount = Mathf.Lerp(0, _fillImageAtTwoStars, oneStarPercentage / 100f);
                _fillImage.DOFillAmount(fillAmount, _fillAnimationDuration).SetEase(_fillAnimationEase).Play();
                
                foreach (var image in _levelEndScreenFillImages)
                {
                    image.fillAmount = fillAmount;
                }
                
                //_continueButton.SetActive(false);
                
                SetObjectives(0);
                SetReachedStarsUI(0);
                _previousReachedStar = 0;
            }
            else if (oneStarPercentage == 100 && threeStarPercentage != 100)
            {
                var fillAmount = Mathf.Lerp(_fillImageAtTwoStars, 1, threeStarPercentage / 100f);
                _fillImage.DOFillAmount(fillAmount, _fillAnimationDuration).SetEase(_fillAnimationEase).Play();
                
                foreach (var image in _levelEndScreenFillImages)
                {
                    image.fillAmount = fillAmount;
                }
                
                _continueButton.SetActive(true);
                
                SetObjectives(2);
                SetReachedStarsUI(2);
                
                if (_previousReachedStar != 2)
                {
                    if(!oneStarObjectiveInvoked)
                    {
                        FindObjectOfType<AudioManager>()._playOneStarReached.Invoke();
                        OneStarReached.Invoke();
                        oneStarObjectiveInvoked = true;
                    }
                }
                _previousReachedStar = 2;
            }
            else if (oneStarPercentage == 100 && threeStarPercentage == 100)
            {
                _fillImage.DOFillAmount(1, _fillAnimationDuration).SetEase(_fillAnimationEase).Play();
                
                foreach (var image in _levelEndScreenFillImages)
                {
                    image.fillAmount = 1;
                }
                
                _continueButton.SetActive(true);
                
                SetObjectives(3);
                SetReachedStarsUI(3);
                
                if (_previousReachedStar != 3)
                {
                    if (!threeStarObjectiveInvoked)
                    {
                        // if (!oneStarObjectiveInvoked)
                        // {
                        //     _threeStarDialog.Prepend(_oneStarDialog.GetLines());
                        // }
                        
                        FindObjectOfType<AudioManager>()._playThreeStarReached.Invoke();
                        
                        ThreeStarsReached.Invoke();
                        oneStarObjectiveInvoked = true;
                        threeStarObjectiveInvoked = true;
                    }
                }
                _previousReachedStar = 3;
            }
            
            _oneStarReachedImage.SetActive(false);
            _oneStarUnreachedImage.SetActive(false);
        }
        
        // has only 2 and 3 Star Objectives
        if (!hasOneStarObjectives && hasTwoStarObjectives && hasThreeStarObjectives)
        {
            if (twoStarPercentage != 100 && threeStarPercentage != 100)
            {
                var fillAmount = Mathf.Lerp(0, _fillImageAtTwoStars, twoStarPercentage / 100f);
                _fillImage.DOFillAmount(fillAmount, _fillAnimationDuration).SetEase(_fillAnimationEase).Play();
                
                foreach (var image in _levelEndScreenFillImages)
                {
                    image.fillAmount = fillAmount;
                }
                
                //_continueButton.SetActive(false);
                
                SetObjectives(1);
                SetReachedStarsUI(0);
                _previousReachedStar = 0;
            }
            else if (twoStarPercentage == 100 && threeStarPercentage != 100)
            {
                var fillAmount = Mathf.Lerp(_fillImageAtTwoStars, 1, threeStarPercentage / 100f);
                _fillImage.DOFillAmount(fillAmount, _fillAnimationDuration).SetEase(_fillAnimationEase).Play();
                
                foreach (var image in _levelEndScreenFillImages)
                {
                    image.fillAmount = fillAmount;
                }
                
                _continueButton.SetActive(true);
                
                SetObjectives(2);
                SetReachedStarsUI(2);
                
                if (_previousReachedStar != 2)
                {
                    if(!twoStarObjectiveInvoked)
                    {
                        FindObjectOfType<AudioManager>()._playTwoStarReached.Invoke();
                        TwoStarsReached.Invoke();
                        twoStarObjectiveInvoked = true;
                    }
                }
                _previousReachedStar = 2;
            }
            else if (twoStarPercentage == 100 && threeStarPercentage == 100)
            {
                _fillImage.DOFillAmount(1, _fillAnimationDuration).SetEase(_fillAnimationEase).Play();
                
                foreach (var image in _levelEndScreenFillImages)
                {
                    image.fillAmount = 1;
                }
                
                _continueButton.SetActive(true);
                
                SetObjectives(3);
                SetReachedStarsUI(3);
                
                if (_previousReachedStar != 3)
                {
                    if (!threeStarObjectiveInvoked)
                    {
                        FindObjectOfType<AudioManager>()._playThreeStarReached.Invoke();
                        ThreeStarsReached.Invoke();
                        twoStarObjectiveInvoked = true;
                        threeStarObjectiveInvoked = true;
                    }
                }
                _previousReachedStar = 3;
            }
            
            _oneStarReachedImage.SetActive(false);
            _oneStarUnreachedImage.SetActive(false);
        }
    }

    private void SetReachedStarsUI(int reachedStar)
    {
        _oneStarReachedImage.SetActive(reachedStar >= 1);
        _oneStarUnreachedImage.SetActive(reachedStar < 1);
        _twoStarReachedImage.SetActive(reachedStar >= 2);
        _twoStarUnreachedImage.SetActive(reachedStar < 2);
        _threeStarReachedImage.SetActive(reachedStar >= 3);
        _threeStarUnreachedImage.SetActive(reachedStar < 3);
    }

    private void SetObjectives(int reachedStar)
    {
        if (reachedStar == 0)
        {
            foreach (var obj in _objectivesTwoStar)
            {
                obj.SetActive(false);
            }
            foreach (var obj in _objectivesThreeStar)
            {
                obj.SetActive(false);
            }
            foreach (var obj in _objectivesOneStar)
            {
                obj.SetActive(true);
                obj.GetComponent<CanvasGroup>().alpha = 1;
            }
            
            foreach(var objective in _twoStarObjectives)
            {
                objective.OnSetInactive();
            }
            foreach(var objective in _threeStarObjectives)
            {
                objective.OnSetInactive();
            }
            foreach(var objective in _oneStarObjectives)
            {
                objective.OnSetActive();
            }
        }

        if (reachedStar == 1)
        {
            foreach (var obj in _objectivesThreeStar)
            {
                obj.SetActive(false);
            }
            foreach (var obj in _objectivesOneStar)
            {
                obj.SetActive(true);
                obj.GetComponent<CanvasGroup>().alpha = _opacityOnceRemoved;
            }
            foreach (var obj in _objectivesTwoStar)
            {
                obj.SetActive(true);
                obj.GetComponent<CanvasGroup>().alpha = 1;
            }

            foreach(var objective in _threeStarObjectives)
            {
                objective.OnSetInactive();
            }
            foreach(var objective in _oneStarObjectives)
            {
                objective.OnSetActive();
            }
            foreach(var objective in _twoStarObjectives)
            {
                objective.OnSetActive();
            }
        }

        if (reachedStar == 2)
        {
            foreach (var obj in _objectivesOneStar)
            {
                obj.SetActive(true);
                obj.GetComponent<CanvasGroup>().alpha = _opacityTwiceRemoved;
            }
            foreach (var obj in _objectivesTwoStar)
            {
                obj.SetActive(true);
                obj.GetComponent<CanvasGroup>().alpha = _opacityOnceRemoved;
            }
            foreach (var obj in _objectivesThreeStar)
            {
                obj.SetActive(true);
                obj.GetComponent<CanvasGroup>().alpha = 1;
            } 
            
            foreach(var objective in _oneStarObjectives)
            {
                objective.OnSetActive();
            }
            foreach(var objective in _twoStarObjectives)
            {
                objective.OnSetActive();
            }
            foreach(var objective in _threeStarObjectives)
            {
                objective.OnSetActive();
            }
        }

        if (reachedStar == 3)
        {
            foreach (var obj in _objectivesOneStar)
            {
                obj.SetActive(true);
                obj.GetComponent<CanvasGroup>().alpha = _opacityTwiceRemoved;
            }
            foreach (var obj in _objectivesTwoStar)
            {
                obj.SetActive(true);
                obj.GetComponent<CanvasGroup>().alpha = _opacityOnceRemoved;
            }
            foreach (var obj in _objectivesThreeStar)
            {
                obj.SetActive(true);
                obj.GetComponent<CanvasGroup>().alpha = 1;
            } 
            
            foreach(var objective in _oneStarObjectives)
            {
                objective.OnSetActive();
            }
            foreach(var objective in _twoStarObjectives)
            {
                objective.OnSetActive();
            }
            foreach(var objective in _threeStarObjectives)
            {
                objective.OnSetActive();
            }
        }

        foreach (var rebuild in _forceRebuildOnChange)
        {
            rebuild.Rebuild();
        }
    }
}
