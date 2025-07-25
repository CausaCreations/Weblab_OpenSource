using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSoundTrigger : MonoBehaviour
{
    private void OnEnable()
    {
        GetComponent<Button>().onClick.AddListener(Trigger);
    }

    private void OnDisable()
    {
        GetComponent<Button>().onClick.RemoveListener(Trigger);
    }

    private void Trigger()
    {
        FindObjectOfType<AudioManager>()._playUIButtonClick.Invoke();
    }
}
