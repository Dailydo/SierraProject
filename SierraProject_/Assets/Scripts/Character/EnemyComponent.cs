using System.Collections.Generic;
using UnityEngine;

public class EnemyComponent : CharacterComponent
{
    private GridComponent m_grid = null;
    private PlayerComponent m_target = null;
    private Cell m_targetCell = null;

    private int m_lastTargetPosX = 0;
    private int m_lastTargetPosY = 0;

    public Cell TargetCell
    {
        get { return m_targetCell; }
    }

    public void Init(GridComponent grid, PlayerComponent target)
    {
        m_grid = grid;
        m_target = target;

        ComputeTargetCell();
    }

    public override bool CanMoveInternal()
    {
        return m_targetCell != null;
    }

    protected override void OnMoveInternal()
    {
        base.OnMoveInternal();

        // remove target cell from pathfinding
        m_targetCell = null;
    }

    protected override void UpdateInternal()
    {
        base.UpdateInternal();

        if (m_targetCell == null)
            ComputeTargetCell();
    }

    private void ComputeTargetCell()
    {
        if (m_target.PosX != m_lastTargetPosX || m_target.PosY != m_lastTargetPosY)
        {
            // update pathfinding

            m_lastTargetPosX = m_target.PosX;
            m_lastTargetPosY = m_target.PosY;
        }
        else
        {
            // next cell in pathfinding
        }

        // TEMP random move
        List<Cell> walkableCells = m_grid.GetWalkableNeighbours(PosX, PosY);
        if (walkableCells.Count > 0)
        {
            int idx = Random.Range(0, walkableCells.Count);
            m_targetCell = walkableCells[idx];
        }
    }

}
