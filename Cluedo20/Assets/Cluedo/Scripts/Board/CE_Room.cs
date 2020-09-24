using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CE_Room : MonoBehaviour
{
    [SerializeField] int id = 0;
    [SerializeField] string roomName = "room";
    [SerializeField] Color roomColor = Color.white;
    [SerializeField] List<CE_Door> roomDoors = new List<CE_Door>();

    public int ID => id;
    public string Name => roomName;
    public Vector3 Position => transform.position;
    public Color RoomColor => roomColor;


    private void Start()
    {
        CE_Board.Instance?.AddRoom(this);
    }

    public CE_Door GetNearestDoor(IGamePlayable _char)
    {
        return roomDoors.OrderBy(d => Vector3.Distance( _char.CharacterRef.CharacterTransform.position, d.Position)).ToList().FirstOrDefault();
    }

    public void AddDoorLink(CE_Door _door) => roomDoors.Add(_door);


    private void OnDrawGizmos()
    {
        Gizmos.color = roomColor;
        Gizmos.DrawSphere(Position, 1);
    }

}
