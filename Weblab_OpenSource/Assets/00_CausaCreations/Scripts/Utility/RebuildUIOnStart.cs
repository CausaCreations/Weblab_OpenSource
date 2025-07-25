using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RebuildUIOnStart : MonoBehaviour
{
    private void Start()
    {
        Rebuild();
    }

    public void Rebuild()
    {
        gameObject.SetActive(false);
        gameObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(this.GetComponent<RectTransform>());
    }
}
