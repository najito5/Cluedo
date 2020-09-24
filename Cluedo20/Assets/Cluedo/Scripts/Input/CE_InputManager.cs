using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CE_InputManager : Singleton<CE_InputManager>
{
    public static event Action<bool> OnLeftMouseDown = null;
    [SerializeField] bool leftMouseInputDown = false;

    public bool GetLeftMouseInputDown => leftMouseInputDown = Input.GetKeyDown(KeyCode.Mouse0);

    private void Update()
    {
        OnLeftMouseDown?.Invoke(GetLeftMouseInputDown);
    }
}
