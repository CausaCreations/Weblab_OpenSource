using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceBarColorChange : MonoBehaviour
{
    [SerializeField] private Image _barOutside;
    [SerializeField] private Color _barOutsideColorA;
    [SerializeField] private Color _barOutsideColorB;
    
    [SerializeField] private Image _barInside;
    [SerializeField] private Color _barInsideColorA;
    [SerializeField] private Color _barInsideColorB;

    [SerializeField] private Color _fillColorA;
    [SerializeField] private Color _fillColorB;
    
    [SerializeField] private Image _break;
    [SerializeField] private Color _breakColorA;
    [SerializeField] private Color _breakColorB;

    [SerializeField] private GameObject _smileyA;
    [SerializeField] private GameObject _smileyB;

    [SerializeField] private bool _changeColor = true;
    [SerializeField] private bool _changeFillColor = true;
    
    
    public void SetToColorA()
    {
        if (_changeColor)
        {
            _barOutside.color = _barOutsideColorA;
            _barInside.color = _barInsideColorA;
            _break.color = _breakColorA;
        }

        if (_changeFillColor)
        {
            foreach (var childImages in _barInside.gameObject.GetComponentsInChildren<Image>())
            {
                if(childImages.gameObject.name == _barInside.gameObject.name) continue;
                childImages.color = _fillColorA;
            }
        }

        if(_smileyA) _smileyA.SetActive(true);
        if(_smileyB) _smileyB.SetActive(false);
    }

    public void SetToColorB()
    {
        if (_changeColor)
        {
            _barOutside.color = _barOutsideColorB;
            _barInside.color = _barInsideColorB;
            _break.color = _breakColorB;
        }
        
        if (_changeFillColor)
        {
            foreach (var childImages in _barInside.gameObject.GetComponentsInChildren<Image>())
            {
                if(childImages.gameObject.name == _barInside.gameObject.name) continue;
                childImages.color = _fillColorB;
            }
        }
        
        if(_smileyA) _smileyA.SetActive(false);
        if(_smileyB) _smileyB.SetActive(true);
    }
}
