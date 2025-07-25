using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoadData : MonoBehaviour
{
    public int CharacterIndex = 0;
    //public List<TileData.TileType> TileInformationAlreadyShown = new List<TileData.TileType>();

    private static DontDestroyOnLoadData _instance;
    
    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        _instance = this;
        GameObject.DontDestroyOnLoad(this.gameObject);
    }

    public void Reset()
    {
        CharacterIndex = 0;
        //TileInformationAlreadyShown.Clear();
    }
}
