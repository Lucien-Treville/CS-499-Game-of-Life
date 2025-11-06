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

    public GameObject treePrefab;
    public GameObject pinePrefab;
    public GameObject flowerTallPrefab;
    public GameObject flower2Prefab;

    public void LoadMap(string jsonPath)
    {
        if (!File.Exists(jsonPath))
        {
            Debug.LogWarning("No map JSON found: " + jsonPath);
            return;
        }

        string json = File.ReadAllText(jsonPath);
        MapData map = JsonUtility.FromJson<MapData>(json);

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
            {
                GameObject instance = Instantiate(prefab, obj.position, obj.rotation);
                instance.tag = "Placeable"; // mark for deletion
            }
        }

        Debug.Log("✅ Map loaded from JSON: " + jsonPath);
    }
}
