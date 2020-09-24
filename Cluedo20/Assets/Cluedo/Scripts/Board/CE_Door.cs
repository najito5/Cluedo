using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CE_Door : MonoBehaviour
{
    [SerializeField] CE_Room connectedRoom = null;

    public CE_Room ConnectedRoom => connectedRoom;

    public CE_Cell Cell { get; private set; }

    public Vector3 Position => transform.position;

    public bool IsValid => connectedRoom;

    private void Start()
    {
        Init();
    }
    void Init()
    {
        Cell = GetComponent<CE_Cell>();
        if (!IsValid) return;
        connectedRoom.AddDoorLink(this);
    }




    private void OnDrawGizmos()
    {
        if (!IsValid) return;
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(Position, connectedRoom.Position);
    }
}
