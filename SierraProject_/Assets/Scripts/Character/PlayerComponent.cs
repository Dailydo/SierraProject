using UnityEngine;
using System.Collections.Generic;

public class PlayerComponent : CharacterComponent
{
    class InondationMapCell
    {
        public int m_x = 0;
        public int m_y = 0;
    }

    private const int HP_MAX = 2;
    private int m_hp = HP_MAX;

    private int m_xAggroRadius = 8;
    private int m_yAggroRadius = 8;
    private int m_inondationMapXSize = -1;
    private int m_inondationMapYSize = -1;
    private float m_inondationMapComputeCooldown = 2f;
    private float m_inondationMapTimer = -1f;
    private int[,] m_inondationMap = null;
    int m_playerPosXWhenInondationComputed = -1;
    int m_playerPosYWhenInondationComputed = -1;

    private GridComponent m_grid = null;

    public int HP
    {
        get { return m_hp; }
    }

    public bool IsDead
    {
        get { return HP <= 0; }
    }

    public void Init(GridComponent grid)
    {
        m_grid = grid;
    }

    protected override void OnStartInternal()
    {
        InitInondationMap();
    }

    public void TakeDamage()
    {
        --m_hp;
        if (m_hp < 0)
            m_hp = 0;

        UpdateSpriteColor();
    }

    public void Heal()
    {
        ++m_hp;
        if (m_hp > HP_MAX)
            m_hp = HP_MAX;

        UpdateSpriteColor();
    }

    private void UpdateSpriteColor()
    {
        Color col = CharacterSprite.color;
        col.g = m_hp == HP_MAX ? 1.0f : 0.5f;
        col.b = m_hp == HP_MAX ? 1.0f : 0.5f;
        CharacterSprite.color = col;
    }

    protected override void UpdateInternal()
    {
        base.UpdateInternal();
        UpdateInondationMap();
    }
 
    public void UpdateInondationMap()
    {
        m_inondationMapTimer -= Time.deltaTime;
        if (m_inondationMapTimer < 0f)
        {
            m_inondationMapTimer = m_inondationMapComputeCooldown;
            ResetInondationMap();
            ComputeInondationMap();
            DisplayDebugInondationMap();
        }
    }

    public void InitInondationMap()
    {
        if (m_inondationMap == null)
        {
            m_inondationMapXSize = m_xAggroRadius * 2 + 1;
            m_inondationMapYSize = m_yAggroRadius * 2 + 1;
            m_inondationMapTimer = m_inondationMapComputeCooldown;
            m_inondationMap = new int[m_inondationMapXSize, m_inondationMapYSize];
        }
    }

    private void ResetInondationMap()
    {
        if (m_inondationMap != null)
        {
            for(int x = 0; x < m_inondationMapXSize; ++x)
            {
                for (int y = 0; y < m_inondationMapYSize; ++y)
                {
                    m_inondationMap[x, y] = -1;
                }
            }
        }
    }

    private void ComputeInondationMap()
    {
        if (m_inondationMap != null && m_grid != null)
        {
            m_playerPosXWhenInondationComputed = PosX;
            m_playerPosYWhenInondationComputed = PosY;
            
            InondationMapCell playerCentricCell = new InondationMapCell();
            playerCentricCell.m_x = m_xAggroRadius;
            playerCentricCell.m_y = m_yAggroRadius;

            m_inondationMap[playerCentricCell.m_x, playerCentricCell.m_y] = 0;

            Queue<InondationMapCell> cellToUpdateQueue = new Queue<InondationMapCell>();
            cellToUpdateQueue.Enqueue(playerCentricCell);

            do
            {
                InondationMapCell currentProcessedCell = cellToUpdateQueue.Dequeue();
                if (currentProcessedCell != null)
                {
                    int weight = m_inondationMap[currentProcessedCell.m_x, currentProcessedCell.m_y] + 1;

                    // Gather all the neighbours and check if they must be added to the queue
                    ProcessInondationMapCellNeighbour(currentProcessedCell.m_x, currentProcessedCell.m_y -1, weight, playerCentricCell, cellToUpdateQueue);
                    ProcessInondationMapCellNeighbour(currentProcessedCell.m_x, currentProcessedCell.m_y + 1, weight, playerCentricCell, cellToUpdateQueue);
                    ProcessInondationMapCellNeighbour(currentProcessedCell.m_x - 1, currentProcessedCell.m_y, weight, playerCentricCell, cellToUpdateQueue);
                    ProcessInondationMapCellNeighbour(currentProcessedCell.m_x + 1, currentProcessedCell.m_y, weight, playerCentricCell, cellToUpdateQueue);
                }
            } while (cellToUpdateQueue.Count > 0);
        }
    }

