using UnityEngine;

public class WorldComponent : MonoBehaviour
{
    [SerializeField]
    private GameObject m_characterPrefab = null;

    [SerializeField]
    private GridComponent m_grid = null;

    private CharacterComponent m_characterInstance = null;

    // Start is called before the first frame update
    void Start()
    {
        GameObject characterGO = Instantiate<GameObject>(m_characterPrefab);
        if (characterGO != null)
        {
            m_characterInstance = characterGO.GetComponent<CharacterComponent>();
            m_characterInstance.transform.position = m_grid.GetWorldPosition(2, 4);
            m_characterInstance.transform.parent = transform;
        }
        else
        {
            Debug.LogError("Cannot instantiate character prefab");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
