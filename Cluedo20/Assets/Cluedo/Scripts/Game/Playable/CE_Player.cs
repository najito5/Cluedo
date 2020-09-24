using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CE_Player : MonoBehaviour, IGamePlayable
{
    #region Events

    public event Action<bool> OnSelectPlayable = null;
    public event Action OnStartTurn = null;
    public event Action OnEndTurn = null;
    public event Action<CE_Suggest, IGamePlayable> OnStartSuggest = null;
    public event Action<CE_Suggest, IGamePlayable, IGamePlayable, CE_Card> OnSuggestProgress = null;
    public event Action<CE_Card, IGamePlayable, IGamePlayable> OnSuggestProgressResult = null;
    public event Action<string, List<CE_Note>, Action<CE_Card>> OnSuggestPhrase = null;
    public event Action<CE_Cell> OnNewStep = null;
    public event Action<CE_Room, Action> OnCanEnterRoom = null;
    public event Action OnEnterInRoom = null;

    #endregion

    #region fields
    [SerializeField] CE_GameBoardCharacter characterRef = null;
    [SerializeField] CE_HandCards handCards = new CE_HandCards();
    [SerializeField] CE_NoteSystem noteSystem = new CE_NoteSystem();
    [SerializeField] Light lightFeedback = null;
    [SerializeField] PlayerPhase currentPhase = PlayerPhase.Idle;
    [SerializeField] List<CE_Card> clickedCards = new List<CE_Card>();
    [SerializeField] CE_Room lastRoom = null;
    [SerializeField] int diceCount = 0;
    [SerializeField] int stepCount = 0;
    [SerializeField] CE_Door currentDoor = null;
    [SerializeField] CE_Room currentRoom = null;
    [SerializeField] bool isInRoom = false;
    CE_CellNavigation playerNavigation = new CE_CellNavigation();
    #endregion

    #region Properties
    public CE_GameBoardCharacter CharacterRef => characterRef;

    public CE_Cell StartCell => throw new NotImplementedException();

    public CE_HandCards HandCards => handCards;

    public CE_Room NextRoomInvestigate => null;

    public CE_Room LastRoom => lastRoom;

    public CE_Door NextDoorTarget => null;

    public bool IsInRoom => isInRoom;

    public CE_NoteSystem NoteSystem => noteSystem;

    public CE_Cell CurrentCell { get; private set; } = null;

    public bool IsMoving { get; private set; } = false;

    public bool CanMove => throw new NotImplementedException();

    public int NavigationDiceValue => diceCount;

    public CE_Door CurrentDoor => currentDoor;

    public CE_Room CurrentRoom => currentRoom;

    public CE_CellNavigation Navigation => playerNavigation;
    #endregion

    private void Awake()
    {
        CE_GameManager.OnDiceRoll += OnStart;
        OnSelectPlayable += LightFeedback;
        OnCanEnterRoom += CE_DoorUI.Instance.UpdateDoorUI;
        OnNewStep += CheckDoorCell;
        OnEnterInRoom += EnterInRoom;
        OnEnterInRoom += CE_DoorUI.Instance.DesactiveDoorUI;

        OnSuggestProgressResult += CE_UIManager.Instance.ShowResult;

    }
    public void Init(CE_GameBoardCharacter _character)
    {
        characterRef = _character;
        transform.position = CE_Board.Instance.GetStartPos(characterRef.Character).position;
        name = $"{_character.CharacterName} [PLAYER]";
        lightFeedback = GetComponentInChildren<Light>();
        Select(false);

        CE_GamePointer.Instance.OnCellHit += SetNavigation;
        OnEndTurn += CE_DoorUI.Instance.DesactiveDoorUI;
    }

    public CE_NoteSystem SetNotSystem(CE_NoteSystem _value) => noteSystem = _value;

    public void Init(CE_GameBoardCharacter _character, Vector3 _pos)
    {
        characterRef = _character;
        transform.position = _pos;
        name = $"{_character.Character} [PLAYER]";
        Select(false);
        lightFeedback = GetComponentInChildren<Light>();
    }

    public void OnStart(IGamePlayable _gamePlayable, int _dice)
    {
        if (_gamePlayable.CharacterRef != characterRef)
            return;
        diceCount = _dice;

        OnStartTurn?.Invoke();
        Select(true);
        StartCoroutine(PlayerFsm());

        //      StartCoroutine(Suggest());
    }

    public void Select(bool _isSelected)
    {
        OnSelectPlayable?.Invoke(_isSelected);
    }

    void LightFeedback(bool _isEnable)
    {
        if (lightFeedback)
            lightFeedback.enabled = _isEnable;
    }

    public void SetNavigation(CE_Cell _cell, bool _action)
    {
        if (!IsMoving) return;
        playerNavigation.ComputePath(CE_Board.Instance, CE_Board.Instance.GetNearestCell(transform.position), _cell);
        stepCount = 0;
        IsMoving = true;
    }

    IEnumerator PlayerFsm()
    {
        if (currentRoom)
        {
            ExitRoom();
            diceCount--;
        }
        CheckDoorCell(CurrentCell);
        yield return StartCoroutine(Move());
        if (isInRoom) yield return StartCoroutine(Suggest());
        OnEndTurn?.Invoke();
    }

    public IEnumerator Move()
    {
        IsMoving = true;
        while (diceCount > 0)
        {
            if (isInRoom) break;
            if (playerNavigation.Path != null && CurrentCell != playerNavigation.EndCell && playerNavigation.StartCell != playerNavigation.EndCell)
            {
                if (CurrentCell == null || CurrentCell == playerNavigation.StartCell) stepCount++;
                CurrentCell = playerNavigation.Path[stepCount];
                transform.position = CurrentCell.Position;
                diceCount--;
                OnNewStep?.Invoke(CurrentCell);
                stepCount++;
                yield return new WaitForSeconds(.3f);
            }
            else yield return null;
        }
        yield return new WaitForSeconds(1);
        playerNavigation.Reset();
        diceCount = 0;
        IsMoving = false;
    }

    public void SetHandCard(CE_HandCards _handCard)
    {
        handCards = _handCard;
    }

    void CheckDoorCell(CE_Cell _currentCell)
    {
        if (!_currentCell) return;
        currentDoor = _currentCell.gameObject.GetComponent<CE_Door>();
        if (!currentDoor)
        {
            CE_DoorUI.Instance.DesactiveDoorUI();
            return;
        }
        if (currentDoor.ConnectedRoom != lastRoom && diceCount > 0)
            OnCanEnterRoom?.Invoke(currentDoor.ConnectedRoom, OnEnterInRoom);
    }

    void ExitRoom()
    {
        CE_Door _outDoor = null;
        if (isInRoom)
        {
            _outDoor = currentRoom.GetNearestDoor(this);
            transform.position = _outDoor.Position;
            isInRoom = false;
        }
        lastRoom = currentDoor.ConnectedRoom;
        currentRoom = null;
        currentDoor = null;
    }

    void EnterInRoom()
    {
        isInRoom = true;
        transform.position = currentDoor.ConnectedRoom.Position;
        currentRoom = currentDoor.ConnectedRoom;
    }




    public void AddClicked(CE_Card _card)
    {
        clickedCards.Add(_card);

    }

    IEnumerator Suggest()
    {
        currentPhase = PlayerPhase.Suggest;

        CE_UISuggestManager.Instance.MakeSuggest();
        while (clickedCards.Count < 2)
            yield return null;
        CE_Suggest _suggest = new CE_Suggest(CE_GameManager.Instance.GameDeck.GetCard(currentRoom.ID), clickedCards[0], clickedCards[1]);

        clickedCards.Clear();
        int _askIndex = CE_GameManager.Instance.CurrentCharacterTurnIndex;
        OnStartSuggest?.Invoke(_suggest, this);

        while (currentPhase == PlayerPhase.Suggest)
        {
            _askIndex++;
            _askIndex = _askIndex > CE_GameManager.Instance.AllCharacterInGame.Count - 1 ? 0 : _askIndex;
            IGamePlayable _askTo = CE_GameManager.Instance.AllCharacterInGame[_askIndex];
            CE_Card _result = _askTo.HandCards.GetSuggestCard(_suggest);

            OnSuggestProgress?.Invoke(_suggest, this, _askTo, _result);

            // Debug.Log($"{_askTo.CharacterRef.ColorName} {(_result == null ? "can't" : "can")} answer.");
            if (_result != null)
            {
                noteSystem.MatchCard(_result.ID);
                OnSuggestProgressResult?.Invoke(_result, this, _askTo);
                currentPhase = PlayerPhase.Idle;
            }
            if (CE_GameManager.Instance.CurrentCharacterTurnIndex == _askIndex)
            {
                CE_GameManager.Instance.EndGame();
                currentPhase = PlayerPhase.Idle;
            }
            yield return new WaitForSeconds(1);
        }
        yield return new WaitForSeconds(5);
        OnEndTurn?.Invoke();


    }





    public void OnDrawGizmos()
    {
        Gizmos.color = characterRef.CharacterColor;
        Gizmos.DrawSphere(transform.position + Vector3.up * 10, 1);
    }
}


public enum PlayerPhase
{
    Idle,
    Move,
    Suggest
}
