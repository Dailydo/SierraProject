using UnityEngine;

public class IngredientComponent : MonoBehaviour
{
    // Array of IngredientInstanceComponent -> EPlane m_plane

    private Cell m_cell = null;

    public void Init(GridComponent grid)
    {
        m_cell = grid.FindCellFromWorldPosition(transform.position);
        if (m_cell != null)
        {
            m_cell.InnerIngredient = this;
        }
        else
        {
            Debug.LogWarning(name + " cannot find its cell");
        }
    }

    public virtual ECellEffect GetCellEffect()
    {
        return ECellEffect.None;
    }
}
