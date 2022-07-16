using UnityEngine;

public class CharacterComponent : MonoBehaviour
{
    [SerializeField]
    private float m_moveDelayInSeconds = 1.0f;

    private int m_posX = 0;
    private int m_posY = 0;
    private float m_timeBeforeNextMove = 0.0f;

    private Vector3 m_currentWorldPos = Vector3.zero;
    private Vector3 m_targetWorldPos = Vector3.zero;
    private bool m_isMoving = false;

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
        m_currentWorldPos = transform.position;
    }

    private void Update()
    {
        if (m_timeBeforeNextMove > 0.0f)
        {
            m_timeBeforeNextMove -= Time.deltaTime;
        }

        if (m_isMoving)
        {
            float lerpVal = 1.0f - Mathf.Max(0.0f, m_timeBeforeNextMove / m_moveDelayInSeconds);
            Vector3 pos = Vector3.Lerp(m_currentWorldPos, m_targetWorldPos, lerpVal);
            transform.position = pos;

            if (m_timeBeforeNextMove <= 0.0f)
            {
                m_currentWorldPos = m_targetWorldPos;
                m_isMoving = false;
            }
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

    public void MoveTo(Vector3 targetWorldPos)
    {
        m_timeBeforeNextMove = m_moveDelayInSeconds;
        m_targetWorldPos = targetWorldPos;
        m_isMoving = true;

        OnMoveInternal();
    }

    protected virtual void OnMoveInternal()
    {
    }
}
