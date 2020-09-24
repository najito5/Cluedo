using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CE_Cell : MonoBehaviour
{
    [SerializeField, Range(0, 10)] float successorRange = 3;
    [SerializeField] LayerMask cellLayer = 0;

    public Vector3 Position => transform.position;
    public List<CE_Cell> Successors { get; private set; } = new List<CE_Cell>();

    public CE_Cell Predecessor { get; set; } = null;

   public float F => G + H;
    public float G { get; set; } = float.MaxValue;
    public float H{ get; set; } = float.MaxValue;

    public bool IsNavigable { get; set; } = true;

    void SetSuccessors()
    {
        Collider[] _successors = Physics.OverlapSphere(Position, successorRange, cellLayer);
        Successors = _successors.Select(s => s.GetComponent<CE_Cell>()).ToList();
        Successors.Distinct();
        Successors.Remove(this);
        CE_Board.Instance?.AddCell(this);
    }
    private void Start()
    {
        SetSuccessors();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.grey - new Color(0, 0, 0, .5f);
        Gizmos.DrawWireSphere(Position, successorRange);
        Gizmos.color = Color.green - new Color(0, 0, 0, .5f);
        Gizmos.DrawSphere(Position, .5f);
        Gizmos.color = Color.red - new Color(0, 0, 0, .4f);

        for (int i = 0; i < Successors.Count; i++)
        {
            Gizmos.DrawLine(Position, Successors[i].Position);
        }
    }
}
