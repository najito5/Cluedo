using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class CE_GameBoardCharacter 
{
    [SerializeField] string characterName = "";
    [SerializeField] CE_GameCharacters character = CE_GameCharacters.Green;
    [SerializeField] Transform characterTransform = null;
    [SerializeField] Color characterColor = Color.green;

    public string ColorName => $"<color=#{ColorUtility.ToHtmlStringRGBA(characterColor)}>{characterName}</color>";

    public string CharacterName => characterName;
    public CE_GameCharacters Character => character;
    public Transform CharacterTransform => characterTransform;
    public Color CharacterColor => characterColor;

}
