using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorComponent : ObstacleComponent
{
    public override ECellEffect GetCellEffect()
    {
        if (IsActiveInCurrentContext() && CurrentState == EIngredientState.Unused)
            return ECellEffect.Obstacle;

        return ECellEffect.None;
    }
}
