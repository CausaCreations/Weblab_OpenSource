using UnityEngine;
using DG.Tweening;

namespace CausaCreations.RisingTide.UIPanels
{
    public class UIPanelDisableXScaleAction : UIPanelAction
    {
        public override void Execute()
        {
            Target.transform.DOScaleX(0, 0.25f).SetEase(Ease.OutQuart).OnComplete(() =>
            {
                Target.SetActive(false);
                DOVirtual.DelayedCall(BaseAnimationSpeed, () => base.Execute()).Play();
            }).Play();
        }
    }
}