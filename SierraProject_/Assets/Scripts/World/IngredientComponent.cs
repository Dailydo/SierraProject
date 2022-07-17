using UnityEngine;

public enum EIngredientState
{
    Unused,
    Used
}

public class IngredientComponent : MonoBehaviour
{
    [SerializeField]
    private bool m_isInteractive = true;

    IngredientInstanceComponent[] m_instances;

    private Cell m_cell = null;
    private EPlane m_currentPlane = EPlane.Base;
    private EIngredientState m_currentState = EIngredientState.Unused;

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

        m_instances = GetComponentsInChildren<IngredientInstanceComponent>(true);
        m_currentState = EIngredientState.Unused;
        m_currentPlane = EPlane.Base;
        UpdateInstancesFromContext();
    }

    public virtual ECellEffect GetCellEffect()
    {
        return ECellEffect.None;
    }

    public EIngredientState CurrentState
    {
        get { return m_currentState; }
    }

    public void SetUsed()
    {
        m_currentState = EIngredientState.Used;
        UpdateInstancesFromContext();
    }

    public void SetCurrentPlane(EPlane plane)
    {
        m_currentPlane = plane;
        UpdateInstancesFromContext();
    }

    private void UpdateInstancesFromContext()
    {
        if (m_instances != null && m_instances.Length > 0)
        {
            foreach (IngredientInstanceComponent instance in m_instances)
            {
                instance.gameObject.SetActive(instance.MatchesContext(m_currentState, m_currentPlane));
            }
        }
    }

    public IngredientInstanceComponent GetActiveContextInstance()
    {
        if (m_instances != null && m_instances.Length > 0)
        {
            foreach (IngredientInstanceComponent instance in m_instances)
            {
                if (instance.MatchesContext(m_currentState, m_currentPlane))
                    return instance;
            }
        }

        return null;
    }

    public bool IsActiveInCurrentContext()
    {
        if (m_instances != null && m_instances.Length > 0)
        {
            return GetActiveContextInstance() != null;
        }

        return true;
    }

    public virtual void OnInteracted()
    {
        if (IsInteractive())
        {
            SetUsed();
            Debug.Log("Player has interacted with " + name);
        }
    }

    public bool IsInteractive()
    {
        IngredientInstanceComponent activeInstance = GetActiveContextInstance();
        if (activeInstance != null)
            return activeInstance.IsInteractive;

        return m_isInteractive;
    }
}
