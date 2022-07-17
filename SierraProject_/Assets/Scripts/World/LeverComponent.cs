using UnityEngine;

public class LeverComponent : IngredientComponent
{
    [SerializeField]
    private DoorComponent m_linkedDoor = null;

    protected override void OnInteractedInternal(PlayerComponent player)
    {
        base.OnInteractedInternal(player);

        if (m_linkedDoor != null)
        {
            m_linkedDoor.SetUsed();
        }
    }
}
