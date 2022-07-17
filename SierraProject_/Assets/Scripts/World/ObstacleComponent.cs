using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleComponent : IngredientComponent
{
    public override ECellEffect GetCellEffect()
    {
        if (IsActiveInCurrentContext())
            return ECellEffect.Obstacle;

        return ECellEffect.None;
    }
}
