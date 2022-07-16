using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GridComponent : MonoBehaviour
{
    [SerializeField]
    private Cell[] m_cells = null;

    [SerializeField]
    private float m_cellSize = 1.0f;

    [SerializeField]
    private int m_width = 5;

    [SerializeField]
    private int m_height = 5;

    public int Width
    {
        get { return m_width; }
    }

    public int Height
    {
        get { return m_height; }
    }

    private void Awake()
    {
        if (m_cells == null)
        {
            m_cells = new Cell[m_width * m_height];
        }
    }

    public void InitCells()
    {
        for (int i = 0; i < m_width; ++i)
        {
            for (int j = 0; j < m_height; ++j)
            {
                Cell cell = GetCell(i, j);
                if (cell != null)
                    cell.Init(i, j);
            }
        }
    }

    private void Update()
    {
        if (m_cells != null)
        {
            for (int x = 0; x <= m_width; ++x)
            {
                for (int y = 0; y <= m_height; ++y)
                {
                    if (y + 1 <= m_height)
                    {
                        Debug.DrawLine(GetCellPosition(x, y), GetCellPosition(x, y + 1), Color.white, Time.deltaTime);
                    }

                    if (x + 1 <= m_width)
                    {
                        Debug.DrawLine(GetCellPosition(x, y), GetCellPosition(x + 1, y), Color.white, Time.deltaTime);
                    }

                    if (x + 1 <= m_width && y + 1 <= m_height)
                    {
                        Cell cell = GetCell(x, y);
                        if (cell != null && !cell.Walkable)
                        {
                            Debug.DrawLine(GetCellPosition(x, y + 1), GetCellPosition(x + 1, y), Color.red, Time.deltaTime);
                            Debug.DrawLine(GetCellPosition(x, y), GetCellPosition(x + 1, y + 1), Color.red, Time.deltaTime);
                        }
                    }
                }
            }  
        }
    }

    public Cell GetCell(int x, int y)
    {
        if (!IsValidPosition(x, y))
            return null;

        return m_cells[x + y * m_width];
    }

    public List<Cell> GetWalkableNeighbours(int x, int y)
    {
        List<Cell> neighbours = new List<Cell>();
        Cell neighbour = GetCell(x + 1, y);
        if (neighbour != null && neighbour.Walkable)
            neighbours.Add(neighbour);

        neighbour = GetCell(x - 1, y);
        if (neighbour != null && neighbour.Walkable)
            neighbours.Add(neighbour);

        neighbour = GetCell(x, y + 1);
        if (neighbour != null && neighbour.Walkable)
            neighbours.Add(neighbour);

        neighbour = GetCell(x, y - 1);
        if (neighbour != null && neighbour.Walkable)
            neighbours.Add(neighbour);

        return neighbours;
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        if (!IsValidPosition(x, y))
            return Vector3.zero;

        return GetCellPosition(x, y) + new Vector3(m_cellSize * 0.5f, m_cellSize * 0.5f);
    }

    public bool IsValidPosition(int x, int y)
    {
        return x >= 0 && x < m_width && y >= 0 && y < m_height;
    }

    public Cell GetSpecificCell(ECellEffect effect)
    {
        for (int i = 0; i < m_cells.Length; ++i)
        {
            if (m_cells[i].Effect == effect)
                return m_cells[i];
        }
        return null;
    }

    private Vector3 GetCellPosition(int x, int y)
    {
        return new Vector3(x, y) * m_cellSize + transform.position;
    }
}
