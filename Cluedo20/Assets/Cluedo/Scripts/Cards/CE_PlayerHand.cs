using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CE_PlayerHand : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GridLayoutGroup layoutGroup = null;
    [SerializeField] bool expand = false;
    [SerializeField] CE_Player playerHand = null;
    [SerializeField] Image image = null;
    bool IsValid => playerHand && layoutGroup;

    public void OnPointerEnter(PointerEventData eventData)
    {
        expand = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        expand = false;
    }


    private void Start()
    {
        image.sprite = null;
        CE_GameManager.OnPlayerInit += SetPlayer;
        CE_GameManager.OnPlayerReady += GetPlayerHand;

    }

    private void Update()
    {
        if (expand)
            layoutGroup.spacing = Vector2.Lerp(new Vector2(layoutGroup.spacing.x, layoutGroup.spacing.y), new Vector2(0, layoutGroup.spacing.y), 5 * Time.deltaTime);
        else
            layoutGroup.spacing = Vector2.Lerp(new Vector2(layoutGroup.spacing.x, layoutGroup.spacing.y), new Vector2(-120, layoutGroup.spacing.y), 5 * Time.deltaTime);

    }

    public void SetPlayer(IGamePlayable _player)
    {
        playerHand = _player as CE_Player;
    }

    void GetPlayerHand()
    {
        if (!IsValid) return;
        for (int i = 0; i < playerHand.HandCards.HandCards.Count; i++)
        {
            Image _new = Instantiate(image, transform);
            _new.sprite = playerHand.HandCards.HandCards[i].Picture;
        }
    }
}
