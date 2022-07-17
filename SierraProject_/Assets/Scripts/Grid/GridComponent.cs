using System.Collections.Generic;
using TMPro;
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

    [SerializeField]
    private GameObject m_cellDebugTextPrefab = null;

    [SerializeField]
    private GameObject m_canvasGO = null;

    [SerializeField]
    private bool m_instantiateDebugText = false;

    [SerializeField]
    private bool m_renderGridDebug = true;

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
        if (m_cells != null && m_renderGridDebug)
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

                        if (cell != null && cell.EnemySpawnPoint)
                        {
                            Debug.DrawLine(GetCellPosition(x, y + 1), GetCellPosition(x + 1, y), Color.black, Time.deltaTime);
                            Debug.DrawLine(GetCellPosition(x, y), GetCellPosition(x + 1, y + 1), Color.black, Time.deltaTime);
                        }

                        if (cell != null && cell.PlayerSpawnPoint)
                        {
                            Debug.DrawLine(GetCellPosition(x, y + 1), GetCellPosition(x + 1, y), Color.blue, Time.deltaTime);
                            Debug.DrawLine(GetCellPosition(x, y), GetCellPosition(x + 1, y + 1), Color.blue, Time.deltaTime);
                        }
                    }

                    if (m_instantiateDebugText)
                        InstantiateCellDebugText(x, y);
                }
            }

            m_instantiateDebugText = false;
        }
    }

    public Cell GetCell(int x, int y)
    {
        if (!IsValidPosition(x, y))
            return null;

        return m_cells[GetCellGlobalIndexFromXYIndex(x, y)];
    }

    public Cell FindCellFromWorldPosition(Vector3 worldPosition)
    {
        Vector3 localPosition = transform.InverseTransformPoint(worldPosition);
        return GetCell(Mathf.FloorToInt(localPosition.x * m_cellSize), Mathf.FloorToInt(localPosition.y * m_cellSize));
    }

    public Cell GetCell(int globalIndex)
    {
        if (!IsValidPositionForGlobalIndex(globalIndex))
            return null;

        return m_cells[globalIndex];
    }

    public int GetCellGlobalIndexFromXYIndex(int x, int y)
    {
        if (!IsValidPosition(x, y))
            return -1;
        return x + y * m_width;
    }

    public bool GetCellXYIndexFromGlobalIndex(int globalIndex, out int xIndex, out int yIndex)
    {
        xIndex = -1;
        yIndex = -1;
        if (globalIndex < 0 || globalIndex >= m_width * m_height)
            return false;
        xIndex = globalIndex % m_width;
        yIndex = globalIndex / m_width;
        return true;
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

    public IngredientComponent GetCloseIngredient(int x, int y)
    {
        IngredientComponent ingredient = GetIngredientAt(x + 1, y);
        if (ingredient != null)
            return ingredient;

        ingredient = GetIngredientAt(x - 1, y);
        if (ingredient != null)
            return ingredient;

        ingredient = GetIngredientAt(x, y + 1);
        if (ingredient != null)
            return ingredient;

        ingredient = GetIngredientAt(x, y - 1);
        if (ingredient != null)
            return ingredient;

        return null;
    }

    public IngredientComponent GetIngredientAt(int x, int y)
    {
        Cell cell = GetCell(x, y);
        if (cell == null)
            return null;

        return cell.InnerIngredient;
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        if (!IsValidPosition(x, y))
            return Vector3.zero;

        return GetCellPosition(x, y) + new Vector3(m_cellSize * 0.5f, m_cellSize * 0.5f, 0.0f);
    }

    public bool IsValidPosition(int x, int y)
    {
        return x >= 0 && x < m_width && y >= 0 && y < m_height;
    }

    public bool IsValidPositionForGlobalIndex(int globalIndex)
    {
        return globalIndex >= 0 && globalIndex < m_width * m_height;
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
        return new Vector3(x, y, 0.0f) * m_cellSize + transform.position;
    }

    private void InstantiateCellDebugText(int x, int y)
    {
        GameObject textGO = Instantiate(m_cellDebugTextPrefab, GetWorldPosition(x, y), Quaternion.identity, m_canvasGO.transform);
        if (textGO != null)
        {
            int idx = GetCellGlobalIndexFromXYIndex(x, y);

            textGO.name = "CellDebugText -> " + idx.ToString();

            TextMeshProUGUI txtMesh = textGO.GetComponent<TextMeshProUGUI>();
            if (txtMesh != null)
                txtMesh.text = idx.ToString();
        }
    }
}
