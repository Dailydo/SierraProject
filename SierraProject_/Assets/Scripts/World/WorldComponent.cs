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
    private HUDComponent m_HUD = null;

    [SerializeField]
    private float m_swapPlaneDelay = 5.0f;

    [SerializeField]
    private EPlane m_overridenPlane = EPlane.Count;

    private PlayerComponent m_playerInstance = null;
    private List<EnemyComponent> m_enemiesInstances = new List<EnemyComponent>();
    private IngredientComponent[] m_ingredients;

    private List<EPlane> m_availablePlanes = new List<EPlane>();
    private EPlane m_currentPlane = EPlane.Base;
    private float m_swapPlaneCooldown;

    private bool m_victory = false;

    // Start is called before the first frame update
    void Start()
    {
        m_grid.InitCells();
        InitIngredients();
        InstantiatePlayer();

        m_swapPlaneCooldown = m_swapPlaneDelay;
        m_victory = false;

        m_availablePlanes.Add(EPlane.Base);
        m_HUD.SetCurrentPlane(EPlane.Base);

        // TEMP test
        Cell defaultEnemySpawnPoint = m_grid.GetSpecificCell(ECellEffect.EnemySpawnPoint);
        if (defaultEnemySpawnPoint != null)
            SpawnEnemy(m_enemyPrefab, defaultEnemySpawnPoint);
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
                    m_playerInstance.Init(m_grid);
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

    public void SpawnEnemy(GameObject enemyPrefab, Cell spawnPoint)
    {
        if (spawnPoint != null)
        {
            GameObject enemyGO = Instantiate(enemyPrefab);
            if (enemyGO != null)
            {
                EnemyComponent enemyInstance = enemyGO.GetComponent<EnemyComponent>();
                if (enemyInstance != null)
                {
                    enemyInstance.transform.parent = transform; // later, will be the proper grid

                    SetCharacterPos(enemyInstance, spawnPoint, true);
                    enemyInstance.Init(m_grid, m_playerInstance);
                    m_enemiesInstances.Add(enemyInstance);
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
        else
        {
            Debug.LogError("Need a valid spawn point to spawn enemy");
        }
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (m_playerInstance.IsDead || m_victory)
            return;

        UpdateInputs();
        UpdateEnemies();
        UpdateCurrentPlane();
    }

    void UpdateInputs()
    {
        if (Input.GetKey(KeyCode.Q) || Input.GetAxis("Horizontal") < -0.5f)
        {
            MovePlayer(-1, 0);
        }

        if (Input.GetKey(KeyCode.D) || Input.GetAxis("Horizontal") > 0.5f)
        {
            MovePlayer(1, 0);
        }

        if (Input.GetKey(KeyCode.Z) || Input.GetAxis("Vertical") > 0.5f)
        {
            MovePlayer(0, 1);
        }

        if (Input.GetKey(KeyCode.S) || Input.GetAxis("Vertical") < -0.5f)
        {
            MovePlayer(0, -1);
        }

        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Joystick1Button0))
        {
            TryInteract();
        }
    }

    void UpdateEnemies()
    {
        foreach (EnemyComponent enemy in m_enemiesInstances)
        {
            if (enemy.CanMove() && enemy.gameObject.activeSelf)
            {
                SetCharacterPos(enemy, enemy.TargetCell, false);
            }
        }
    }

    public void AddAvailablePlane(EPlane plane)
    {
        if (plane != EPlane.Count && !m_availablePlanes.Contains(plane))
        {
            m_availablePlanes.Add(plane);
        }
    }

    void UpdateCurrentPlane()
    {
        if (m_swapPlaneCooldown > 0.0f && m_availablePlanes.Count > 1)
        {
            m_swapPlaneCooldown -= Time.deltaTime;
            if (m_swapPlaneCooldown <= 0.0f)
            {
                EPlane newPlane = m_overridenPlane != EPlane.Count ? m_overridenPlane : m_availablePlanes[Random.Range(0, m_availablePlanes.Count)];
                SetCurrentPlane(newPlane);

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
            if (cell.IsLethal)
            {
                m_playerInstance.TakeDamage();
            }

            // check victory condition
            if (cell.Effect == ECellEffect.Victory)
            {
                m_HUD.SetVictoryTextActive(true);
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
            cell.IsLethal = true;
        }

        if (m_playerInstance.IsDead)
        {
            m_HUD.SetDefeatTextActive(true);
            m_playerInstance.gameObject.SetActive(false);
        }
    }

    void OnCharacterLeftCell(CharacterComponent character, Cell cell)
    {
        if (character is EnemyComponent)
        {
            // remove cell danger
            cell.IsLethal = false;
        }
    }

    private void TryInteract()
    {
        IngredientComponent closeIngredient = m_grid.GetCloseIngredient(m_playerInstance.PosX, m_playerInstance.PosY);
        if (closeIngredient != null)
            closeIngredient.OnInteracted(m_playerInstance);
    }

    private void SetCurrentPlane(EPlane newPlane)
    {
        if (m_currentPlane == newPlane)
            return;

        m_currentPlane = newPlane;
        m_HUD.SetCurrentPlane(newPlane);

        foreach (IngredientComponent ingredient in m_ingredients)
        {
            ingredient.SetCurrentPlane(newPlane);
        }

        foreach (EnemyComponent enemy in m_enemiesInstances)
        {
            bool lethal = enemy.LethalPlane == newPlane;
            Cell enemyCell = m_grid.GetCell(enemy.PosX, enemy.PosY);
            if (enemyCell != null)
                enemyCell.IsLethal = lethal;
            enemy.gameObject.SetActive(lethal);
        }

        // TODO update tilemap

        Debug.Log("Current plane updated to " + newPlane.ToString());
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
