using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CE_UIManager : Singleton<CE_UIManager>
{
    [SerializeField] GameObject UIResultSuggest = null;
    [SerializeField] GameObject resultContent = null;
    [SerializeField] GameObject answerContent = null;
    [SerializeField] CE_CardUIComponent cardUIPrefab = null;

    [SerializeField] TMP_Text headerText = null;
    [SerializeField] TMP_Text answerTextPrefab = null;
    [SerializeField] List<TMP_Text> allAnswers = new List<TMP_Text>();
    [SerializeField] List<CE_CardUIComponent> allSuggestCards = new List<CE_CardUIComponent>();

    float timer = 0;
    float maxTimer = 5;
    bool activateTimer = false;

    public void InitUI(CE_Suggest _suggest, IGamePlayable _playerAsking)
    {
        UIResultSuggest.SetActive(true);
        CE_CardUIComponent _character = Instantiate(cardUIPrefab, resultContent.transform);
        _character.Init(_suggest.Character);
        allSuggestCards.Add(_character);
        CE_CardUIComponent _weapon = Instantiate(cardUIPrefab, resultContent.transform);
        _weapon.Init(_suggest.Weapon);
        allSuggestCards.Add(_weapon);
        CE_CardUIComponent _room = Instantiate(cardUIPrefab, resultContent.transform);
        _room.Init(_suggest.Room);
        allSuggestCards.Add(_room);

        headerText.text = $"{_playerAsking.CharacterRef.ColorName} is suggesting {_suggest.Character.Name} with {_suggest.Weapon.Name} at {_suggest.Room.Name}";

    }

    public void AnswerPhrase(CE_Suggest _suggest, IGamePlayable _playerAsking, IGamePlayable _playerAsked, CE_Card _result)
    {
        activateTimer = true;
        timer = 0;

        TMP_Text _answer = Instantiate(answerTextPrefab, answerContent.transform);
        allAnswers.Add(_answer);

        _answer.text = $"{_playerAsked.CharacterRef.ColorName} {(_result == null ? "can't" : "can")} answer to {_playerAsking.CharacterRef.ColorName}";
    }

    public void ShowResult(CE_Card _result, IGamePlayable _playerAsking, IGamePlayable _playerAsked)
    {
        DestroyUI();
        CE_CardUIComponent _resultUI = Instantiate(cardUIPrefab, resultContent.transform);
        _resultUI.Init(_result);
        allSuggestCards.Add(_resultUI);

        TMP_Text _answer = Instantiate(answerTextPrefab, answerContent.transform);
        allAnswers.Add(_answer);

        _answer.color = Color.green;
        _answer.text = $"{_playerAsked.CharacterRef.ColorName} show a card to {_playerAsking.CharacterRef.ColorName}";

    }


    void DestroyUI()
    {
        for (int i = 0; i < allAnswers.Count; i++)
            Destroy(allAnswers[i].gameObject);

        allAnswers.Clear();

        for (int i = 0; i < allSuggestCards.Count; i++)
            Destroy(allSuggestCards[i].gameObject);

        allSuggestCards.Clear();
    }

    private void Update()
    {
        if (activateTimer)
        {
            timer += Time.deltaTime;
            if (timer >= maxTimer)
            {
                UIResultSuggest.SetActive(false);
                DestroyUI();
                activateTimer = false;
            }
        }
    }


}
