using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CE_CardUIComponent : MonoBehaviour
{
    [SerializeField] CE_Card card = null;
    [SerializeField] Image image = null;


    public void Init(CE_Card _card)
    {
        card = _card;
        image.sprite = card.Picture;
    }




}
