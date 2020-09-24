using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMove 
{
    CE_Cell CurrentCell { get; }

    CE_CellNavigation Navigation { get; }

    bool IsMoving { get; }
    bool CanMove { get; }
    int NavigationDiceValue { get; }

    void StartNavigation();
    void SetNavigation(CE_Cell _cell, bool _action);
    IEnumerator Move();
}
