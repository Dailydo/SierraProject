using System;
using UnityEngine;

public enum ECellEffect
{
    None = 0,
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

    public bool Walkable
    {
        get { return m_walkable; }
        set { m_walkable = value; }
    }

    public ECellEffect Effect
    {
        get { return m_effect; }
    }

}
