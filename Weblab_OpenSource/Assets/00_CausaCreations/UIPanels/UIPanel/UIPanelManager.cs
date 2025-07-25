using System;
using System.Collections.Generic;
using UnityEngine;

namespace CausaCreations.RisingTide.UIPanels
{
    public class UIPanelManager : MonoBehaviour
    {
        [SerializeField] private UIPanel _openOnEnable;
        
        private UIPanel[] _UIPanels;
        private Stack<UIPanel> _openedPanels = new Stack<UIPanel>();

        private void OnEnable()
        {
            _UIPanels = GetComponentsInChildren<UIPanel>(true);
            foreach (var panel in _UIPanels)
            {
                panel.Init(this);
            }
            
            _openOnEnable?.Open();
        }

        public void RegisterOpening(UIPanel panel)
        {
            _openedPanels.Push(panel);
        }
    }
}