using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CE_CardButtonComponent : MonoBehaviour
{
    [SerializeField] CE_Card card = null;
    [SerializeField] Image image = null;
    [SerializeField] Button button = null;


    public void Init(CE_Card _card, System.Action<CE_Card> _callBack, bool _isChecked)
    {
        card = _card;
        image.sprite = card.Picture;
        if (_isChecked) image.color = Color.red;

        button.onClick.AddListener(() => _callBack?.Invoke(_card));
    }

}
