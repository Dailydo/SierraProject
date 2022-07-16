using System.Collections.Generic;
using UnityEngine;

public class WorldComponent : MonoBehaviour
{
    [SerializeField]
    private GameObject m_playerPrefab = null;

    [SerializeField]
    private GameObject m_enemyPrefab = null;

    [SerializeField]
    private GridComponent m_grid = null;

    [SerializeField]
    private GameObject m_victoryTextGO = null;

    [SerializeField]
    private GameObject m_defeatTextGO = null;

    private PlayerComponent m_playerInstance = null;
    private List<EnemyComponent> m_enemiesInstances = new List<EnemyComponent>();

    // Start is called before the first frame update
    void Start()
    {
        m_grid.InitCells();
        InstantiatePlayer();
        InitVictoryText();
        InitDefeatText();

        // TEMP test
        InstantiateEnemy();
    }

    void InstantiatePlayer()
    {
        GameObject characterGO = Instantiate(m_playerPrefab);
        if (characterGO != null)
        {
            m_playerInstance = characterGO.GetComponent<PlayerComponent>();
            if (m_playerInstance != null)
            {
                m_playerInstance.transform.parent = transform;

                Cell spawnPoint = m_grid.GetSpecificCell(ECellEffect.PlayerSpawnPoint);
                if (spawnPoint != null)
                {
                    SetCharacterPos(m_playerInstance, spawnPoint);
                }
                else
                {
                    Debug.LogError("Cannot find player spawn point");
                }
            }
            else
            {
                Debug.LogError("No PlayerComponent on player");
            }
        }
        else
        {
            Debug.LogError("Cannot instantiate player prefab");
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

    void InitDefeatText()
    {
        if (m_defeatTextGO != null)
        {
            m_defeatTextGO.SetActive(false);
        }
        else
        {
            Debug.LogError("No defeat text GO filled");
        }
    }

    void InstantiateEnemy()
    {
        GameObject enemyGO = Instantiate(m_enemyPrefab);
        if (enemyGO != null)
        {
            EnemyComponent enemyInstance = enemyGO.GetComponent<EnemyComponent>();
            if (enemyInstance != null)
            {
                enemyInstance.transform.parent = transform; // later, will be the proper grid

                Cell spawnPoint = m_grid.GetSpecificCell(ECellEffect.EnemySpawnPoint);
                if (spawnPoint != null)
                {
                    SetCharacterPos(enemyInstance, spawnPoint);
                    m_enemiesInstances.Add(enemyInstance);
                }
                else
                {
                    Destroy(enemyGO);
                    Debug.LogError("Cannot find enemy spawn point");
                }
            }
            else
            {
                Debug.LogError("No EnemyComponent on enemy");
            }
        }
        else
        {
            Debug.LogError("Cannot instantiate enemy prefab");
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

        SetCharacterPos(m_playerInstance, cell);
    }

    void SetCharacterPos(CharacterComponent character, Cell cell)
    {
        Cell previousCell = m_grid.GetCell(character.PosX, character.PosY);
        if (previousCell != null)
            OnCharacterLeftCell(character, previousCell);

        character.transform.position = m_grid.GetWorldPosition(cell.PosX, cell.PosY);
        character.PosX = cell.PosX;
        character.PosY = cell.PosY;

        OnCharacterEnteredCell(character, cell);
    }

    void OnCharacterEnteredCell(CharacterComponent character, Cell cell)
    {
        if (character == m_playerInstance)
        {
            // check cell danger
            if (cell.IsLetal)
            {
                m_defeatTextGO.SetActive(true);
            }

            // check victory condition
            if (cell.Effect == ECellEffect.Victory)
            {
                m_victoryTextGO.SetActive(true);
            }
        }
        else
        {
            // check defeat condition
            if (AreOnSameCell(character, m_playerInstance))
            {
                m_defeatTextGO.SetActive(true);
            }

            // update cell danger
            cell.IsLetal = true;
        }
    }

    void OnCharacterLeftCell(CharacterComponent character, Cell cell)
    {
        if (character is EnemyComponent)
        {
            // remove cell danger
            cell.IsLetal = false;
        }
    }

    bool AreOnSameCell(CharacterComponent character, CharacterComponent other)
    {
        return character.PosX == other.PosX && character.PosY == other.PosY;
    }
}
