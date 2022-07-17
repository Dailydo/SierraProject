using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextPanelComponent : MonoBehaviour
{
    private TextMeshProUGUI m_textMeshComponent = null;
    private bool m_isActive = false;

    // Start is called before the first frame update
    void Start()
    {
        if (gameObject != null)
        {
            m_textMeshComponent = gameObject.GetComponent<TextMeshProUGUI>();
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    { 
    }

    public void RequestText(string text)
    {
        if (m_textMeshComponent != null && gameObject != null)
        {
            gameObject.SetActive(true);
            m_textMeshComponent.text = text;
        }
    }

    public bool IsDisplayingText()
    {
        if (gameObject != null)
        {
            return gameObject.activeSelf;
        }
        return false;
    }

    public void ResetText()
    {
        if (m_textMeshComponent != null && gameObject != null && gameObject.activeSelf == true)
        {
            m_textMeshComponent.text = new string("");
            gameObject.SetActive(true);
        }
    }
}
