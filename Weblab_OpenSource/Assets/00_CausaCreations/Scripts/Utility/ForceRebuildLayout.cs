using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ForceRebuildLayout : MonoBehaviour
{
    public void Force()
    {
        gameObject.SetActive(false);
        gameObject.SetActive(true);
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(gameObject.GetComponent<RectTransform>());
        
        gameObject.SetActive(false);
        gameObject.SetActive(true);

        Canvas.ForceUpdateCanvases();
    }
}
