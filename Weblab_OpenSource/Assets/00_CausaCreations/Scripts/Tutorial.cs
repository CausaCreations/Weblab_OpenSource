using System;
using System.Collections;
using System.Collections.Generic;
using CausaCreations.RisingTide.UIPanels;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private GameObject _bottomTileSpawners;
    [SerializeField] private GameObject _routerTutorialSpawnerLines;

    [SerializeField] private GameObject _cableTileSpawners;
    [SerializeField] private GameObject _cableTileSpawner01;
    [SerializeField] private GameObject _cableTileSpawner01TutorialLines;
    
    [SerializeField] private GameObject _cableTileSpawner02;
    [SerializeField] private GameObject _cableTileSpawner02TutorialLines;


    private bool _routerPlacedOnce = false;
    private bool _firstCablePlacedOnce = false;
    private bool _secondCablePlacedOnce = false;
    
    private void Start()
    {
        _bottomTileSpawners.SetActive(false);
        _cableTileSpawners.SetActive(false);
        _cableTileSpawner01.SetActive(false);
        _cableTileSpawner02.SetActive(false);
    }

    public void OnIntroDialogDone()
    {
        // TODO Show Dialog
        _bottomTileSpawners.SetActive(true);
        _routerTutorialSpawnerLines.SetActive(true);
    }

    public void OnRouterPlaced()
    {
        if(_routerPlacedOnce) return;
        _routerPlacedOnce = true;
        
        // TODO Hide Dialog
        _routerTutorialSpawnerLines.SetActive(false);
        _cableTileSpawners.SetActive(true);
        _cableTileSpawner01.SetActive(true);
        _cableTileSpawner01TutorialLines.SetActive(true);
        
        // TODO Show Next Dialog
    }

    public void OnFirstCablePlaced()
    {
        if(_firstCablePlacedOnce) return;
        _firstCablePlacedOnce = true;
        
        // TODO Hide Dialog
        _cableTileSpawner01TutorialLines.SetActive(false);
        _cableTileSpawner02.SetActive(true);
        _cableTileSpawner02TutorialLines.SetActive(true);
        
        // TODO Show Next Dialog
    }

    public void OnSecondCablePlaced()
    {
        if(_secondCablePlacedOnce) return;
        _secondCablePlacedOnce = true;
        
        // TODO Hide Dialog
        _cableTileSpawner02TutorialLines.SetActive(false);
    }

}
