using System.Collections.Generic;
using UnityEngine;

public class EggComponent : ObstacleComponent
{
    [SerializeField]
    private GameObject m_enemyPrefab = null;

    private Cell m_targetCellForEnemy = null;
    private bool m_hasEnemySpawned = false;

    protected override void OnInit(GridComponent grid)
    {
        base.OnInit(grid);

        List<Cell> walkableCells = grid.GetWalkableNeighbours(GetCell().PosX, GetCell().PosY);
        if (walkableCells.Count > 0)
            m_targetCellForEnemy = walkableCells[0];
        else
            Debug.LogWarning("An egg did not find any place to spawn enemy");
    }

    protected override void OnInteractedInternal(PlayerComponent player)
    {
        base.OnInteractedInternal(player);

        player.Heal();
    }

    protected override void OnPlaneChanged()
    {
        base.OnPlaneChanged();

        if (CurrentState == EIngredientState.Used && !m_hasEnemySpawned)
        {
            if (m_targetCellForEnemy != null && World != null)
            {
                World.SpawnEnemy(m_enemyPrefab, m_targetCellForEnemy);
            }
            m_hasEnemySpawned = true;
        }
    }
}
