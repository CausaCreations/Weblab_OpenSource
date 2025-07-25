using System;
using UnityEngine;
using UnityEngine.Events;

namespace CausaCreations.RisingTide.UIPanels
{
    [Serializable]
    public class UIPanelActionProvider
    {
        [SerializeField] private  UIPanelActions _action;
        
        public UIPanelAction GetNewUIPanelActionInstance()
        {
            switch (_action)
            {
                case UIPanelActions.Enable:
                    return new UIPanelEnableAction();
                case UIPanelActions.Disable:
                    return new UIPanelDisableAction();
                case UIPanelActions.EnableXScale:
                    return new UIPanelEnableXScaleAction();
                case UIPanelActions.DisableXScale:
                    return new UIPanelDisableXScaleAction();
                default:
                    return new UIPanelEnableAction();
            }
        }

        public enum UIPanelActions
        {
            Enable,
            Disable,
            EnableXScale,
            DisableXScale
        }
    }

    public class UIPanelAction
    {
        public UnityEvent ExecutionDoneEvent = new UnityEvent();
        public GameObject Target;
        public float BaseAnimationSpeed;
        
        public virtual void Execute()
        {
            ExecutionDoneEvent.Invoke();
        }
    }
}