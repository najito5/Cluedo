using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class CE_DoorUI : Singleton<CE_DoorUI>
{
    [SerializeField] GameObject doorGroup = null;
    [SerializeField] Button doorButton = null;
    [SerializeField] TMP_Text doorText = null;


    Action callback;
    private void Start()
    {
        doorButton.onClick.AddListener(() => callback?.Invoke());
    }

    public void UpdateDoorUI(CE_Room _room, Action _callback)
    {
        doorGroup.SetActive(true);
        doorText.text = $"Enter in {_room.Name}";
        callback = _callback;
    }

    public void DesactiveDoorUI()
    {
        doorGroup.SetActive(false);
    }



}
