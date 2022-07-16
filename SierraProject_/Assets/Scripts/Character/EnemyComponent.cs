using System.Collections.Generic;
using UnityEngine;

public class EnemyComponent : CharacterComponent
{
    [SerializeField]
    private float m_moveDelayInSeconds = 1.0f;

    private GridComponent m_grid = null;
    private PlayerComponent m_target = null;
    private Cell m_targetCell = null;
    private float m_timeBeforeNextMove = 0.0f;

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

        m_timeBeforeNextMove = m_moveDelayInSeconds;
        ComputeTargetCell();
    }

    public bool CanMove()
    {
        return m_timeBeforeNextMove <= 0.0f && m_targetCell != null;
    }

    public void OnMove()
    {
        m_timeBeforeNextMove = m_moveDelayInSeconds;
        // remove target cell from pathfinding
        m_targetCell = null;
    }

    private void Update()
    {
        if (m_timeBeforeNextMove > 0.0f)
        {
            m_timeBeforeNextMove -= Time.deltaTime;
        }

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
