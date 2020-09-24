using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Linq;

public class CE_UINoteComponent : Singleton<CE_UINoteComponent>, IPointerExitHandler, IPointerEnterHandler
{
    [SerializeField] Image backgroundImage = null;
    [SerializeField] Transform UINoteGroup = null;
    [SerializeField] TMP_Text textPrefab = null;
    [SerializeField] CE_NoteSystem playerNotes = null;
    [SerializeField] List<TMP_Text> allTextNotes = new List<TMP_Text>();


    public void SetPlayerNoteSystem(CE_NoteSystem _playerNotes)
    {
        playerNotes = _playerNotes;
        playerNotes.OnUpdateNotes += SetStyleText;
        InstantiateNotes();
    }


    private void Start()
    {
        backgroundImage = GetComponent<Image>();
        if (backgroundImage) backgroundImage.color = Color.white - new Color(0, 0, 0, .5f);

    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (backgroundImage) backgroundImage.color = Color.white;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (backgroundImage) backgroundImage.color = Color.white - new Color(0, 0, 0, .5f);

    }

    void InstantiateNotes()
    {
        List<CE_Note> _notes = playerNotes.GetNotes;

        for (int i = 0; i < _notes.Count; i++)
        {
            TMP_Text _text = Instantiate(textPrefab, UINoteGroup);
            allTextNotes.Add(_text);
            SetText(_text, _notes[i]);
        }
    }

    void SetText(TMP_Text _text, CE_Note _note)
    {
        _text.text = _note.NoteName;
        if (_note.IsChecked)
        {
            _text.fontStyle = FontStyles.Strikethrough;
            _text.color = Color.red;
        }
    }

    void SetStyleText(CE_Note _note)
    {
        TMP_Text _text = GetTextByName(_note.NoteName);
        if (_note.IsChecked)
        {
            _text.fontStyle = FontStyles.Strikethrough;
            _text.color = Color.red;
        }
    }

    TMP_Text GetTextByName(string _name)
    {
        return allTextNotes.Where(t => t.text.ToLower() == _name.ToLower()).FirstOrDefault();

    }

}

