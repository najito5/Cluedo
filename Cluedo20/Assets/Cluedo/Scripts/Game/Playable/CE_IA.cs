using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CE_IA : MonoBehaviour, IGamePlayable, IMove
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
    #endregion

    #region fields
    [SerializeField] CE_GameBoardCharacter characterRef = null;
    [SerializeField] CE_HandCards handCards = new CE_HandCards();
    [SerializeField] CE_NoteSystem noteSystem = new CE_NoteSystem();
    [SerializeField] int diceCount = 0;
    [SerializeField] int stepCount = 0;
    [SerializeField] Light lightFeedback = null;
    [SerializeField] CE_Room nextRoomInvestigate = null;
    [SerializeField] CE_Room lastRoom = null;
    [SerializeField] CE_Door nextDoorTarget = null;
    [SerializeField] IAPhase currentIAPhase = IAPhase.Idle;
    [SerializeField] bool isInRoom = false;
    [SerializeField] bool endMoveThinking = false;
    CE_CellNavigation iaNavigation = new CE_CellNavigation();

    #endregion

    #region Properties
    public CE_GameBoardCharacter CharacterRef => characterRef;

    public CE_Cell StartCell => throw new NotImplementedException();

    public CE_HandCards HandCards => handCards;

    public CE_Room NextRoomInvestigate => nextRoomInvestigate;

    public CE_Room LastRoom => lastRoom;

    public CE_Door NextDoorTarget => nextDoorTarget;

    public bool IsInRoom => isInRoom;

    public CE_NoteSystem NoteSystem => noteSystem;
    public IAPhase Phase => currentIAPhase;


    public CE_Cell CurrentCell { get; private set; } = null;

    public bool IsMoving { get; private set; } = false;

    public bool CanMove => throw new NotImplementedException();

    public int NavigationDiceValue => diceCount;

    public CE_CellNavigation Navigation => iaNavigation;
    #endregion

    public void Init(CE_GameBoardCharacter _character, Vector3 pos, bool _isInRoom, CE_Room _lastRoom, CE_Room _nextRoom, IAPhase _phase = IAPhase.Idle)
    {
        characterRef = _character;
        transform.position = pos;
        name = $"{_character.Character} [IA]";
        isInRoom = _isInRoom;
        lastRoom = _lastRoom;
        nextRoomInvestigate = _nextRoom;

        nextDoorTarget = GetNextDoor();

        currentIAPhase = _phase;
        Select(false);
        lightFeedback = GetComponentInChildren<Light>();
    }


    public void Init(CE_GameBoardCharacter _character)
    {
        characterRef = _character;
        transform.position = CE_Board.Instance.GetStartPos(characterRef.Character).position;
        name = $"{_character.CharacterName} [IA]";
        lightFeedback = GetComponentInChildren<Light>();

        Select(false);
    }

    private void Awake()
    {
        CE_GameManager.OnDiceRoll += OnStart;
        OnSelectPlayable += LightFeedback;

    }

    public void OnStart(IGamePlayable _gamePlayable, int _dice)
    {
        if (_gamePlayable.CharacterRef != characterRef)
            return;
        diceCount = _dice;
        OnStartTurn?.Invoke();
        Select(true);

        StartCoroutine(IAFsm());
    }

    public void SetHandCard(CE_HandCards _handCard)
    {
        handCards = _handCard;
    }
    public CE_NoteSystem SetNoteSystem(CE_NoteSystem _value) => noteSystem = _value;

    IEnumerator IAFsm()
    {
        //Debug.Log($"it's my turn {CharacterRef.ColorName} with {diceCount}");
        currentIAPhase = IAPhase.Idle;
        if (!nextRoomInvestigate)
            nextRoomInvestigate = GetNextRoom();
        if (!nextDoorTarget)
            nextDoorTarget = GetNextDoor();
        yield return StartCoroutine(IAMove());
        yield return StartCoroutine(IAEndMove());
        yield return StartCoroutine(IASuggest());
        OnEndTurn?.Invoke();

    }

    IEnumerator IAMove()
    {
        if (!nextDoorTarget) yield break;

        SetNavigation(nextDoorTarget.Cell, true);
        currentIAPhase = IAPhase.Move;
        yield return StartCoroutine(Move());
        endMoveThinking = true;
    }

    public IEnumerator Move()
    {
        while (iaNavigation.PathCompleted && transform.position != iaNavigation.EndCell.Position)
        {
            CurrentCell = iaNavigation.Path[stepCount];
            transform.position = CurrentCell.Position;
            stepCount++;
            if (stepCount == NavigationDiceValue)
            {
                yield return new WaitForSeconds(1);
                IsMoving = false;
                yield break;
            }
            yield return new WaitForSeconds(.3f);
        }
        yield return new WaitForSeconds(1);
        IsMoving = false;
    }

    IEnumerator IAEndMove()
    {
        while (endMoveThinking)
        {
            currentIAPhase = IAPhase.RoomEnterThink;
            if (stepCount < diceCount && CurrentCell == nextDoorTarget.Cell)
            {
                endMoveThinking = false;
                EnterInRoom();
            }
            //endMoveThinking = false;
            else
                endMoveThinking = false;
            yield return null;
        }
        nextDoorTarget = null;
        IsMoving = false;
        stepCount = 0;
        diceCount = 0;
        iaNavigation.Reset();
    }

    void EnterInRoom()
    {
        isInRoom = true;
        transform.position = nextRoomInvestigate.Position;
        currentIAPhase = IAPhase.Suggest;
    }


    IEnumerator IASuggest()
    {
        if (!isInRoom) yield break;
        CE_Card _pickCharacterCard = CE_GameManager.Instance.GameDeck.GetCard(noteSystem.PickRandomNote(CardType.Character).ID);
        CE_Card _pickWeaponCard = CE_GameManager.Instance.GameDeck.GetCard(noteSystem.PickRandomNote(CardType.Weapon).ID);
        CE_Card _pickRoomCard = CE_GameManager.Instance.GameDeck.GetCard(NextRoomInvestigate.ID);
        CE_Suggest _suggest = new CE_Suggest(_pickRoomCard, _pickCharacterCard, _pickWeaponCard);
        OnStartSuggest?.Invoke(_suggest, this);

        int _askIndex = CE_GameManager.Instance.CurrentCharacterTurnIndex;
       // Debug.Log($"{characterRef.ColorName} is suggesting {_suggest.Character.Name} with {_suggest.Weapon.Name} at {_suggest.Room.Name}");
        while (IsInRoom && currentIAPhase == IAPhase.Suggest)
        {
            _askIndex++;
            _askIndex = _askIndex > CE_GameManager.Instance.AllCharacterInGame.Count - 1 ? 0 : _askIndex;
            IGamePlayable _askTo = CE_GameManager.Instance.AllCharacterInGame[_askIndex];
            CE_Card _result = _askTo.HandCards.GetSuggestCard(_suggest);

            OnSuggestProgress?.Invoke(_suggest, this, _askTo, _result);

          //  Debug.Log($"{_askTo.CharacterRef.ColorName} {(_result == null ? "can't" : "can")} answer.");
            if (_result != null)
            {
                noteSystem.MatchCard(_result.ID);
                currentIAPhase = IAPhase.Idle;
            }
            if (CE_GameManager.Instance.CurrentCharacterTurnIndex == _askIndex)
            {
                CE_GameManager.Instance.EndGame();
                currentIAPhase = IAPhase.Idle;
            }
            yield return new WaitForSeconds(1);
        }
        nextRoomInvestigate = null;
        currentIAPhase = IAPhase.Idle;
        yield return new WaitForSeconds(5);
    }


    CE_Room GetNextRoom()
    {
        List<CE_Note> _nextRoomEmpty = noteSystem.PickRoomNotes();
        List<CE_Room> _correctRooms = !lastRoom ?
                                      CE_Board.Instance.AllRooms.Where(r => _nextRoomEmpty.Any(n => n.ID == r.Key)).Select(r => r.Value).ToList()
                                      :
                                      CE_Board.Instance.AllRooms.Where(r => r.Value != lastRoom && _nextRoomEmpty.Any(n => n.ID == r.Key)).Select(r => r.Value).ToList(); //fix cas Olivier

        CE_Room _room = _correctRooms.OrderBy(r => Vector3.Distance(r.Position, transform.position)).FirstOrDefault();
        return _room;
    }

    CE_Door GetNextDoor()
    {
        if (!nextRoomInvestigate) return null;
        CE_Door _door = nextRoomInvestigate.GetNearestDoor(this);
        CE_Door _outDoor = null;
        if (isInRoom)
        {
            _outDoor = lastRoom.GetNearestDoor(this);
            transform.position = _outDoor.Position;
            isInRoom = false;
        }
        lastRoom = nextRoomInvestigate;
        return _door;

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

    public void OnDrawGizmos()
    {
        Gizmos.color = characterRef.CharacterColor;
        Gizmos.DrawSphere(transform.position + Vector3.up * 10, 1);

        if (nextRoomInvestigate)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(nextRoomInvestigate.Position, 2);
            Gizmos.DrawLine(transform.position, nextRoomInvestigate.Position);
        }
        if (nextDoorTarget)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(nextDoorTarget.Position, 2);
            Gizmos.DrawLine(transform.position, nextDoorTarget.Position);
        }
    }

    public void StartNavigation()
    {
        throw new NotImplementedException();
    }

    public void SetNavigation(CE_Cell _cell, bool _action)
    {
        iaNavigation.ComputePath(CE_Board.Instance, CE_Board.Instance.GetNearestCell(transform.position), _cell);
        stepCount = 0;
        IsMoving = true;
    }

}


public enum IAPhase
{
    Idle,
    RollDice,
    FindTarget,
    RoomEnterThink,
    Move,
    Suggest
}

