using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CE_StartCharacter
{

    [SerializeField] Transform startCell;
    [SerializeField] CE_GameCharacters character;

    public Transform StartCell => startCell;
    public CE_GameCharacters Character => character;

}
