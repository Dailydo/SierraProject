using UnityEngine;

public class GridComponent : MonoBehaviour
{
    [SerializeField]
    private Cell[] m_cells;

    [SerializeField]
    private float m_cellSize = 1.0f;

    [SerializeField]
    private int m_width = 5;

    [SerializeField]
    private int m_height = 5;

    void Awake()
    {
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
