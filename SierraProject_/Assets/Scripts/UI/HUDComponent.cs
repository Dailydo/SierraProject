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

    [SerializeField]
    private float m_rotatingDieDuration = 1.0f;

    [SerializeField]
    private float m_rotatingDieSpeed = 5.0f;

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

    public void UpdateDieRemainingTime(float remainingTime)
    {
        if (remainingTime <= m_rotatingDieDuration && IsCurrentDieValid())
        {
            m_diceGO[m_currentDie].transform.Rotate(Vector3.forward, m_rotatingDieSpeed * Time.deltaTime);
        }
    }

    public void SetCurrentPlane(EPlane plane)
    {
        if (IsCurrentDieValid())
        {
            m_diceGO[m_currentDie].transform.rotation = Quaternion.identity;
            m_diceGO[m_currentDie].SetActive(false);
        }
            
        m_currentDie = (int)plane;
        if (plane != EPlane.Count)
            m_diceGO[m_currentDie].SetActive(true);
    }

    private bool IsCurrentDieValid()
    {
        return m_currentDie >= 0 && m_currentDie < m_diceGO.Length && m_diceGO[m_currentDie] != null;
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
