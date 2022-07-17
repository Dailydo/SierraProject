using System.Collections.Generic;
using UnityEngine;

public class EnemyComponent : CharacterComponent
{
    [SerializeField]
    private EPlane m_lethalPlane = EPlane.Flesh;

    private GridComponent m_grid = null;
    private PlayerComponent m_target = null;
    private Cell m_targetCell = null;
    private Queue<Cell> m_pendingPathfindCell = new Queue<Cell>();
    public int m_aggroDistanceInTile = 8;

    private int m_lastTargetPosX = 0;
    private int m_lastTargetPosY = 0;

    public Cell TargetCell
    {
        get { return m_targetCell; }
    }

    public EPlane LethalPlane
    {
        get { return m_lethalPlane; }
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
        m_targetCell = null;
    }

    protected override void UpdateInternal()
    {
        base.UpdateInternal();
        ComputeTargetCell();
        DebugDrawToTargetCell();
    }

    private void ComputeTargetCell()
    {
        if (m_targetCell == null)
        {
            if (m_target.PosX != m_lastTargetPosX || m_target.PosY != m_lastTargetPosY)
            {
                TryComputePathToTargetCell();
                m_lastTargetPosX = m_target.PosX;
                m_lastTargetPosY = m_target.PosY;
            }

            if (m_pendingPathfindCell.Count > 0)
            {
                m_targetCell = m_pendingPathfindCell.Dequeue();
            }

            // Idle random move
            if (m_targetCell == null)
            {
                List<Cell> walkableCells = m_grid.GetWalkableNeighbours(PosX, PosY);
                if (walkableCells.Count > 0)
                {
                    int idx = Random.Range(0, walkableCells.Count);
                    m_targetCell = walkableCells[idx];
                }
            }
        }
    }

    private void DebugDrawToTargetCell()
    {
        if (m_targetCell != null && m_grid != null && gameObject != null)
        {
            Vector3 targetCellWorldPos = m_grid.GetWorldPosition(m_targetCell.PosX, m_targetCell.PosY);
            Debug.DrawLine(gameObject.transform.position, targetCellWorldPos, Color.green, Time.deltaTime);
        }

        if (m_pendingPathfindCell != null && m_grid != null && gameObject != null)
        {
            bool previousPositionSet = false;
            Vector3 previousPosition = new Vector3();
            Vector3 currentPosition = new Vector3();
            foreach(Cell cell in m_pendingPathfindCell)
            {
                if (cell != null)
                {
                    currentPosition = m_grid.GetWorldPosition(cell.PosX, cell.PosY);
                }

                if (previousPositionSet == true)
                {
                    Debug.DrawLine(previousPosition, currentPosition, Color.blue, Time.deltaTime);
                }

                previousPosition = currentPosition;
                previousPositionSet = true;
            }
        }
    }

    private void TryComputePathToTargetCell()
    {
        if (m_target != null)
        {
            m_target.GetPathToPlayer(PosX, PosY, m_pendingPathfindCell, m_aggroDistanceInTile);
        }
    }

}
