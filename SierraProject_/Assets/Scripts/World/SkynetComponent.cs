using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkynetComponent : ObstacleComponent
{
    protected override void OnInteractedInternal(PlayerComponent player)
    {
        base.OnInteractedInternal(player);

        World.IsPowerOn = true;
    }
}
