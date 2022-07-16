
public class PlayerComponent : CharacterComponent
{
    private bool m_isDead = false;

    public bool IsDead
    {
        get { return m_isDead; }
    }

    public void TakeDamage()
    {
        m_isDead = true;
    }
}
