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
        // SelectObject(0); // forces the menu to have first item pre-selected. Means if you click Obstacles menu, boulders can be spawned without selecting boulder option. 
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
            if (Physics.Raycast(ray, out hit, 400f, groundMask))
            {
                Instantiate(activeObject, hit.point, Quaternion.identity);
                // update population manager after instantiating creature
                // Debug.LogWarning("Initializing species: " + activeObject.name);
                PopulationManager.Instance.InitializeSpecies(activeObject.name, 1);




                // Debug.Log("Spawned on: " + hit.collider.gameObject.name);
            }
        }
    }

    public void SelectObject (int index)
    {
        activeObject = objects[index];
    }
}
