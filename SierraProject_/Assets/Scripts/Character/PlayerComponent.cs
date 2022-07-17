using UnityEngine;

public class PlayerComponent : CharacterComponent
{
    private const int HP_MAX = 2;
    private int m_hp = HP_MAX;

    public int HP
    {
        get { return m_hp; }
    }

    public bool IsDead
    {
        get { return HP <= 0; }
    }

    public void TakeDamage()
    {
        --m_hp;
        if (m_hp < 0)
            m_hp = 0;

        UpdateSpriteAlpha();
    }

    public void Heal()
    {
        ++m_hp;
        if (m_hp > HP_MAX)
            m_hp = HP_MAX;

        UpdateSpriteAlpha();
    }

    private void UpdateSpriteAlpha()
    {
        Color col = CharacterSprite.color;
        col.a = m_hp == HP_MAX ? 1.0f : 0.5f;
        CharacterSprite.color = col;
    }
}
