using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDComponent : MonoBehaviour
{
    [SerializeField]
    private TextPanelComponent m_textPanel = null;

    [SerializeField]
    private GameObject[] m_diceGO = new GameObject[(int)EPlane.Count];

    private int m_currentDie = (int)EPlane.Count;

    // Start is called before the first frame update
    void Awake()
    {
        InitDice();
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

    public void RequestDisplayText(string text)
    {
        if (m_textPanel != null)
        {
            m_textPanel.RequestText(text);
        }
    }

    public bool IsDisplayingText()
    {
        if (m_textPanel != null)
        {
            return m_textPanel.IsDisplayingText();
        }

        return false;
    }

    public void ResetText()
    {
        if (m_textPanel != null)
        {
            m_textPanel.ResetText();
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
}
