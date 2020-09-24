using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;


public class CE_UISuggestManager : Singleton<CE_UISuggestManager>
{
    [SerializeField] GameObject UISuggestGroup = null;
    [SerializeField] GameObject cardContent = null;
    [SerializeField] CE_CardButtonComponent suggestPrefab = null;
    [SerializeField] TMP_Text headerText = null;

    [SerializeField] List<CE_CardButtonComponent> allUICards = new List<CE_CardButtonComponent>();
    [SerializeField] List<CE_Card> suggestCards = new List<CE_Card>();
    [SerializeField] CE_Player player = null;

    //  CE_Card actualClicked = null;
    bool switchClick = false;


    public void SetPlayer(CE_Player _player) => player = _player;

    public void SetActiveGroup(bool _isEnable)
    {
        UISuggestGroup.SetActive(_isEnable);
    }


    public void InstantiateSuggest(CE_NoteSystem _playerNotes)
    {
        ClearUICards();
        suggestCards =switchClick? CE_GameManager.Instance.GameDeck.AllCardsDB.Where(c => c.Type == CardType.Weapon).ToList()
            :
            CE_GameManager.Instance.GameDeck.AllCardsDB.Where(c => c.Type == CardType.Character).ToList();

        headerText.text = switchClick ? "Select a Weapon : " : "Select a Character : ";
        for (int i = 0; i < suggestCards.Count; i++)
        {
            CE_CardButtonComponent _actual = Instantiate(suggestPrefab, cardContent.transform);
            _actual.Init(suggestCards[i], ClickOnCard, _playerNotes.GetNoteByID(suggestCards[i].ID).IsChecked);
            allUICards.Add(_actual);
        }
    }

    //private IEnumerator Start()
    //{
    //    yield return new WaitForSeconds(1);
    //    CE_NoteSystem _notes = new CE_NoteSystem();
    //    _notes.AddAllItems(CE_GameManager.Instance.GameDeck.AllCardsDB);
    //    _notes.MatchCard(1);
    //    InstantiateSuggest(_notes);
    //}


    void ClickOnCard(CE_Card _card)
    {
        player.AddClicked(_card);
        switchClick = !switchClick;
        InstantiateSuggest(player.NoteSystem);

        if (!switchClick)
            SetActiveGroup(false);
    }

    public void MakeSuggest()
    {
        if (!player) throw new System.Exception("player null ref");
        SetActiveGroup(true);
        InstantiateSuggest(player.NoteSystem);

    }

    void ClearUICards()
    {
        for (int i = 0; i < allUICards.Count; i++)
        {
            Destroy(allUICards[i].gameObject);
        }
        allUICards.Clear();
    }


}
