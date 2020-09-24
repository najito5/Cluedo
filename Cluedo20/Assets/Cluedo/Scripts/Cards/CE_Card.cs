using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class CE_Card
{
    [SerializeField] string name = "";
    [SerializeField] int id = 0;
    [SerializeField] CardType type = CardType.Character;
    [NonSerialized] Sprite picture = null;

    public int ID => id;
    public string Name => name;
    public CardType Type => type;
    public Sprite Picture => picture;

    public void SetSprite(Sprite[] _sprites)
    {
        for (int i = 0; i < _sprites.Length; i++)
        {
            if (_sprites[i].name.ToLower() == name.ToLower())
                picture = _sprites[i];
        }
    }

}

public enum CardType
{
    Weapon,
    Character,
    Room
}

