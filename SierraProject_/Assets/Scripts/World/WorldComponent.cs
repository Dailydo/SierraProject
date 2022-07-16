using UnityEngine;

public class WorldComponent : MonoBehaviour
{
    [SerializeField]
    private GameObject m_characterPrefab = null;

    [SerializeField]
    private GridComponent m_grid = null;

    [SerializeField]
    private GameObject m_victoryTextGO = null;

    private CharacterComponent m_characterInstance = null;

    // Start is called before the first frame update
    void Start()
    {
        m_grid.InitCells();
        InstantiateCharacter();
        InitVictoryText();
    }

    void InstantiateCharacter()
    {
        GameObject characterGO = Instantiate<GameObject>(m_characterPrefab);
        if (characterGO != null)
        {
            m_characterInstance = characterGO.GetComponent<CharacterComponent>();
            m_characterInstance.transform.parent = transform;

            Cell cell = m_grid.GetSpecificCell(ECellEffect.PlayerSpawnPoint);
            if (cell != null)
                SetCharacterPos(cell.PosX, cell.PosY);
        }
        else
        {
            Debug.LogError("Cannot instantiate character prefab");
        }
    }

    void InitVictoryText()
    {
        if (m_victoryTextGO != null)
        {
            m_victoryTextGO.SetActive(false);
        }
        else
        {
            Debug.LogError("No victory text GO filled");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            MoveCharacter(-1, 0);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            MoveCharacter(1, 0);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            MoveCharacter(0, 1);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            MoveCharacter(0, -1);
        }
    }

    void MoveCharacter(int moveX, int moveY)
    {
        int posX = Mathf.Clamp(m_characterInstance.PosX + moveX, 0, m_grid.Width - 1);
        int posY = Mathf.Clamp(m_characterInstance.PosY + moveY, 0, m_grid.Height - 1);
        if (posX == m_characterInstance.PosX && posY == m_characterInstance.PosY)
            return;

        Cell cell = m_grid.GetCell(posX, posY);
        if (cell == null)
        {
            Debug.LogWarning("Invalid cell(" + posX.ToString() + ", " + posY.ToString() + ") in grid(" + m_grid.Width.ToString() +", " + m_grid.Height.ToString() + ")");
            return;
        }

        if (!cell.Walkable)
            return;

        SetCharacterPos(posX, posY);
        OnCharacterEnteredCell(cell);
    }

    void SetCharacterPos(int posX, int posY)
    {
        m_characterInstance.transform.position = m_grid.GetWorldPosition(posX, posY);
        m_characterInstance.PosX = posX;
        m_characterInstance.PosY = posY;
    }

    void OnCharacterEnteredCell(Cell cell)
    {
        if (cell.Effect == ECellEffect.Victory)
        {
            m_victoryTextGO.SetActive(true);
        }
    }
}
