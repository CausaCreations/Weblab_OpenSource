using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetTiles : MonoBehaviour
{
    public void Reset()
    {
        var tileDeletions = GetComponentsInChildren<TileDeletion>();
        for (int i = 0; i < tileDeletions.Length; i++)
        {
            tileDeletions[i].DeleteTile();
        }
    }
}
