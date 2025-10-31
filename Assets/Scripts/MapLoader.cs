using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class MapLoader : MonoBehaviour
{
    [System.Serializable]
    public class MapObject
    {
        public string name;
        public Vector3 position;
        public Quaternion rotation;
    }

    [System.Serializable]
    public class MapData
    {
        public List<MapObject> objects;
    }

    // Prefabs for editable objects
    public GameObject treePrefab;
    public GameObject pinePrefab;
    public GameObject flowerTallPrefab;
    public GameObject flower2Prefab;

    private void Start()
    {
        string jsonPath = Path.Combine(Application.persistentDataPath, "grasslands_template.json");

        if (!File.Exists(jsonPath))
        {
            Debug.LogWarning("No custom map JSON found. Using default layout.");
            return;
        }

        string json = File.ReadAllText(jsonPath);
        MapData map = JsonUtility.FromJson<MapData>(json);

        // Spawn all editable objects
        foreach (MapObject obj in map.objects)
        {
            GameObject prefab = null;

            switch (obj.name)
            {
                case "tree":
                    prefab = treePrefab;
                    break;
                case "tree-pine":
                    prefab = pinePrefab;
                    break;
                case "flowers-tall":
                    prefab = flowerTallPrefab;
                    break;
                case "flowers2":
                    prefab = flower2Prefab;
                    break;
            }

            if (prefab != null)
                Instantiate(prefab, obj.position, obj.rotation);
        }

        Debug.Log("✅ Editable objects loaded from JSON: " + jsonPath);
    }
}
