using DG.Tweening;
using UnityEngine;

namespace CausaCreations.RisingTide.UIPanels
{
    public class UIPanelEnableXScaleAction : UIPanelAction
    {
        public override void Execute()
        {
            Target.transform.localScale = new Vector3(0, 1, 1);
            Target.SetActive(true);
            Target.transform.DOScaleX(1, 0.25f).SetEase(Ease.OutQuart).OnComplete(() =>
            {
                DOVirtual.DelayedCall(0.25f, () => base.Execute()).Play();
            }).Play();
        }
    }
}