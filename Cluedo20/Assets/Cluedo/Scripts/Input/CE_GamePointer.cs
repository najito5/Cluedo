using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CE_GamePointer : Singleton<CE_GamePointer>
{
    public event Action<CE_Cell, bool> OnCellHit = null;

    [SerializeField] LayerMask cellsLayer = 0;

    private void Start()
    {
        CE_InputManager.OnLeftMouseDown += SetEndCell;
    }

    void SetEndCell(bool _action)
    {
        if (!_action) return;
        Ray _ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
        RaycastHit _hit;
        bool _cell = Physics.Raycast(_ray, out _hit, 200, cellsLayer);
        if (!_cell) return;
        OnCellHit?.Invoke(_hit.collider.GetComponent<CE_Cell>(), _action);
    }
}
