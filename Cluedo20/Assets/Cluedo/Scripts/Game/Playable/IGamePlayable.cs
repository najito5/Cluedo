using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGamePlayable
{
    event Action<bool> OnSelectPlayable;
    event Action OnStartTurn;
    event Action OnEndTurn;
    event Action<CE_Suggest, IGamePlayable> OnStartSuggest;
    event Action<CE_Suggest, IGamePlayable, IGamePlayable, CE_Card> OnSuggestProgress;
    event Action<CE_Card, IGamePlayable, IGamePlayable> OnSuggestProgressResult;
    event Action<string, List<CE_Note>, Action<CE_Card>> OnSuggestPhrase;
    event Action<CE_Cell> OnNewStep;
    event Action<CE_Room, Action> OnCanEnterRoom;

    CE_GameBoardCharacter CharacterRef { get; }

    CE_Cell StartCell { get; }

    CE_NoteSystem NoteSystem { get; }
    CE_HandCards HandCards { get; }

    CE_Room NextRoomInvestigate { get; }
    CE_Room LastRoom { get; }
    CE_Door NextDoorTarget { get; }

    bool IsInRoom { get; }


    void Init(CE_GameBoardCharacter _character);
    void Select(bool _isSelected);
    void OnStart(IGamePlayable _gamePlayable, int _dice);
    void SetHandCard(CE_HandCards _handCard);
    void OnDrawGizmos();
}
