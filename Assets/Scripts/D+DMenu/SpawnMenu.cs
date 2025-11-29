using UnityEngine;
using UnityEngine.EventSystems;

public class SpawnMenu : MonoBehaviour
{
    public GameObject[] objects;

    public LayerMask groundMask;
    public VisibilityToggle visibilityToggle;

    private GameObject activeObject;
    void Awake()
    {
        SelectObject(0);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && visibilityToggle.isToggled)
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;
            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Only hit colliders on the Ground layer
            if (Physics.Raycast(ray, out hit, 300f, groundMask))
            {
                Instantiate(activeObject, hit.point, Quaternion.identity);
                Debug.Log("Spawned on: " + hit.collider.gameObject.name);
            }
        }
    }

    public void SelectObject (int index)
    {
        activeObject = objects[index];
    }
}
