using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugLog : MonoBehaviour
{
    [SerializeField] private string _message;
    [SerializeField] private TextMeshPro _text;

    public void Log()
    {
        Debug.Log(_message);
        _text.text += "\n" +  _message;
    }
}
