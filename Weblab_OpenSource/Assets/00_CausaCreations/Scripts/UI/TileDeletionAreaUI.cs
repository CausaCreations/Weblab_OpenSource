using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TileDeletionAreaUI : MonoBehaviour
{
    [SerializeField] private DragAndDrop _dragAndDrop;
    [SerializeField] private float _animationDuration;
    [SerializeField] private float _hideDelay;
    [SerializeField] private Ease _animationEase;
    

    private RectTransform _rectTransform;

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _rectTransform.DOScaleY(0, 0).Play();
    }

    private void OnEnable()
    {
        _dragAndDrop.OnStartDragging.AddListener(OnStartDragging);
        _dragAndDrop.OnEndDragging.AddListener(OnEndDragging);
    }

    private void OnDisable()
    {
        _dragAndDrop.OnStartDragging.RemoveListener(OnStartDragging);
        _dragAndDrop.OnEndDragging.RemoveListener(OnEndDragging);
    }

    public void OnStartDragging()
    {
        _rectTransform.DOScaleY(1, _animationDuration).SetEase(_animationEase).Play();
    }

    public void OnEndDragging(Transform tileTransform)
    {
        DOVirtual.DelayedCall(_hideDelay, () =>
        {
            _rectTransform.DOScaleY(0, _animationDuration).SetEase(_animationEase).Play();
        }).Play();
    }
}
