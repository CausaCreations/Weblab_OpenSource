using UnityEngine;

namespace CausaCreations.RisingTide.UIPanels
{
    public class UIPanelDisableAction : UIPanelAction
    {
        public override void Execute()
        {
            Target.SetActive(false);
            base.Execute();
        }
    }
}