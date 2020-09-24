using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
[Serializable]
public class CE_GlobalSaveData
{
    #region Events
    #endregion

    #region Members
    #region Private
    [SerializeField] public List<CE_PlayerDB> savePlayerData = new List<CE_PlayerDB>();
    [SerializeField] public CE_GameManagerDB saveGameManagerData = new CE_GameManagerDB();
    #endregion
    #region Public
    #endregion
    #endregion

    #region Getters/Setters
    #endregion

    #region Methods
    #region Private
    #endregion
    #region Public
    public CE_GlobalSaveData(int CharacterIndexTurn, int NumberOfTurns, CE_MysteryCards mysteryCards, int PlayerIndex, List<IGamePlayable> _characters)
    {
        savePlayerData.Clear();
        for (int i = 0; i < _characters.Count; i++)
        {
            int _idNextRoom = _characters[i].NextRoomInvestigate ? _characters[i].NextRoomInvestigate.ID : -1;
            int _idLastRoom = _characters[i].LastRoom ? _characters[i].LastRoom.ID : -1;
            Vector3 _posNextDoor = _characters[i].NextDoorTarget ? _characters[i].NextDoorTarget.Position : Vector3.zero;
            CE_IA ai = _characters[i].CharacterRef.CharacterTransform.GetComponent<CE_IA>();
            if (ai)
                savePlayerData.Add(new CE_PlayerDB(_characters[i].CharacterRef.CharacterTransform.position,
                    _characters[i].IsInRoom,
                    _characters[i].NoteSystem.GetNotes,
                    _characters[i].HandCards, _idNextRoom, _idLastRoom, ai.Phase, _posNextDoor));
            else
                savePlayerData.Add(new CE_PlayerDB(_characters[i].CharacterRef.CharacterTransform.position,
                                                    _characters[i].IsInRoom,
                                                    _characters[i].NoteSystem.GetNotes,
                                                    _characters[i].HandCards, _idNextRoom, _idLastRoom, 0, _posNextDoor));
        }
        saveGameManagerData = new CE_GameManagerDB(CharacterIndexTurn, NumberOfTurns, mysteryCards, PlayerIndex);
    }
    #endregion
    #endregion
}