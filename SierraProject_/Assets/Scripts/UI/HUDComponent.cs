using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDComponent : MonoBehaviour
{
    [SerializeField]
    private GameObject m_victoryTextGO = null;

    [SerializeField]
    private GameObject m_defeatTextGO = null;

    [SerializeField]
    private GameObject[] m_diceGO = new GameObject[(int)EPlane.Count];

    private int m_currentDie = (int)EPlane.Count;

    // Start is called before the first frame update
    void Awake()
    {
        InitDice();
        InitVictoryText();
        InitDefeatText();
    }

    void InitDice()
    {
        if (m_diceGO == null || m_diceGO.Length != (int)EPlane.Count)
        {
            Debug.LogWarning("HUD - no die go or invalid number!");
            return;
        }

        for (int i = 0; i < m_diceGO.Length; ++i)
        {
            if (m_diceGO[i] != null)
                m_diceGO[i].SetActive(false);
        }
    }

    void InitVictoryText()
    {
        if (m_victoryTextGO != null)
        {
            m_victoryTextGO.SetActive(false);
        }
        else
        {
            Debug.LogError("No victory text GO filled");
        }
    }

    void InitDefeatText()
    {
        if (m_defeatTextGO != null)
        {
            m_defeatTextGO.SetActive(false);
        }
        else
        {
            Debug.LogError("No defeat text GO filled");
        }
    }

    public void SetCurrentPlane(EPlane plane)
    {
        if (m_currentDie >= 0 && m_currentDie < m_diceGO.Length && m_diceGO[m_currentDie] != null)
            m_diceGO[m_currentDie].SetActive(false);
            
        m_currentDie = (int)plane;
        if (plane != EPlane.Count)
            m_diceGO[m_currentDie].SetActive(true);
    }

    public void SetVictoryTextActive(bool active)
    {
        m_victoryTextGO.SetActive(active);
    }

    public void SetDefeatTextActive(bool active)
    {
        m_defeatTextGO.SetActive(active);
    }
}
