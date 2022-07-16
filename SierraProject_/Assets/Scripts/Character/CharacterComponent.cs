using UnityEngine;

public class CharacterComponent : MonoBehaviour
{
    [SerializeField]
    private float m_moveDelayInSeconds = 1.0f;

    private int m_posX = 0;
    private int m_posY = 0;
    private float m_timeBeforeNextMove = 0.0f;

    public int PosX
    {
        get { return m_posX; }
        set { m_posX = value; }
    }

    public int PosY
    {
        get { return m_posY; }
        set { m_posY = value; }
    }

    private void Start()
    {
        m_timeBeforeNextMove = m_moveDelayInSeconds;
    }

    private void Update()
    {
        if (m_timeBeforeNextMove > 0.0f)
        {
            m_timeBeforeNextMove -= Time.deltaTime;
        }

        UpdateInternal();
    }

    protected virtual void UpdateInternal()
    { }

    public bool CanMove()
    {
        return m_timeBeforeNextMove <= 0.0f && CanMoveInternal();
    }

    public virtual bool CanMoveInternal()
    {
        return true;
    }

    public void OnMove()
    {
        m_timeBeforeNextMove = m_moveDelayInSeconds;
        OnMoveInternal();
    }

    protected virtual void OnMoveInternal()
    {
    }
}
