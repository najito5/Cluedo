using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct CE_PlayerDB
{
    #region Events
    #endregion

    #region Members
    #region Private
    #endregion
    #region Public
    [SerializeField] public float positionX;
    [SerializeField] public float positionY;
    [SerializeField] public float positionZ;
    public Vector3 Pos => new Vector3(positionX, positionY, positionZ);
    [SerializeField] public bool IsInRoom;
    [SerializeField] public List<CE_Note> notes;
    [SerializeField] public CE_HandCards HandCards;
    [SerializeField] public int idNextRoom;
    [SerializeField] public int idLastRoom;
    [SerializeField] public IAPhase aiPhase;


    [SerializeField] public float nextDoorPositionX;
    [SerializeField] public float nextDoorPositionY;
    [SerializeField] public float nextDoorPositionZ;
    #endregion
    #endregion

    #region Getters/Setters
    #endregion

    #region Methods
    #region Private
    #endregion
    #region Public
    public CE_PlayerDB(Vector3 _position, bool _isInRoom, List<CE_Note> _notes, CE_HandCards _handCards, int _idNextRoom, int _idLastRoom, IAPhase _aiPhase, Vector3 _nextDoorPosition)
    {
        positionX = _position.x;
        positionY = _position.y;
        positionZ = _position.z;

        IsInRoom = _isInRoom;

        notes = _notes;

        HandCards = _handCards;
        Debug.Log(_handCards.HandCards.Count);
        idLastRoom = _idLastRoom;
        idNextRoom = _idNextRoom;

        aiPhase = _aiPhase;
        nextDoorPositionX = _nextDoorPosition.x;
        nextDoorPositionY = _nextDoorPosition.y;
        nextDoorPositionZ = _nextDoorPosition.z;
    }
    #endregion
    #endregion
}