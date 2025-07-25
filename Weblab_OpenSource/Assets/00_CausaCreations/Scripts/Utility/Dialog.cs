using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using CausaCreations.RisingTide.UIPanels;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

public class Dialog : MonoBehaviour
{
    [SerializeField] private UnityEvent OnLastDialog = new UnityEvent();
    
    [SerializeField] private List<LocalizedString> _lines;
    [SerializeField] private LocalizeStringEvent _localizeStringEvent;
    [SerializeField] private UIPanel _panel;
    [SerializeField] private GameObject _prevButton;
    [SerializeField] private GameObject _nextButton;
    [SerializeField] private GameObject _endButton;
    [SerializeField] private GameObject _logContainer;
    [SerializeField] private GameObject _prefabLog;
    [SerializeField] private GameObject _characterImage;
    [SerializeField] private bool _xIsFlipped;
    [SerializeField] private bool _hasNoCharacter;
    
    private int _index = 0;
    private bool _onLastDialogTriggered = false;
    private bool _isInLog = false;
    private bool _firstDialog = false;

    public void StartDialog()
    {
        _firstDialog = true;
        _characterImage.transform.DOScale(0, 0f).Play();
        GetComponentInParent<CharacterDialogAnimator>()?.OnOpenDialog();
        DOVirtual.DelayedCall(0f, () =>
        {
            _index = 0;
            UpdateUI();
            if (!_isInLog && _logContainer)
            {
                _isInLog = true;
                foreach (var line in _lines)
                {
                    var log = Instantiate(_prefabLog, _logContainer.transform);
                    log.transform.GetChild(1).GetComponent<LocalizeStringEvent>().StringReference = line;
                    log.transform.GetChild(1).GetComponent<LocalizeStringEvent>().RefreshString();
                }

                DOTween.Kill(_characterImage.transform);
                
                if(_xIsFlipped) _characterImage.transform.DOScale(new Vector3(-1,1,1), 0.25f).SetEase(Ease.OutBack).Play();
                else _characterImage.transform.DOScale(1, 0.25f).SetEase(Ease.OutBack).Play();
                
                _logContainer.SetActive(true);
            }
        }).Play();
    }

    public void Next()
    {
        _index++;
        if (_index >= _lines.Count) _index = _lines.Count-1;

        if(!_hasNoCharacter) FindObjectOfType<AudioManager>()._playCharacterSpeechProgress.Invoke();

        UpdateUI();
    }

    public void Previous()
    {
        _index--;
        if (_index < 0) _index = 0;
        
        UpdateUI();
    }

    private void UpdateUI()
    {
        _localizeStringEvent.StringReference = _lines[_index];
        _localizeStringEvent.RefreshString();
        
        _prevButton.SetActive(_index != 0);
        _nextButton.SetActive(_index < _lines.Count - 1);
        _endButton.SetActive(_index >= _lines.Count - 1);

        if (_index >= _lines.Count - 1)
        {
            if (!_onLastDialogTriggered)
            {
                OnLastDialog.Invoke();
                _onLastDialogTriggered = true;
            }
        }

        if (!_firstDialog && _characterImage)
        {
            _characterImage.transform.DOKill();
            DOTween.Kill(_characterImage.transform);
            _characterImage.transform.DOPunchScale(new Vector3(0, 0.05f, 0), 0.25f).Play();
        }
        _firstDialog = false;
    }

    public void End()
    {
        if(!_hasNoCharacter) FindObjectOfType<AudioManager>()._playCharacterSpeechEnd.Invoke();
        
        DOTween.Kill(_characterImage.transform);
        _characterImage.transform.DOScale(0, 0.25f).SetEase(Ease.OutBack).Play();
        DOVirtual.DelayedCall(0.25f, () => _panel?.Close()).Play();
        DOVirtual.DelayedCall(0.50f,()=> GetComponentInParent<CharacterDialogAnimator>()?.OnCloseDialog()).Play();
    }

    public void Prepend(List<LocalizedString> dialogToPrepend)
    {
        _lines.InsertRange(0, dialogToPrepend);
    }

    public List<LocalizedString> GetLines()
    {
        return _lines;
    }

}
