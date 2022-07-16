using System;
using UnityEngine;

public class GridPainterComponent : MonoBehaviour
{
    public GridComponent m_gridToProcess = null;

    [Serializable]
    public enum EPaintType 
    {
        Empty,
        Obstacle,
        PlayerSpawn,
        EnemySpawn,
        Exit,
    }

    [Serializable]
    public class AllPaintingRequest
    {
        public bool m_paintAll = false;
        public EPaintType m_paintType = EPaintType.Empty;
    }

    [Serializable]
    public class UnitPaintingRequest
    {
        public int m_cellIndex = -1;
        public EPaintType m_paintType = EPaintType.Empty;
    }

    [Serializable]
    public class LinePaintingRequest
    {
        public int m_startCellIndex = -1;
        public int m_endCellIndex = -1;
        public EPaintType m_paintType = EPaintType.Empty;
    }

    public AllPaintingRequest m_allRequest;
    public UnitPaintingRequest[] m_unitRequestList;
    public LinePaintingRequest[] m_horizontalRequestList;
    public LinePaintingRequest[] m_verticalRequestList;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ProcessPaintRequests()
    {
        ProcessAllPaintRequest();
        ProcessUnitPaintRequest();
        ProcessHorizontalPaintRequest();
        ProcessVerticalPaintRequest();
    }

    private void ProcessAllPaintRequest()
    {
        if (m_gridToProcess != null && m_allRequest.m_paintAll == true)
        {
            for(int x = 0; x < m_gridToProcess.Width; ++x)
            {
                for(int y = 0; y < m_gridToProcess.Height; ++y)
                {
                    Cell cell = m_gridToProcess.GetCell(x,y);
                    PaintCell(cell, m_allRequest.m_paintType);
                }
            }
        }
    }

    private void ProcessUnitPaintRequest()
    {
        if (m_gridToProcess != null)
        {
            foreach(UnitPaintingRequest unit in m_unitRequestList)
            {
                if (unit.m_cellIndex >= 0)
                {
                    Cell cell = m_gridToProcess.GetCell(unit.m_cellIndex);
                    if (cell != null)
                    {
                        PaintCell(cell, unit.m_paintType);
                    }
                }
            }
        }
    }

    private void ProcessHorizontalPaintRequest()
    {
    }

    private void ProcessVerticalPaintRequest()
    {
    }

    private void PaintCell(Cell cell, EPaintType paintType)
    {
        if (cell != null)
        {
            switch(paintType)
            {
                case EPaintType.Empty:
                cell.Effect = ECellEffect.None;
                break;

                case EPaintType.Obstacle:
                cell.Effect = ECellEffect.Obstacle;
                break;

                case EPaintType.PlayerSpawn:
                cell.Effect = ECellEffect.Obstacle;
                break;

                case EPaintType.EnemySpawn:
                cell.Effect = ECellEffect.EnemySpawnPoint;
                break;

                case EPaintType.Exit:
                cell.Effect = ECellEffect.Victory;
                break;
            }
        }
    }
}
