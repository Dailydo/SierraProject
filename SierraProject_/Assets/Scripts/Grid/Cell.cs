using System;
using UnityEngine;

[Serializable]
public class Cell
{
    [SerializeField]
    private bool m_walkable = true;

    public bool Walkable
    {
        get
        {
            return m_walkable;
        }
        set
        {
            m_walkable = value;
        }
    }

}
