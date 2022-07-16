using System.Collections.Generic;
using UnityEngine;

public enum EPlane
{
    Base = 0,
    Flesh,
    Electric,
    Ether,
    
    Count
}

public class WorldComponent : MonoBehaviour
{
    [SerializeField]
    private GameObject m_playerPrefab = null;

    [SerializeField]
    private GameObject m_enemyPrefab = null;

    [SerializeField]
    private Transform m_cameraTransform = null;

    [SerializeField]
    private GridComponent m_grid = null;

    [SerializeField]
    private GameObject m_victoryTextGO = null;

    [SerializeField]
    private GameObject m_defeatTextGO = null;

    [SerializeField]
    private float m_swapPlaneDelay = 5.0f;

    private PlayerComponent m_playerInstance = null;
    private List<EnemyComponent> m_enemiesInstances = new List<EnemyComponent>();
    private IngredientComponent[] m_ingredients;

    private EPlane m_currentPlane = EPlane.Base;
    private float m_swapPlaneCooldown;

    private bool m_victory = false;

    // Start is called before the first frame update
    void Start()
    {
        m_grid.InitCells();
        InitIngredients();
        InstantiatePlayer();
        InitVictoryText();
        InitDefeatText();

        m_swapPlaneCooldown = m_swapPlaneDelay;
        m_victory = false;

        // TEMP test
        InstantiateEnemy();
    }

    void InitIngredients()
    {
        m_ingredients = transform.GetComponentsInChildren<IngredientComponent>();
        for (int i = 0; i < m_ingredients.Length; ++i)
        {
            m_ingredients[i].Init(m_grid);
        }
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
                    SetCharacterPos(m_playerInstance, spawnPoint, true);
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
                    SetCharacterPos(enemyInstance, spawnPoint, true);
                    enemyInstance.Init(m_grid, m_playerInstance);
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

    void Update()
    {
        if (m_playerInstance.IsDead || m_victory)
            return;

        UpdateInputs();
        UpdateEnemies();
        UpdateCurrentPlane();
    }

    void UpdateInputs()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            MovePlayer(-1, 0);
        }

        if (Input.GetKey(KeyCode.D))
        {
            MovePlayer(1, 0);
        }

        if (Input.GetKey(KeyCode.Z))
        {
            MovePlayer(0, 1);
        }

        if (Input.GetKey(KeyCode.S))
        {
            MovePlayer(0, -1);
        }
    }

    void UpdateEnemies()
    {
        foreach (EnemyComponent enemy in m_enemiesInstances)
        {
            if (enemy.CanMove())
            {
                SetCharacterPos(enemy, enemy.TargetCell, false);
            }
        }
    }

    void UpdateCurrentPlane()
    {
        if (m_swapPlaneCooldown > 0.0f)
        {
            m_swapPlaneCooldown -= Time.deltaTime;
            if (m_swapPlaneCooldown <= 0.0f)
            {
                int newPlaneIdx = Random.Range(0, (int)EPlane.Count);
                SetCurrentPlane((EPlane)newPlaneIdx);

                m_swapPlaneCooldown = m_swapPlaneDelay;
            }
        }
    }

    void MovePlayer(int moveX, int moveY)
    {
        if (!m_playerInstance.CanMove())
            return;

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

        SetCharacterPos(m_playerInstance, cell, false);
    }

    void SetCharacterPos(CharacterComponent character, Cell cell, bool teleport)
    {
        Cell previousCell = m_grid.GetCell(character.PosX, character.PosY);
        if (previousCell != null)
            OnCharacterLeftCell(character, previousCell);

        Vector3 targetWorldPos = m_grid.GetWorldPosition(cell.PosX, cell.PosY);
        if (teleport)
            character.transform.position = targetWorldPos;
        else
            character.MoveTo(targetWorldPos);

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
                m_playerInstance.TakeDamage();
            }

            // check victory condition
            if (cell.Effect == ECellEffect.Victory)
            {
                m_victoryTextGO.SetActive(true);
                m_victory = true;
            }
        }
        else
        {
            // check defeat condition
            if (AreOnSameCell(character, m_playerInstance))
            {
                m_playerInstance.TakeDamage();
            }

            // update cell danger
            cell.IsLetal = true;
        }

        if (m_playerInstance.IsDead)
        {
            m_defeatTextGO.SetActive(true);
            m_playerInstance.gameObject.SetActive(false);
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

    private void SetCurrentPlane(EPlane newPlane)
    {
        m_currentPlane = newPlane;

        foreach (IngredientComponent ingredient in m_ingredients)
        {
            ingredient.SetCurrentPlane(newPlane);
        }

        // TODO update tilemap
    }

    private void LateUpdate()
    {
        Vector3 playerPosition = m_playerInstance.transform.position;
        m_cameraTransform.position = new Vector3(playerPosition.x, playerPosition.y, m_cameraTransform.position.z);
    }

    bool AreOnSameCell(CharacterComponent character, CharacterComponent other)
    {
        return character.PosX == other.PosX && character.PosY == other.PosY;
    }
}
