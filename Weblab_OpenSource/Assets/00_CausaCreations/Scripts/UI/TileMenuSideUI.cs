using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TileMenuSideUI : MonoBehaviour
{
    [SerializeField] private DragAndDrop _dragAndDrop;
    [SerializeField] private float _animationDuration;
    [SerializeField] private float _hideDelay;
    [SerializeField] private Ease _animationEase;

    private RectTransform _rectTransform;

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
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
    
    private void Update()
    {
        if (!_dragAndDrop.IsDragging && _rectTransform.localScale.x == 0)
        {
            DOTween.Kill(_rectTransform);
            _rectTransform.DOKill();
            _rectTransform.DOScaleX(1, _animationDuration).SetEase(_animationEase).Play();
        }
    }

    public void OnStartDragging()
    {
        DOTween.Kill(_rectTransform);
        _rectTransform.DOKill();
        DOVirtual.DelayedCall(_hideDelay, () =>
        {
            _rectTransform.DOScaleX(0, _animationDuration).SetEase(_animationEase).Play();
        }).Play();
    }

    public void OnEndDragging(Transform tile)
    {
        DOTween.Kill(_rectTransform);
        _rectTransform.DOKill();
        _rectTransform.DOScaleX(1, _animationDuration).SetEase(_animationEase).Play();
    }
}
