using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneFilterComponent : MonoBehaviour
{
    public GameObject[] m_filters;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPlaneFilter(int planeIndex)
    {
        if (planeIndex >= 0 && planeIndex < m_filters.Length)
        {
            int localIndex = 0;
            foreach(GameObject filter in m_filters)
            {
                if (filter != null)
                {
                    if (planeIndex == localIndex)
                    {
                        filter.SetActive(true);
                    }
                    else
                    {
                        filter.SetActive(false);      
                    }
                }
                ++localIndex;
            }
        }
    }
}
