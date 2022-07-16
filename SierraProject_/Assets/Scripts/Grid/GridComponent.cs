using UnityEngine;

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

    void Awake()
    {
        if (m_cells == null)
        {
            m_cells = new Cell[m_width * m_height];
        }

        for (int x = 0; x < m_width; ++x)
        {
            for (int y = 0; y < m_height; ++y)
            {
                if (y + 1 < m_height)
                {
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                }

                if (x + 1 < m_width)
                {
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
                }
            }
        }  
    }

    void Update()
    {
    }

    public Cell GetCell(int x, int y)
    {
        if (x >= m_width || y >= m_height)
            return null;

        return m_cells[x + y * m_height];
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        if (x >= m_width || y >= m_height)
            return Vector3.zero;

        return new Vector3(x, y) * m_cellSize + transform.position;
    }
}
