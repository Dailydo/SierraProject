using UnityEngine;

public class ObstacleComponent : IngredientComponent
{
    private WorldComponent m_world = null;

    public WorldComponent World
    {
        get { return m_world; }
    }

    protected override void OnInit(GridComponent grid)
    {
        base.OnInit(grid);

        m_world = grid.GetComponentInParent<WorldComponent>();
        if (m_world == null)
            Debug.LogWarning(name + " did not find the world to spawn enemy in");
    }

    public override ECellEffect GetCellEffect()
    {
        if (IsActiveInCurrentContext())
            return ECellEffect.Obstacle;

        return ECellEffect.None;
    }
}
