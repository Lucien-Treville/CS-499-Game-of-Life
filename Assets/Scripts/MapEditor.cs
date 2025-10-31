using UnityEngine;
using System.Collections.Generic;

public class MapEditor : MonoBehaviour
{
    // Prefab references
    public Camera mainCamera;
    public GameObject treePrefab;
    public GameObject pinePrefab;
    public GameObject flowerTallPrefab;
    public GameObject flower2Prefab;

    private GameObject selectedPrefab;
    private List<GameObject> placedObjects = new List<GameObject>();

    void Start()
    {
        selectedPrefab = treePrefab; // default
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject.name.Contains("block-grass-overhang-low"))
                {
                    GameObject newObj = Instantiate(selectedPrefab, hit.point, Quaternion.identity);
                    placedObjects.Add(newObj);
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (placedObjects.Contains(hit.collider.gameObject))
                {
                    placedObjects.Remove(hit.collider.gameObject);
                    Destroy(hit.collider.gameObject);
                }
            }
        }
    }

    // Switch which prefab is selected
    public void SelectPrefab(string prefabName)
    {
        switch (prefabName)
        {
            case "tree":
                selectedPrefab = treePrefab;
                break;
            case "tree-pine":
                selectedPrefab = pinePrefab;
                break;
            case "flowers-tall":
                selectedPrefab = flowerTallPrefab;
                break;
            case "flowers2":
                selectedPrefab = flower2Prefab;
                break;
        }
    }
}
