using UnityEngine;

namespace CausaCreations.RisingTide.UIPanels
{
    public class UIPanelEnableAction : UIPanelAction
    {
        public override void Execute()
        {
            Target.SetActive(true);
            base.Execute();
        }
    }
}