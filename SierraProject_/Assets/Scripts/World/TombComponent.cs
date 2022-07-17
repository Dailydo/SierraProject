using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TombComponent : ObstacleComponent
{
    protected override void OnInteractedInternal(PlayerComponent player)
    {
        base.OnInteractedInternal(player);

        player.Victory = true;
    }
}
