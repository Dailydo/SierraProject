using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleComponent : IngredientComponent
{
    private bool m_isOpened = false;

    public bool IsOpened
    {
        get { return m_isOpened; }
        set { m_isOpened = true; }
    }

    public override ECellEffect GetCellEffect()
    {
        if (IsActiveInCurrentPlane() && !IsOpened)
            return ECellEffect.Obstacle;

        return ECellEffect.None;
    }
}
