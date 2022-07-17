using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientInstanceComponent : MonoBehaviour
{
    [SerializeField]
    private EPlane m_plane = EPlane.Base;

    [SerializeField]
    private EIngredientState m_state = EIngredientState.Unused;

    [SerializeField]
    private bool m_isInteractive = true;

    public bool IsInteractive
    {
        get { return m_isInteractive; }
    }

    public bool MatchesContext(EIngredientState state, EPlane plane)
    {
        return MatchesState(state) && IsActiveInPlane(plane);
    }

    public bool IsActiveInPlane(EPlane plane)
    {
        return m_plane == plane;
    }

    public bool MatchesState(EIngredientState state)
    {
        return m_state == state;
    }
}
