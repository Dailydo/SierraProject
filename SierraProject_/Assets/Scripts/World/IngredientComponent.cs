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

    [SerializeField]
    private bool m_needsPower = false;

    [SerializeField]
    private AudioClip m_interactSound;

    IngredientInstanceComponent[] m_instances;

    private Cell m_cell = null;
    private EPlane m_currentPlane = EPlane.Base;
    private EIngredientState m_currentState = EIngredientState.Unused;
    private bool m_isPowerOn = false;
    private AudioSource m_audioSource;

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
        m_isPowerOn = false;
        UpdateInstancesFromContext();

        m_audioSource = GetComponent<AudioSource>();

        OnInit(grid);
    }

    protected virtual void OnInit(GridComponent grid)
    { }

    public virtual ECellEffect GetCellEffect()
    {
        return ECellEffect.None;
    }

    public EIngredientState CurrentState
    {
        get { return m_currentState; }
    }

    protected Cell GetCell()
    {
        return m_cell;
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
        OnPlaneChanged();
    }

    protected virtual void OnPlaneChanged()
    { }

    public void PowerOn()
    {
        m_isPowerOn = true;
        OnPowerOn();
    }

    protected virtual void OnPowerOn()
    { }

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

    public void OnInteracted(PlayerComponent player)
    {
        if (IsInteractive())
        {
            SetUsed();
            PlaySounds(); 
            OnInteractedInternal(player);

            Debug.Log("Player has interacted with " + name);
        }
    }

    protected virtual void OnInteractedInternal(PlayerComponent player)
    { }

    public bool IsInteractive()
    {
        if (m_needsPower && !m_isPowerOn)
            return false;

        IngredientInstanceComponent activeInstance = GetActiveContextInstance();
        if (activeInstance != null)
            return activeInstance.IsInteractive;

        return m_isInteractive;
    }

    public void PlaySounds()
    {
        if (m_audioSource)
        {
            if (m_interactSound)
            {
                m_audioSource.clip = m_interactSound;
                m_audioSource.Play();
            }
        }
        else
            Debug.Log("No audio source component in " + name);

    }
}
