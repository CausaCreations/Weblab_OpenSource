using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class SetVideoClipURL : MonoBehaviour
{
    public void Awake()
    {
        var videoplayer = GetComponent<VideoPlayer>();
        var clip = videoplayer.clip;
        videoplayer.url = System.IO.Path.Combine (Application.streamingAssetsPath, clip.name + ".mp4"); 
    }
}
