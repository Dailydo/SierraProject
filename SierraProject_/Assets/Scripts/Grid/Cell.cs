using System;
using UnityEngine;

public enum ECellEffect
{
    None = 0,
    PlayerSpawnPoint,
    Victory,
    Trap
}

[Serializable]
public class Cell
{
    [SerializeField]
    private bool m_walkable = true;

    [SerializeField]
    private ECellEffect m_effect = ECellEffect.None;

    private int m_posX = 0;
    private int m_posY = 0;

    public bool Walkable
    {
        get { return m_walkable; }
        set { m_walkable = value; }
    }

    public ECellEffect Effect
    {
        get { return m_effect; }
    }

    public int PosX
    {
        get { return m_posX; }
    }

    public int PosY
    {
        get { return m_posY; }
    }

    public void Init(int posX, int posY)
    {
        m_posX = posX;
        m_posY = posY;
    }

}
