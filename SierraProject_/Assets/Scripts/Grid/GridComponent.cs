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

    void Awake()
    {
        if (m_cells == null)
        {
            m_cells = new Cell[m_width * m_height];
        }
    }

    void Update()
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

        return m_cells[x + y * m_height];
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

    private Vector3 GetCellPosition(int x, int y)
    {
        return new Vector3(x, y) * m_cellSize + transform.position;
    }
}
