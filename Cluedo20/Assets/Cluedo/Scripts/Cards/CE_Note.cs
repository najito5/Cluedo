using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class CE_Note
{
    [SerializeField] int id = 0;
    [SerializeField] string name = string.Empty;
    [SerializeField] bool isChecked = false;
    [SerializeField] CardType type = CardType.Character;

    public int ID { get => id; set => id = value; }
    public string NoteName { get => name; set => name = value; }

    public bool IsChecked { get => isChecked; set => isChecked = value; }
    public CardType Type { get => type; set => type = value; }


    public CE_Note(int _id, string _name, CardType _type)
    {
        ID = _id;
        NoteName = _name;
        Type = _type;
    }

    public override string ToString()
    {
        return $"game note => {id} {name} {isChecked}";
    }
}
