using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class MapExporter : MonoBehaviour
{
    public string outputFileName = "grasslands_template.json";

    // Editable prefab names (case sensitive)
    private readonly string[] editableNames = { "flowers-tall", "flowers2", "rocks", "tree-pine", "tree" };



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
        public List<MapObject> objects = new List<MapObject>();
    }

    [ContextMenu("Export Map To JSON")]
    public void ExportMapToJSON()
    {
        MapData data = new MapData();

        // Loop through every root object in the scene
        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            if (!obj.scene.IsValid()) continue; // Skip prefabs not in scene

            string objName = obj.name.ToLower();

            // Only export editable items
            foreach (string editable in editableNames)
            {
                if (objName.Contains(editable.ToLower()))
                {
                    data.objects.Add(new MapObject
                    {
                        name = editable,
                        position = obj.transform.position,
                        rotation = obj.transform.rotation
                    });
                    break;
                }
            }
        }

        // Convert to JSON (pretty printed)
        string json = JsonUtility.ToJson(data, true);

        // Save inside Assets/Resources/
        string path = Path.Combine(Application.dataPath, "Resources", outputFileName);
        File.WriteAllText(path, json);

        Debug.Log($"✅ Map exported successfully to: {path}");
    }
    public string ExportCurrentMapToJSON()
    {
        MapData data = new MapData();

        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            if (!obj.scene.IsValid()) continue;

            string objName = obj.name.ToLower();

            foreach (string editable in editableNames)
            {
                if (objName.Contains(editable.ToLower()))
                {
                    data.objects.Add(new MapObject
                    {
                        name = editable,
                        position = obj.transform.position,
                        rotation = obj.transform.rotation
                    });
                    break;
                }
            }
        }

        // Return JSON text (not saving to file here)
        return JsonUtility.ToJson(data, true);
    }

}
