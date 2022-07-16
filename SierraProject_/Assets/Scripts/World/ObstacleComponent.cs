using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleComponent : IngredientComponent
{
    private bool m_isActive = true;

    public override ECellEffect GetCellEffect()
    {
        if (m_isActive)
            return ECellEffect.Obstacle;

        return ECellEffect.None;
    }
}
