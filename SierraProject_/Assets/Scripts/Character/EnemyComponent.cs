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

    private int m_xAggroRadius = 4;
    private int m_yAggroRadius = 4;

    private int m_lastTargetPosX = 0;
    private int m_lastTargetPosY = 0;

    class InondationMapCell
    {
        public int m_x = 0;
        public int m_y = 0;
    }

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

        // TODO remove target cell from pathfinding
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
                TryComputePathToTargetCell(m_target.PosX, m_target.PosY);
                m_lastTargetPosX = m_target.PosX;
                m_lastTargetPosY = m_target.PosY;
            }
            else
            {
                if (m_pendingPathfindCell.Count > 0)
                {
                    m_targetCell = m_pendingPathfindCell.Dequeue();
                }
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
    }

    private void TryComputePathToTargetCell(int targetPosX, int targetPosY)
    {
        if (Mathf.Abs(m_target.PosX - PosX) <= m_xAggroRadius || Mathf.Abs(m_target.PosY - PosY) <= m_yAggroRadius)
        {
            /*
            m_pendingPathfindCell.Clear();

            int xSize = m_xAggroRadius * 2 + 1;
            int ySize = m_yAggroRadius * 2 + 1;

            int[,] inondationMap = new int[xSize, ySize];
            for (int x = 0; x < xSize; x++)
            {
                for(int y = 0; y < ySize; y++)
                {
                    inondationMap[x, y] = -1;
                }
            }

            int targetXPosInGrid = m_target.PosX - (PosX - m_xAggroRadius);
            int targetYPosInGrid = m_target.PosY - (PosY - m_yAggroRadius);
            if (targetXPosInGrid >= 0 && targetYPosInGrid >= 0)
            {
                inondationMap[targetXPosInGrid, targetYPosInGrid] = 0;

                InondationMapCell inondationMapCell = new InondationMapCell();
                inondationMapCell.m_x = targetXPosInGrid;
                inondationMapCell.m_y = targetYPosInGrid;

                Queue<Cell> pendingInondationCell = new Queue<Cell>();
                pendingInondationCell.Queue(inondationMapCell);

            }
            */
        }
    }

}
