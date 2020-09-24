using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class CE_SaveManager : MonoBehaviour
{
    #region Events
    public static event Action<bool> OnLoadSave = null;
    #endregion

    #region Members
    #region Private
    static CE_SaveManager instance = null;
   public  static CE_SaveManager Instance => instance;


    CE_GlobalSaveData globlaSaveData = null;

    [SerializeField] CE_GameUser user = new CE_GameUser();


    [SerializeField] bool loadSave = false;
    public bool LoadSave => loadSave /*&& CE_DataPath.IsSave(currentUser)*/;
    #endregion
    #region Public
    #endregion
    #endregion

    #region Getters/Setters
    #endregion

    #region Methods
    #region Private
    private void Awake()
    {
        if (loadSave)
            CE_GameManager.OnEndInit += () => StartCoroutine(Load());
        else
            CE_GameManager.OnEndInit += () => StartCoroutine(Init());
        InitSingleton();
    }

    void InitSingleton()
    {
        if (instance == null)
            instance = this;

        name += "[MNG]";

        DontDestroyOnLoad(this);
    }

    IEnumerator Init()
    {
        yield return StartCoroutine(CreateGameEnvironment());
        yield return StartCoroutine(CreateUserEnvironmentJson(user));
        yield return new WaitForSeconds(20);
        yield return StartCoroutine(SaveGame());
    }

    IEnumerator CreateGameEnvironment()
    {
        bool _userExist = Directory.Exists(user.UserFolder);
        if(!_userExist)
        {
            Directory.CreateDirectory(user.UserFolder);
            _userExist = Directory.Exists(user.UserFolder);
            if (!_userExist) yield break ;
        }
        yield return null;
    }

    IEnumerator CreateUserEnvironmentJson(CE_GameUser _user)
    {
        bool _saveExist = File.Exists(_user.UserSaveJson);
        if (!_saveExist)
        {
            _user.SaveUserJson();
            _saveExist = File.Exists(_user.UserSaveJson);
            if (!_saveExist) yield break;
        }

        _saveExist = File.Exists(_user.UserSaveBin);
        if(!_saveExist)
        {
            _user.SaveUserBinary();
            _saveExist = File.Exists(_user.UserSaveBin);
            if (!_saveExist) yield break;
        }
        yield return null;
    }

    public IEnumerator SaveGame()
    {
        bool _userExist = Directory.Exists(user.UserFolder);
        bool _saveExist = File.Exists(user.UserSaveJson);
        if (!_userExist || !_saveExist) yield break;
        user.SaveUserJson();
        user.SaveUserBinary();
        yield return null;
    }

    public IEnumerator Load()
    {
        if (IsSave(user.UserPseudo))
        {
            globlaSaveData = CE_LoadSave.ReadSave(user.UserSaveJson);

        }
        CE_GameManager _instance = CE_GameManager.Instance;

        yield return StartCoroutine(_instance.SetPlayerAndAI(globlaSaveData));
        yield return StartCoroutine(_instance.LoadMysteryCard(globlaSaveData));
        yield return StartCoroutine(_instance.CardDeckShare(globlaSaveData));
        yield return StartCoroutine(SaveComplete());
    }

    private IEnumerator SaveComplete()
    {
        OnLoadSave?.Invoke(JsonFileExist());
        yield return null;
    }
    // bin
    public bool BinFileExist()
    {
        bool _exist = File.Exists(user.UserSaveBin);
        if (!_exist)
        {
            File.WriteAllText(user.UserSaveBin, "");
        }
        return _exist;
    }

    public bool JsonFileExist()
    {
        bool _exist = File.Exists(user.UserSaveJson);
        if (!_exist)
        {
            File.WriteAllText(user.UserSaveJson, "");
        }
        return _exist;
    }

    #endregion
    #region Public
    public bool IsSave(string _user)
    {
        if (!JsonFileExist() || !BinFileExist()) return false;
        
        return File.ReadAllText(user.UserSaveJson) != string.Empty; // todo test bin
    }
    #endregion
    #endregion
}