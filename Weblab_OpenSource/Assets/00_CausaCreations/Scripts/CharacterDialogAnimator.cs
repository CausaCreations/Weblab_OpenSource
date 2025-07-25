using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class CharacterDialogAnimator : MonoBehaviour
{
    [SerializeField] private Transform _characterOnMap;
    [SerializeField] private Transform _characterAtDialog;

    public UnityEvent OnCharacterReappearsOnMap = new UnityEvent();

    public void OnOpenDialog()
    {
        _characterOnMap.DOScale(0, 0.25f).SetEase(Ease.InOutBack).Play();
    }

    public void OnCloseDialog()
    {
        FindObjectOfType<AudioManager>()._playCharacterAppearOnMap.Invoke();
        _characterOnMap.DOScale(1, 0.25f).SetEase(Ease.InOutBack).Play();
        OnCharacterReappearsOnMap.Invoke();
    }
    
}
