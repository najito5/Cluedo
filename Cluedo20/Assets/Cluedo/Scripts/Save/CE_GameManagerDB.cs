using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct CE_GameManagerDB
{
    #region Events
    #endregion

    #region Members
    #region Private
    #endregion
    #region Public
    [SerializeField] public int CharacterIndexTurn;
    [SerializeField] public int NumberOfTurns;
    [SerializeField] public CE_MysteryCards mysteryCards;
    [SerializeField] public int PlayerIndex;
	#endregion
	#endregion

	#region Getters/Setters
	#endregion

	#region Methods
	#region Private
	#endregion
	#region Public
    public CE_GameManagerDB(int _characterIndexTurn, int _numberOfTurns, CE_MysteryCards _mysteryCards, int _playerIndex)
    {
        CharacterIndexTurn = _characterIndexTurn;
        NumberOfTurns = _numberOfTurns;
        mysteryCards = _mysteryCards;
        PlayerIndex = _playerIndex;
    }
    #endregion
    #endregion
}