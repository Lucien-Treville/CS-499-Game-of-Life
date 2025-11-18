using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class MapEditor : MonoBehaviour
{
    // Prefab references
    public Camera mainCamera;
    public GameObject treePrefab;
    public GameObject pinePrefab;
    public GameObject flowerTallPrefab;
    public GameObject flower2Prefab;

    // Parent for all placed objects
    public Transform defaultMapRootTransform;

    private GameObject selectedPrefab;
    private List<GameObject> placedObjects = new List<GameObject>();

    // Layer name for terrain
    public string terrainLayerName = "Terrain";
    private int terrainLayerMask;

    void Start()
    {
        selectedPrefab = treePrefab; // default prefab

        // Build layer mask for raycast
        terrainLayerMask = LayerMask.GetMask(terrainLayerName);

        // Add any existing Placeable objects already in the scene
        GameObject[] existingObjects = GameObject.FindGameObjectsWithTag("Placeable");
        placedObjects.AddRange(existingObjects);
    }

    void Update()
    {
        // ---------------- Left-click: place object ----------------
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, terrainLayerMask))
            {
                GameObject newObj = Instantiate(selectedPrefab, hit.point, Quaternion.identity);
                newObj.tag = "Placeable"; // mark as deletable
                if (defaultMapRootTransform != null)
                    newObj.transform.parent = defaultMapRootTransform; // parent under DefaultMapRoot
                placedObjects.Add(newObj);
            }
        }

        // ---------------- Right-click: delete object ----------------
        if (Input.GetMouseButtonDown(1))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                GameObject hitObj = hit.collider.gameObject;

                // Climb to root with "Placeable" tag
                while (hitObj.transform.parent != null && !hitObj.CompareTag("Placeable"))
                {
                    hitObj = hitObj.transform.parent.gameObject;
                }

                if (hitObj.CompareTag("Placeable"))
                {
                    placedObjects.Remove(hitObj);
                    DestroyRecursively(hitObj);
                    Debug.Log("Deleted: " + hitObj.name);
                }
            }
        }
    }

    // Recursive destroy function to remove children as well
    private void DestroyRecursively(GameObject obj)
    {
        foreach (Transform child in obj.transform)
        {
            DestroyRecursively(child.gameObject);
        }
        Destroy(obj);
    }

    // ---------------- Select prefab via UI button ----------------
    public void SelectPrefab(string prefabName)
    {
        Debug.Log("Selected prefab: " + prefabName);
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
