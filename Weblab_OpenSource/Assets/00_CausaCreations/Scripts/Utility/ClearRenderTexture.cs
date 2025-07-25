using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearRenderTexture : MonoBehaviour
{
    [SerializeField] private RenderTexture _texture;
    
    public void Clear()
    {
        _texture.Release();
    }
}
