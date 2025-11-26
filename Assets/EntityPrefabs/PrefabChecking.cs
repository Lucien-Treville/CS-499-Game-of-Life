using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabChecking : MonoBehaviour
{   

    public GameObject wolfPrefab;
    public GameObject tigerPrefab;
    public GameObject snakePrefab;
    public GameObject rabbitPrefab;
    public GameObject sheepPrefab;
    public GameObject horsePrefab;
    public GameObject bushPrefab;
    public GameObject treePrefab;
    public GameObject flowerPrefab;
    public GameObject stumpPrefab;
    public GameObject boulderPrefab;
    public List<GameObject> prefabList = new List<GameObject>();
    // Start is called before the first frame update

    void Start()
    {
        prefabList.Add(wolfPrefab);
        prefabList.Add(tigerPrefab);
        prefabList.Add(snakePrefab);
        prefabList.Add(rabbitPrefab);
        prefabList.Add(sheepPrefab);
        prefabList.Add(horsePrefab);
        prefabList.Add(bushPrefab);
        prefabList.Add(treePrefab);
        prefabList.Add(flowerPrefab);
        prefabList.Add(stumpPrefab);
        prefabList.Add(boulderPrefab);

        // spawn each prefab three times, in a row with scale increasing by 0.5 each time
        float zOffset = -80f;
        float scale = 1f;
        foreach (GameObject prefab in prefabList)
        {
            float xOffset = -12f;

            // get prefab scale and adjust starting scale accordingly
            Vector3 prefabScale = prefab.transform.localScale;
            Debug.Log("Prefab: " + prefab.name + " Original Scale: " + prefabScale.ToString());
            Instantiate(prefab, new Vector3(xOffset - 4, 1.1f, zOffset-10), Quaternion.identity);
            for (int i = 0; i < 3; i++)
            {
                Instantiate(prefab, new Vector3(xOffset, 1.1f, zOffset), Quaternion.identity).transform.localScale = prefabScale * scale;
                xOffset += 4f; // Move the next prefab over
                scale *= 1.5f; // Increase scale for next prefab
            }
            zOffset += 10f; // Move to the next row for the next prefab
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