    private void ProcessInondationMapCellNeighbour(int x, int y, int weight, InondationMapCell playerCentricCell, Queue<InondationMapCell> queue)
    {
        if (m_inondationMap != null && m_grid != null && queue != null)
        {
            // XY is in local inondation grid
            if (x >= 0 && x < m_inondationMapXSize && y >= 0 && y < m_inondationMapYSize)
            {
                // Convert into grid coordinate
                int xOffset = x - playerCentricCell.m_x;
                int yOffset = y - playerCentricCell.m_y;

                int gridXPos = PosX + xOffset;
                int gridYPos = PosY + yOffset;
                if (m_grid.IsValidPosition(gridXPos, gridYPos))
                {
                    Cell cell = m_grid.GetCell(gridXPos, gridYPos);
                    if (cell != null && cell.Walkable)
                    {
                        int currentWeightInMap = m_inondationMap[x, y];
                        if (currentWeightInMap == -1 || weight < currentWeightInMap)
                        {
                            m_inondationMap[x, y] = weight;

                            InondationMapCell cellToAddToQueue = new InondationMapCell();
                            cellToAddToQueue.m_x = x;
                            cellToAddToQueue.m_y = y;
                            queue.Enqueue(cellToAddToQueue);
                        }
                    }
                }
            }
        }
    }

    private void DisplayDebugInondationMap()
    {
        Debug.Log("Inondation Map DEBUG Size " + m_inondationMapXSize + " " + m_inondationMapYSize);
        if (m_inondationMap != null)
        {
            string debugString = new string("");
            for(int y = m_inondationMapYSize - 1; y >= 0; --y)
            {
                for(int x = 0; x < m_inondationMapXSize; ++x)
                {
                    string debugValue = new string("X");
                    if (m_inondationMap[x, y] >= 0)
                    debugValue = m_inondationMap[x, y].ToString();

                    debugString += " | " + debugValue;
                }
                debugString += "\n";
            }
            Debug.Log(debugString);
        }
    }

