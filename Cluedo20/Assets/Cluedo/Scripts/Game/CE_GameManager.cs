using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CE_GameManager : Singleton<CE_GameManager>
{
    #region Events

    public static event Action<IGamePlayable> OnPlayerInit = null;
    public static event Action OnPlayerReady = null;
    public static event Action<IGamePlayable> OnEndTurn = null;
    public static event Action<IGamePlayable> OnStartTurn = null;
    public static event Action<IGamePlayable, int> OnDiceRoll = null;
    public static event Action<IGamePlayable, CE_MysteryCards> OnEndGame = null;
    public static event Action OnEndInit = null;

    #endregion

    [SerializeField] List<CE_GameBoardCharacter> allGamePlayable = new List<CE_GameBoardCharacter>();

    [SerializeField] CE_Deck gameDeck = null;
    [SerializeField] CE_MysteryCards mysteryCards;

    [SerializeField, Range(0, 5)] int playerCharacterIndex = 0;
    [SerializeField, Range(2, 6)] int charactersNumber = 4;
    [SerializeField] int currentCharacterTurn = -1;
    [SerializeField] int currentTurn = 0;
    [SerializeField] bool demoMode = false;

    #region properties

    public int CurrentTurn => currentTurn;
    public CE_MysteryCards MysteryCards => mysteryCards;
    public int PlayerCharacterIndex => playerCharacterIndex;

    public bool IsValid => gameDeck;

    public bool StartGame { get; private set; } = false;
    public CE_Deck GameDeck => gameDeck;

    public List<IGamePlayable> AllCharacterInGame { get; private set; } = new List<IGamePlayable>();
    public IGamePlayable CurrentCharacterTurn => currentCharacterTurn > -1 ? AllCharacterInGame[currentCharacterTurn] : null;

    public int CurrentCharacterTurnIndex
    {
        get => currentCharacterTurn;
        set
        {
            currentCharacterTurn = value;
            currentCharacterTurn = currentCharacterTurn > AllCharacterInGame.Count - 1 ? 0 : currentCharacterTurn;
        }
    }
    #endregion

    protected override void Awake()
    {
        base.Awake();
        CE_SaveManager.OnLoadSave += CallbackOnLoadSave;
    }

    private IEnumerator Start()
    {
        //if (!IsValid) yield break;
        //while (!gameDeck.IsReady) yield return null;
        //yield return StartCoroutine(SetPlayerAndIA());
        //yield return StartCoroutine(LoadMysteryCards());
        //yield return StartCoroutine(CardDeckShare());
        //StartGame = true;
        //SetNextTurn();


        if (!IsValid) yield break;
        while (!gameDeck.IsReady)
            yield return null;

        if (!CE_SaveManager.Instance.LoadSave)
        {
            yield return StartCoroutine(SetPlayerAndIA());
            yield return StartCoroutine(LoadMysteryCards());
            yield return StartCoroutine(CardDeckShare());
            StartGame = true;
            SetNextTurn();
        }
        OnEndInit?.Invoke();

    }


    IEnumerator SetPlayerAndIA()
    {
        CE_Player _player = allGamePlayable[playerCharacterIndex].CharacterTransform.gameObject.AddComponent<CE_Player>();
        _player.Init(allGamePlayable[playerCharacterIndex]);
        AllCharacterInGame.Add(_player);
        _player.NoteSystem.AddAllItems(gameDeck.AllCardsDB);
        OnPlayerInit?.Invoke(_player);

        CE_UISuggestManager.Instance.SetPlayer(_player);
        CE_UINoteComponent.Instance.SetPlayerNoteSystem(_player.NoteSystem);
        _player.OnStartSuggest += CE_UIManager.Instance.InitUI;
        _player.OnSuggestProgress += CE_UIManager.Instance.AnswerPhrase;


        yield return new WaitForSeconds(.5f);


        int _count = 0;
        for (int i = 0; i < allGamePlayable.Count; i++)
        {
            if (i != playerCharacterIndex && _count < charactersNumber - 1)
            {
                CE_IA _ia = allGamePlayable[i].CharacterTransform.gameObject.AddComponent<CE_IA>();
                _ia.Init(allGamePlayable[i]);
                _ia.NoteSystem.AddAllItems(gameDeck.AllCardsDB);
                AllCharacterInGame.Add(_ia);

                _ia.OnStartSuggest += CE_UIManager.Instance.InitUI;
                _ia.OnSuggestProgress += CE_UIManager.Instance.AnswerPhrase;

                yield return new WaitForSeconds(.5f);
                _count++;
            }
        }
    }

    public IEnumerator LoadMysteryCard(CE_GlobalSaveData _db)
    {
        if (!IsValid) yield break;
        yield return new WaitForSecondsRealtime(1);
        mysteryCards = _db.saveGameManagerData.mysteryCards;

    }

    void CallbackOnLoadSave(bool _ok)
    {
        if (_ok)
        {
            StartGame = true;
            SetNextTurn();
        }

        // throw excepetion failed load save
    }


    public IEnumerator CardDeckShare(CE_GlobalSaveData _db)
    {
        if (!IsValid) yield break;
        int _characterIndex = _db.saveGameManagerData.CharacterIndexTurn;
        for (int i = 0; i < _db.savePlayerData.Count; i++)
        {
            AllCharacterInGame[i].SetHandCard(_db.savePlayerData[i].HandCards);
        }
        OnPlayerReady?.Invoke();
    }
    public IEnumerator SetPlayerAndAI(CE_GlobalSaveData _db)
    {
        playerCharacterIndex = _db.saveGameManagerData.PlayerIndex;
        charactersNumber = _db.savePlayerData.Count;
        CE_Player _player = allGamePlayable[_db.saveGameManagerData.PlayerIndex].CharacterTransform.gameObject.AddComponent<CE_Player>();
        _player.Init(allGamePlayable[_db.saveGameManagerData.PlayerIndex], _db.savePlayerData[_db.saveGameManagerData.PlayerIndex].Pos);
        AllCharacterInGame.Add(_player);
        _player.SetNotSystem(new CE_NoteSystem(_db.savePlayerData[_db.saveGameManagerData.PlayerIndex].notes));
        OnPlayerInit?.Invoke(_player);

        CE_UISuggestManager.Instance.SetPlayer(_player);
        CE_UINoteComponent.Instance.SetPlayerNoteSystem(_player.NoteSystem);
        _player.OnStartSuggest += CE_UIManager.Instance.InitUI;
        _player.OnSuggestProgress += CE_UIManager.Instance.AnswerPhrase;


        yield return new WaitForSeconds(.5f);
        int _count = 0;
        for (int i = 0; i < allGamePlayable.Count; i++)
        {
            if (i != playerCharacterIndex && _count < charactersNumber - 1)
            {
                CE_IA _ai = allGamePlayable[i].CharacterTransform.gameObject.AddComponent<CE_IA>();

                CE_Room _nextRoom = _db.savePlayerData[i].idNextRoom < 0 ? null : CE_Board.Instance.AllRooms[_db.savePlayerData[i].idNextRoom];
                CE_Room _lastRoom = _db.savePlayerData[i].idLastRoom < 0 ? null : CE_Board.Instance.AllRooms[_db.savePlayerData[i].idLastRoom];
                IAPhase _iaPhase = _db.savePlayerData[i].aiPhase;

                _ai.Init(allGamePlayable[i], _db.savePlayerData[i].Pos, _db.savePlayerData[i].IsInRoom, _lastRoom, _nextRoom, _iaPhase);

                _ai.SetNoteSystem(new CE_NoteSystem(_db.savePlayerData[i].notes));
                AllCharacterInGame.Add(_ai);
                yield return new WaitForSeconds(.5f);
                _count++;
            }
        }
    }


    IEnumerator LoadMysteryCards()
    {
        if (!IsValid) yield break;

        yield return new WaitForSeconds(1);
        CE_Card _character = gameDeck.PickRandomCard(CardType.Character);
        CE_Card _room = gameDeck.PickRandomCard(CardType.Room);
        CE_Card _weapon = gameDeck.PickRandomCard(CardType.Weapon);
        mysteryCards = new CE_MysteryCards(_character, _room, _weapon);

    }

    IEnumerator CardDeckShare()
    {
        if (!IsValid) yield break;

        int _characterIndex = 0;
        while (gameDeck.DeckCount != 0)
        {
            _characterIndex++;
            _characterIndex = _characterIndex > AllCharacterInGame.Count - 1 ? 0 : _characterIndex;
            IGamePlayable _character = AllCharacterInGame[_characterIndex];
            gameDeck.GiveCard(_character);
            yield return new WaitForEndOfFrame();
            //yield return null;
        }
        OnPlayerReady?.Invoke();
    }

    void SetNextTurn()
    {
        if (!StartGame) return;
        Transform _currentPlayableTransform = null;

        if (CurrentCharacterTurn != null)
        {
            OnEndTurn?.Invoke(CurrentCharacterTurn);
            CurrentCharacterTurn.Select(false);
            CurrentCharacterTurn.OnEndTurn -= SetNextTurn;
        }
        currentTurn++;
        CurrentCharacterTurnIndex++;

        _currentPlayableTransform = CurrentCharacterTurn.CharacterRef.CharacterTransform;
        //dice
        OnDiceRoll?.Invoke(CurrentCharacterTurn, UnityEngine.Random.Range(2, 13));
        OnStartTurn?.Invoke(CurrentCharacterTurn);
        CurrentCharacterTurn.OnEndTurn += SetNextTurn;

        if (_currentPlayableTransform && _currentPlayableTransform.GetComponent<CE_Player>())
        {
                SetNextTurn();
        }
        else
            CurrentCharacterTurn.Select(true);
    }

    public void EndGame()
    {
        Debug.Log($"{CurrentCharacterTurn.CharacterRef.ColorName} wins !");
        OnEndGame?.Invoke(CurrentCharacterTurn, mysteryCards);
        CurrentCharacterTurn.OnEndTurn -= SetNextTurn;
        StartGame = false;
    }

}
