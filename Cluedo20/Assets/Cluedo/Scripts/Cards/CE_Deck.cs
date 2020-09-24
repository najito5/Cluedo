using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CE_Deck : MonoBehaviour
{
    [SerializeField] CE_CardsDB gameCardsDB;
    [SerializeField] List<CE_Card> availableCards = new List<CE_Card>();

    public CE_Card[] AllCardsDB => gameCardsDB.AllCards;
    public int DeckCount => availableCards.Count;
    public bool IsReady { get; private set; } = false;

    private void Start()
    {
        LoadDB();
    }
    void LoadDB()
    {
        TextAsset _data = Resources.Load<TextAsset>("Data/cards");
        if (!_data) return;
        gameCardsDB = JsonUtility.FromJson<CE_CardsDB>(_data.text);
        if (!gameCardsDB.IsValid) return;

        Sprite[] _rooms = Resources.LoadAll<Sprite>("UI/room_cards");
        Sprite[] _character = Resources.LoadAll<Sprite>("UI/suspect_cards");
        Sprite[] _weapons = Resources.LoadAll<Sprite>("UI/weapon_cards");

        ApplyPictureData(_rooms, CardType.Room, gameCardsDB);
        ApplyPictureData(_character, CardType.Character, gameCardsDB);
        ApplyPictureData(_weapons, CardType.Weapon, gameCardsDB);

        availableCards = new List<CE_Card>(gameCardsDB.AllCards);
        IsReady = true;
    }

    void ApplyPictureData(Sprite[] _sprite, CardType _type, CE_CardsDB _db)
    {
        if (_sprite == null) return;
        List<CE_Card> _cards = _db.AllCards.ToList().Where(c => c.Type == _type).ToList();
        _cards.ForEach(c => c.SetSprite(_sprite));
    }

    public CE_Card GetCard(int _id)
    {
        return AllCardsDB.Where(c => c.ID == _id).FirstOrDefault();
    }


    public CE_Card PickRandomCard(CardType _type)
    {
        List<CE_Card> _cards = availableCards.Where(c => c.Type == _type).ToList();
        if (_cards == null) return null;

        int _randomIndex = UnityEngine.Random.Range(0, _cards.Count);
        CE_Card _card = _cards[_randomIndex];
        availableCards.Remove(_card);

        return _card;
    }
    CE_Card PickRandomCard()
    {
        if (!gameCardsDB.IsValid) return null;

        int _randomIndex = UnityEngine.Random.Range(0, availableCards.Count);
        CE_Card _card = availableCards[_randomIndex];
        availableCards.Remove(_card);

        return _card;
    }

    public void GiveCard(IGamePlayable _character)
    {
        CE_Card _card = PickRandomCard();
        _character.HandCards.AddCard(_card);
        _character.NoteSystem.MatchCard(_card.ID);
    }

}


[System.Serializable]
public struct CE_CardsDB
{
    [SerializeField] CE_Card[] Cards;
    public CE_Card[] AllCards => Cards;
    public bool IsValid => Cards != null;
}
