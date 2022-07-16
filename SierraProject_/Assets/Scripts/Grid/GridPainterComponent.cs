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
        if (m_gridToProcess != null)
        {
            foreach(LinePaintingRequest line in m_horizontalRequestList)
            {
                if (line.m_startCellIndex >= 0 && line.m_endCellIndex >= 0)
                {
                    int finalYIndex = -1;
                    int finalXStartIndex = -1;
                    int finalXEndIndex = -1;

                    int startXIndex = -1;
                    int startYIndex = -1;
                    int endXIndex = -1;
                    int endYIndex = -1;
                    if(m_gridToProcess.GetCellXYIndexFromGlobalIndex(line.m_startCellIndex, out startXIndex, out startYIndex) && m_gridToProcess.GetCellXYIndexFromGlobalIndex(line.m_endCellIndex, out endXIndex, out endYIndex))
                    {
                        Debug.Log("Horinzontal Line Start X " + startXIndex + " Y " + startYIndex);
                        Debug.Log("Horinzontal Line End X " + endXIndex + " Y " + endYIndex);

                        if(startYIndex == endYIndex)
                        {
                            finalYIndex = startYIndex;
                            if (startXIndex < endXIndex)
                            {
                                finalXStartIndex = startXIndex;
                                finalXEndIndex = endXIndex;
                            }
                            else
                            {
                                finalXStartIndex = endXIndex;
                                finalXEndIndex = startXIndex;    
                            }
                        }
                    }

                    if (finalYIndex >= 0)
                    {
                        Debug.Log("Final Horinzontal Line Start X " + finalXStartIndex + " End X " + finalXEndIndex + " Y " + endYIndex);
                        for(int x = finalXStartIndex; x <= finalXEndIndex; ++x)
                        {
                            Cell cell = m_gridToProcess.GetCell(x, finalYIndex);
                            if (cell != null)
                            {
                                PaintCell(cell, line.m_paintType);
                            }
                        }
                    }
                    else
                    {
                        // ERROR
                        Debug.Log("Error with line " + line.m_startCellIndex + " " + line.m_endCellIndex);
                    }
                }
            }
        }
    }

    private void ProcessVerticalPaintRequest()
    {
        if (m_gridToProcess != null)
        {
            foreach(LinePaintingRequest line in m_verticalRequestList)
            {
                if (line.m_startCellIndex >= 0 && line.m_endCellIndex >= 0)
                {
                    int finalXIndex = -1;
                    int finalYStartIndex = -1;
                    int finalYEndIndex = -1;

                    int startXIndex = -1;
                    int startYIndex = -1;
                    int endXIndex = -1;
                    int endYIndex = -1;
                    if(m_gridToProcess.GetCellXYIndexFromGlobalIndex(line.m_startCellIndex, out startXIndex, out startYIndex) && m_gridToProcess.GetCellXYIndexFromGlobalIndex(line.m_endCellIndex, out endXIndex, out endYIndex))
                    {
                        Debug.Log("Vertical Line Start X " + startXIndex + " Y " + startYIndex);
                        Debug.Log("Vertical Line End X " + endXIndex + " Y " + endYIndex);

                        if(startXIndex == endXIndex)
                        {
                            finalXIndex = startXIndex;
                            if (startYIndex < endYIndex)
                            {
                                finalYStartIndex = startYIndex;
                                finalYEndIndex = endYIndex;
                            }
                            else
                            {
                                finalYStartIndex = endYIndex;
                                finalYEndIndex = startYIndex;    
                            } 
                        }
                    }

                    if (finalXIndex >= 0)
                    {
                        Debug.Log("Final Vertical Line X " + finalXIndex + " Start Y " + finalYStartIndex + " End Y " + finalYEndIndex);
                        for(int y = finalYStartIndex; y <= finalYEndIndex; ++y)
                        {
                            Cell cell = m_gridToProcess.GetCell(finalXIndex, y);
                            if (cell != null)
                            {
                                PaintCell(cell, line.m_paintType);
                            }
                        }
                    }
                    else
                    {
                        // ERROR
                        Debug.Log("Error with line " + line.m_startCellIndex + " " + line.m_endCellIndex);
                    }
                }
            }
        }
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
