using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "CharacterDataAsset", menuName = "InternetBuilder/CharacterDataAsset", order = 1)]
public class CharacterDataAsset : ScriptableObject
{
    public List<CharacterData> Data = new List<CharacterData>();

    [Serializable]
    public struct CharacterData
    {
        public Sprite Image;
        public Sprite ImageCharacterSelection;
        public LocalizedString FirstName;
        public LocalizedString LastName;
        public LocalizedString Place;
        public LocalizedString Description;
        public VideoClip Level_01_Animation;
        public VideoClip Level_02_Animation;
        public VideoClip Level_03_Animation;
    }
}
