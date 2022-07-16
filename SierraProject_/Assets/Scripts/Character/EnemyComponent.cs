using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyComponent : CharacterComponent
{
    [SerializeField]
    private float m_movePerSeconds = 1.0f;

    public float MovePerSeconds
    {
        get { return m_movePerSeconds; }
    }

}
