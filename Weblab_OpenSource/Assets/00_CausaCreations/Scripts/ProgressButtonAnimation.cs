using DG.Tweening;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ProgressButtonAnimation : MonoBehaviour
{
    [SerializeField] private Transform _middleOfScreen;

    [SerializeField] private float _animationDurationScale;
    [SerializeField] private Ease _animationEaseScale;
    [SerializeField] private float _animationDurationMove;
    [SerializeField] private Ease _animationEaseMove;
    
    [Button(nameof(Animate))] public bool animate;
    
    public void Animate()
    {
        var previousPosition = transform.position;
        
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOScale(0, 0));
        sequence.Append(transform.DOMove(_middleOfScreen.position, 0));
        sequence.Append(transform.DOScale(1, _animationDurationScale).SetEase(_animationEaseScale));
        sequence.AppendInterval(_animationDurationScale);
        sequence.Append(transform.DOMove(previousPosition, _animationDurationMove).SetEase(_animationEaseMove));
        
        sequence.Play();
    }
}
