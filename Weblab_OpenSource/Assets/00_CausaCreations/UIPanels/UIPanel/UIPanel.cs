using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CausaCreations.RisingTide.UIPanels
{
    public class UIPanel : MonoBehaviour
    {
        [SerializeField] private List<UIPanelActionContainer> _openingActionContainers;
        [SerializeField] private List<UIPanelActionContainer> _closingActionContainers;
        [SerializeField] private bool _addEnablelingAndDisablingActionsAutomatically;
        
        private List<UIPanelAction> _openingActions = new List<UIPanelAction>();
        private List<UIPanelAction> _closingActions = new List<UIPanelAction>();

        public UnityEvent OpeningDoneAlwaysEvent = new UnityEvent();
        public UnityEvent ClosingDoneAlwaysEvent = new UnityEvent();
        public UnityEvent OnOpenEvent = new UnityEvent();
        public UnityEvent OnCloseEvent = new UnityEvent();

        public UIPanelManager Manager => _manager;
        private UIPanelManager _manager;

        private UIPanel _parent;
        private List<UIPanel> _siblings = new List<UIPanel>();
        private List<UIPanel> _children = new List<UIPanel>();

        private bool _isOpen = false;
        public bool IsOpen => _isOpen;

        private bool _isInitialized = false;

        [Button(nameof(OpenPanel))] public bool open;

        [Button(nameof(ClosePanel))] public bool close;

        public void Init(UIPanelManager manager)
        {
            if (_isInitialized)
            {
                return;
            }
            
            _manager = manager;

            // Gather Parent
            _parent = transform.parent.gameObject.GetComponentInParent<UIPanel>(true);
            if (_parent == this) _parent = null;

            // Gather Sibling UI Panels
            var parentTransform = transform.parent;
            foreach (Transform child in parentTransform)
            {
                if (child.TryGetComponent<UIPanel>(out UIPanel sib))
                {
                    if (sib != this) _siblings.Add(sib);
                }
            }

            // Gather Children UI Panels
            foreach (Transform child in transform)
            {
                if (child.TryGetComponent<UIPanel>(out UIPanel uiPanel))
                {
                    _children.Add(uiPanel);
                }
            }

            // Instantiate UI Panel Actions
            _openingActions = new List<UIPanelAction>();
            
            // Add Enabling this Panel to the Beginning Automatically
            if(_addEnablelingAndDisablingActionsAutomatically || _openingActionContainers.Count == 0)
            {
                var openingAction = new UIPanelEnableAction();
                openingAction.Target = this.gameObject;
                _openingActions.Add(openingAction);
            }
            
            for (int i = 0; i < _openingActionContainers.Count; i++)
            {
                var newAction = _openingActionContainers[i].UIPanelActionProvider.GetNewUIPanelActionInstance();
                newAction.Target = (_openingActionContainers[i].Target == null)
                    ? this.gameObject
                    : _openingActionContainers[i].Target;

                newAction.BaseAnimationSpeed = _openingActionContainers[i].BaseAnimationSpeed;
                _openingActions.Add(newAction);
            }
            
            _closingActions = new List<UIPanelAction>();
            for (int i = 0; i < _closingActionContainers.Count; i++)
            {
                var newAction = _closingActionContainers[i].UIPanelActionProvider.GetNewUIPanelActionInstance();
                newAction.Target = _closingActionContainers[i].Target == null
                    ? this.gameObject
                    : _closingActionContainers[i].Target;
                
                newAction.BaseAnimationSpeed = _openingActionContainers[i].BaseAnimationSpeed;
                _closingActions.Add(newAction);
            }
            
            // Add Disabling this Panel to the End automatically
            if(_addEnablelingAndDisablingActionsAutomatically || _closingActionContainers.Count == 0)
            {
                var closingAction = new UIPanelDisableAction();
                closingAction.Target = this.gameObject;
                _closingActions.Add(closingAction);
            }
            
            // Link Callbacks
            for (int i = 1; i < _openingActions.Count; i++)
            {
                _openingActions[i-1].ExecutionDoneEvent.AddListener(_openingActions[i].Execute);
            }
            _openingActions[^1].ExecutionDoneEvent.AddListener(OpeningDone);
            
            for (int i = 1; i < _closingActions.Count; i++)
            {
                _closingActions[i-1].ExecutionDoneEvent.AddListener(_openingActions[i].Execute);
            }
            _closingActions[^1].ExecutionDoneEvent.AddListener(ClosingDone);

            gameObject.SetActive(false);

            _isInitialized = true;
        }

        public void OpenPanel()
        {
            Open();
        }

        public void ClosePanel()
        {
            Close();
        }

        public void OpenWithDelay(float delay)
        {
            DOVirtual.DelayedCall(delay, () => Open(true)).Play();
        }

        public async void Open(bool register = true)
        {
            await WaitForInit();
            
            if (_isOpen)
            {
                OpeningDoneAlwaysEvent.Invoke();
                return;
            }

            if (register) _manager.RegisterOpening(this);

            // Open Parent
            if (_parent != null)
            {
                //CLog.Log(_parent.gameObject.name);
                _parent.OpeningDoneAlwaysEvent.AddListener(WaitForParentOpened);
                _parent.Open(false);
                return;
            }

            WaitForParentOpened();
        }

        public async Task WaitForInit()
        {
            while (!_isInitialized)
            {
                // waiting for initialization
                await Task.Yield();
            }
        }

        private void WaitForParentOpened()
        {
            _parent?.OpeningDoneAlwaysEvent.RemoveListener(WaitForParentOpened);

            foreach (var sibling in _siblings)
            {
                sibling.ClosingDoneAlwaysEvent.AddListener(WaitForSiblingsClosed);
                sibling.Close();
            }

            if (_siblings.Count == 0)
            {
                WaitForSiblingsClosed();
            }
        }

        private void WaitForSiblingsClosed()
        {
            foreach (var sibling in _siblings)
            {
                if (sibling.IsOpen) return;
            }

            foreach (var sibling in _siblings)
            {
               sibling.ClosingDoneAlwaysEvent.RemoveListener(WaitForSiblingsClosed);
            }

            _openingActions[0].Execute();
        }

        private void OpeningDone()
        {
            if(!_isOpen) OnOpenEvent.Invoke();
            
            _isOpen = true;
            OpeningDoneAlwaysEvent.Invoke();
            
            gameObject.SetActive(false);
            LayoutRebuilder.ForceRebuildLayoutImmediate(gameObject.GetComponent<RectTransform>());
            gameObject.SetActive(true);
        }

        public void Close()
        {
            if (!_isOpen)
            {
                ClosingDoneAlwaysEvent.Invoke();
                return;
            }

            foreach (var child in _children)
            {
                child.ClosingDoneAlwaysEvent.AddListener(WaitForAllChildrenClosed);
                child.Close();
            }

            if (_children.Count == 0)
            {
                WaitForAllChildrenClosed();
            }
        }

        private void WaitForAllChildrenClosed()
        {
            foreach (var child in _children)
            {
                if (child.IsOpen) return;
            }

            foreach (var child in _children)
            {
                child.ClosingDoneAlwaysEvent.RemoveListener(WaitForAllChildrenClosed);
            }

            _closingActions[0].Execute();
        }

        private void ClosingDone()
        {
            if(_isOpen) OnCloseEvent.Invoke();

            _isOpen = false;
            ClosingDoneAlwaysEvent.Invoke();
        }
        
        private void OnDestroy()
        {
            _openingActions.Clear();
            _closingActions.Clear();
        }

        [Serializable]
        public class UIPanelActionContainer
        {
            public GameObject Target;
            public float BaseAnimationSpeed;
            public UIPanelActionProvider UIPanelActionProvider;
        }
    }
}