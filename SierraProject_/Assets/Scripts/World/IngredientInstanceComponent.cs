using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientInstanceComponent : MonoBehaviour
{
    [SerializeField]
    private EPlane m_plane = EPlane.Base;

    public bool IsActiveInPlane(EPlane plane)
    {
        return m_plane == plane;
    }
}
