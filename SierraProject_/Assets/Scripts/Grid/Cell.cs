using System;
using UnityEngine;

public enum ECellEffect
{
    None = 0,
    PlayerSpawnPoint,
    Victory,
    EnemySpawnPoint,
    Obstacle,
}

[Serializable]
public class Cell
{
    [SerializeField]
    private ECellEffect m_effect = ECellEffect.None;

    private IngredientComponent m_innerIngredient = null;

    private int m_posX = 0;
    private int m_posY = 0;
    private bool m_isLetal = false;

    public bool Walkable
    {
        get { return Effect != ECellEffect.Obstacle; }
    }

    public ECellEffect Effect
    {
        get
        {
            if (m_innerIngredient != null && m_innerIngredient.GetCellEffect() != ECellEffect.None)
                return m_innerIngredient.GetCellEffect();

            return m_effect;
        }
        set { m_effect = value; }
    }

    public int PosX
    {
        get { return m_posX; }
    }

    public int PosY
    {
        get { return m_posY; }
    }

    public bool IsLetal
    {
        get { return m_isLetal; }
        set { m_isLetal = value; }
    }

    public IngredientComponent InnerIngredient
    {
        get { return m_innerIngredient; }
        set { m_innerIngredient = value; }
    }

    public void Init(int posX, int posY)
    {
        m_posX = posX;
        m_posY = posY;
    }
}
