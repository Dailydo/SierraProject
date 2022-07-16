using UnityEngine;

public class WorldComponent : MonoBehaviour
{
    [SerializeField]
    private GameObject m_playerPrefab = null;

    [SerializeField]
    private GridComponent m_grid = null;

    [SerializeField]
    private GameObject m_victoryTextGO = null;

    private PlayerComponent m_playerInstance = null;

    // Start is called before the first frame update
    void Start()
    {
        m_grid.InitCells();
        InstantiatePlayer();
        InitVictoryText();
    }

    void InstantiatePlayer()
    {
        GameObject characterGO = Instantiate<GameObject>(m_playerPrefab);
        if (characterGO != null)
        {
            m_playerInstance = characterGO.GetComponent<PlayerComponent>();
            m_playerInstance.transform.parent = transform;

            Cell cell = m_grid.GetSpecificCell(ECellEffect.PlayerSpawnPoint);
            if (cell != null)
                SetCharacterPos(m_playerInstance, cell.PosX, cell.PosY);
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
            MovePlayer(-1, 0);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            MovePlayer(1, 0);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            MovePlayer(0, 1);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            MovePlayer(0, -1);
        }
    }

    void MovePlayer(int moveX, int moveY)
    {
        int posX = Mathf.Clamp(m_playerInstance.PosX + moveX, 0, m_grid.Width - 1);
        int posY = Mathf.Clamp(m_playerInstance.PosY + moveY, 0, m_grid.Height - 1);
        if (posX == m_playerInstance.PosX && posY == m_playerInstance.PosY)
            return;

        Cell cell = m_grid.GetCell(posX, posY);
        if (cell == null)
        {
            Debug.LogWarning("Invalid cell(" + posX.ToString() + ", " + posY.ToString() + ") in grid(" + m_grid.Width.ToString() +", " + m_grid.Height.ToString() + ")");
            return;
        }

        if (!cell.Walkable)
            return;

        SetCharacterPos(m_playerInstance, posX, posY);
        OnCharacterEnteredCell(cell);
    }

    void SetCharacterPos(CharacterComponent character, int posX, int posY)
    {
        character.transform.position = m_grid.GetWorldPosition(posX, posY);
        character.PosX = posX;
        character.PosY = posY;
    }

    void OnCharacterEnteredCell(Cell cell)
    {
        if (cell.Effect == ECellEffect.Victory)
        {
            m_victoryTextGO.SetActive(true);
        }
    }
}
