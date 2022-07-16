using UnityEngine;

public class IngredientComponent : MonoBehaviour
{
    IngredientInstanceComponent[] m_instances;

    private Cell m_cell = null;
    private EPlane m_currentPlane = EPlane.Base;

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

        m_instances = GetComponentsInChildren<IngredientInstanceComponent>();
    }

    public virtual ECellEffect GetCellEffect()
    {
        return ECellEffect.None;
    }

    public void SetCurrentPlane(EPlane plane)
    {
        m_currentPlane = plane;
    }

    public bool IsActiveInCurrentPlane()
    {
        if (m_instances != null && m_instances.Length > 0)
        {
            foreach (IngredientInstanceComponent instance in m_instances)
            {
                if (instance.IsActiveInPlane(m_currentPlane))
                    return true;
            }

            return false;
        }

        return true;
    }
}