    public bool GetPathToPlayer(int xPos, int yPos, Queue<Cell> path, int maxWeight)
    {
        path.Clear();

        if (path != null && maxWeight > 0 && m_grid != null && m_playerPosXWhenInondationComputed >= 0 && m_playerPosYWhenInondationComputed >= 0)
        {
            int xOffset = xPos - m_playerPosXWhenInondationComputed;
            int yOffset = yPos - m_playerPosYWhenInondationComputed;

            int xPlayerPosInInondation = m_xAggroRadius;
            int yPlayerPosInInondation = m_yAggroRadius;

            // Is in inondation map patch
            Debug.Log("XOffset " + xOffset + " YOffset " + yOffset);
            if (Mathf.Abs(xOffset) <= m_xAggroRadius && Mathf.Abs(yOffset) <= m_yAggroRadius)
            {
                int currentXPosInInondation = xPlayerPosInInondation + xOffset;
                int currentYPosInInondation = yPlayerPosInInondation + yOffset;
                int currentInondationWeight = m_inondationMap[currentXPosInInondation, currentYPosInInondation];

                Debug.Log("Get path to player from enemy Pos X " + xPos + " Pos Y " + yPos + " Global Index " + m_grid.GetCellGlobalIndexFromXYIndex(xPos, yPos) + " Inondation X " + currentXPosInInondation + " Inondation Y " + currentYPosInInondation + " Inondation Weight " + currentInondationWeight + " Player Pos X " + m_playerPosXWhenInondationComputed + " Player Pos Y " + m_playerPosYWhenInondationComputed + " Player Global Index " + m_grid.GetCellGlobalIndexFromXYIndex(m_playerPosXWhenInondationComputed, m_playerPosYWhenInondationComputed));

                if (currentInondationWeight > 0 && currentInondationWeight <= maxWeight)
                {
                    // A path is existing
                    do
                    {
                        int xPosWithSmallerWeight = currentXPosInInondation;
                        int yPosWithSmallerWeight = currentYPosInInondation;
                        int smallerWeight = currentInondationWeight;

                        int neighbourX = currentXPosInInondation - 1;
                        int neighbourY = currentYPosInInondation;
                        if (neighbourX >= 0)
                        {
                            int neighbourWeight = m_inondationMap[neighbourX, neighbourY];
                            //Debug.Log("Testing Neighbour : Pos X " + neighbourX + " Pos Y " + neighbourY + " Weight " + neighbourWeight);
                            if ( neighbourWeight >= 0 && neighbourWeight < smallerWeight)
                            {
                                //Debug.Log("Neighbour Selected : Pos X " + neighbourX + " Pos Y " + neighbourY);
                                xPosWithSmallerWeight = neighbourX;
                                yPosWithSmallerWeight = neighbourY;  
                                smallerWeight =  neighbourWeight;      
                            }
                        }

                        neighbourX = currentXPosInInondation + 1;
                        neighbourY = currentYPosInInondation;
                        if (neighbourX < m_inondationMapXSize)
                        {
                            int neighbourWeight = m_inondationMap[neighbourX, neighbourY];
                            //Debug.Log("Testing Neighbour : Pos X " + neighbourX + " Pos Y " + neighbourY + " Weight " + neighbourWeight);
                            if ( neighbourWeight >= 0 && neighbourWeight < smallerWeight)
                            {
                                //Debug.Log("Neighbour Selected : Pos X " + neighbourX + " Pos Y " + neighbourY);
                                xPosWithSmallerWeight = neighbourX;
                                yPosWithSmallerWeight = neighbourY;  
                                smallerWeight =  neighbourWeight;      
                            }
                        }

                        neighbourX = currentXPosInInondation;
                        neighbourY = currentYPosInInondation - 1;
                        if (neighbourY >= 0)
                        {
                            int neighbourWeight = m_inondationMap[neighbourX, neighbourY];
                            //Debug.Log("Testing Neighbour : Pos X " + neighbourX + " Pos Y " + neighbourY + " Weight " + neighbourWeight);
                            if ( neighbourWeight >= 0 && neighbourWeight < smallerWeight)
                            {
                                //Debug.Log("Neighbour Selected : Pos X " + neighbourX + " Pos Y " + neighbourY);
                                xPosWithSmallerWeight = neighbourX;
                                yPosWithSmallerWeight = neighbourY;  
                                smallerWeight =  neighbourWeight;      
                            }
                        }

                        neighbourX = currentXPosInInondation;
                        neighbourY = currentYPosInInondation + 1;
                        if (neighbourY < m_inondationMapYSize)
                        {
                            int neighbourWeight = m_inondationMap[neighbourX, neighbourY];
                            //Debug.Log("Testing Neighbour : Pos X " + neighbourX + " Pos Y " + neighbourY + " Weight " + neighbourWeight);
                            if (neighbourWeight >= 0 && neighbourWeight < smallerWeight)
                            {
                                //Debug.Log("Neighbour Selected : Pos X " + neighbourX + " Pos Y " + neighbourY);
                                xPosWithSmallerWeight = neighbourX;
                                yPosWithSmallerWeight = neighbourY;  
                                smallerWeight =  neighbourWeight;      
                            }
                        }

                        currentXPosInInondation = xPosWithSmallerWeight;
                        currentYPosInInondation = yPosWithSmallerWeight;
                        currentInondationWeight = smallerWeight;

                        int gridXCellPos = m_playerPosXWhenInondationComputed + (currentXPosInInondation - xPlayerPosInInondation);
                        int gridYCellPos = m_playerPosYWhenInondationComputed + (currentYPosInInondation - yPlayerPosInInondation);

                        Cell cell = m_grid.GetCell(gridXCellPos, gridYCellPos);
                        if (cell != null)
                        {
                            path.Enqueue(cell);
                        }
                    }while(currentInondationWeight > 0);
                }
            }
        }
        Debug.Log("Computed path size " + path.Count + " to go to Pos X " + PosX + " Pos Y " + PosY + " from PosX " + xPos + " PosY " + yPos);
        foreach(Cell pathCell in path)
        {
            Debug.Log("Cell X " + pathCell.PosX + " Cell Y " + pathCell.PosY);
        }
        return path.Count > 0;
    }
}
