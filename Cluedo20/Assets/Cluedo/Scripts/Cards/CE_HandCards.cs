using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[Serializable]
public class CE_HandCards
{
    public Func<CE_HandCards, IEnumerator> OnHandReady = null;

    [SerializeField]
    List<CE_Card> handCards = new List<CE_Card>();
    public List<CE_Card> HandCards => handCards;


    public void AddCard(CE_Card _card) => handCards.Add(_card);

    public CE_Card GetSuggestCard(CE_Suggest _suggest)
    {
        CE_Card _character = handCards.Where(c => c.ID == _suggest.Character.ID).FirstOrDefault();
        CE_Card _weapon = handCards.Where(c => c.ID == _suggest.Weapon.ID).FirstOrDefault();
        CE_Card _room = handCards.Where(c => c.ID == _suggest.Room.ID).FirstOrDefault();

        if (_character != null)
            return _character;
        if (_weapon != null)
            return _weapon;
        if (_room != null)
            return _room;
        return null;
    }

}
