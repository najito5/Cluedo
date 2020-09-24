using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CE_CellNavigation
{
    public CE_Board BoardCells { get; private set; } = null;

    public CE_Cell StartCell { get; private set; } = null;
    public CE_Cell EndCell { get; private set; } = null;

    public CE_Cell[] Path { get; private set; } = null;
    public bool PathCompleted { get; private set; } = false;

    public void ComputePath(CE_Board _board, CE_Cell _start, CE_Cell _end)
    {
        if (!_start || !_end) return;
        BoardCells = _board;
        _board.AllBoardCells.ForEach(c =>
            {
                c.H = float.MaxValue;
                c.G = float.MaxValue;
            });

        StartCell = _start;
        EndCell = _end;
        List<CE_Cell> _open = new List<CE_Cell>();
        List<CE_Cell> _close = new List<CE_Cell>();
        _start.G = 0;
        _open.Add(_start);
        while (_open.Count != 0)
        {
            CE_Cell _current = _open[0];
            if (_current == _end)
            {
                Path = GetFinalPath(_start, _end);
                break;
            }

            _open.Remove(_current);
            _close.Add(_current);
            for (int i = 0; i < _current.Successors.Count; i++)
            {
                float _g = _current.G + Vector3.Distance(_current.Position, _current.Successors[i].Position);
                if (_g < _current.Successors[i].G)
                {
                    _current.Successors[i].Predecessor = _current;
                    _current.Successors[i].G = _g;
                    _current.Successors[i].H = Vector3.Distance(_current.Successors[i].Position, _end.Position);
                    if (!_open.Contains(_current.Successors[i]) && _current.Successors[i].IsNavigable)
                        _open.Add(_current.Successors[i]);
                }
            }
        }

    }

    CE_Cell[] GetFinalPath(CE_Cell _start, CE_Cell _end)
    {
        List<CE_Cell> _path = new List<CE_Cell>();
        CE_Cell _current = _end;

        _path.Add(_current);
        while (_current != _start)
        {
            _current = _current.Predecessor;
            _path.Add(_current);
        }
        _path.Reverse();
        PathCompleted = _path != null && _path.Count > 0;
        return _path.ToArray();

    }

    public void Reset()
    {
        StartCell = null;
        EndCell = null;
        Path = null;
        
        BoardCells.AllBoardCells.ForEach(c =>
        {
            c.H = float.MaxValue;
            c.G = float.MaxValue;
        });
    }

}
