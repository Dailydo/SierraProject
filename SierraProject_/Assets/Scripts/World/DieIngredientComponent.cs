using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieIngredientComponent : ObstacleComponent
{
    [SerializeField]
    private EPlane m_addingPlane = EPlane.Flesh;

    protected override void OnInteractedInternal(PlayerComponent player)
    {
        base.OnInteractedInternal(player);

        World.AddAvailablePlane(m_addingPlane);
    }
}
