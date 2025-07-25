using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetTutorialPlayerPrefs : MonoBehaviour
{
    void Start()
    {
        PlayerPrefs.SetInt("WrongPlacementTile", 0);
        PlayerPrefs.SetInt("WrongPlacementOutside", 0);
    }
}
