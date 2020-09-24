using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CE_Board : Singleton<CE_Board>
{
    [SerializeField] List<CE_StartCharacter> allStartPoints = new List<CE_StartCharacter>();
    public List<CE_Cell> AllBoardCells { get; private set; } = new List<CE_Cell>();

    public Dictionary<int, CE_Room> AllRooms { get; private set; } = new Dictionary<int, CE_Room>();

    public void AddCell(CE_Cell _cell)
    {
        AllBoardCells.Add(_cell);
    }

    public void AddRoom(CE_Room _room)
    {
        AllRooms.Add(_room.ID, _room);
    }

    public CE_Room GetRoom(int _id) => AllRooms[_id];

    public CE_Cell GetNearestCell(Vector3 _pos)
    {
        return AllBoardCells.OrderBy(c => Vector3.Distance(c.Position, _pos)).FirstOrDefault();
    }

    public Transform GetStartPos(CE_GameCharacters _chara) => allStartPoints.FirstOrDefault(s => s.Character == _chara).StartCell;

}
