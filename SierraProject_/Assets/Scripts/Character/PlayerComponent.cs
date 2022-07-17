
public class PlayerComponent : CharacterComponent
{
    private int m_hp = 2;

    public bool IsDead
    {
        get { return m_hp <= 0; }
    }

    public void TakeDamage()
    {
        --m_hp;
    }

    public void Heal()
    {
        ++m_hp;
    }
}
