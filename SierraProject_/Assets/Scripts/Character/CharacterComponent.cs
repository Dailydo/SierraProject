using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterComponent : MonoBehaviour
{
    private int m_posX = 0;
    private int m_posY = 0;

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

}
