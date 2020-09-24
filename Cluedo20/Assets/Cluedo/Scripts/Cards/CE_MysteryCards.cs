using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct CE_MysteryCards
{

    [SerializeField] CE_Card firstCard, secondCard, thirdCard;

    public CE_Card FirstCard => firstCard;
    public CE_Card SecondCard => secondCard;
    public CE_Card ThirdCard => thirdCard;

    public CE_MysteryCards(CE_Card _first, CE_Card _second, CE_Card _third)
    {
        firstCard = _first;
        secondCard = _second;
        thirdCard = _third;
    }
}
