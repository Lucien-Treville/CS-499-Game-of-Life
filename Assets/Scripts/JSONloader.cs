using System.IO;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor; // For file picker in Editor
#endif

[System.Serializable]
public class MapData
{
    public int width;
    public int height;
    public string[] tiles; // flattened array
}

public class JSONloader : MonoBehaviour
{
    public Text statusText;  // UI Text for feedback

    private string baseMapName;
    private string mapsFolderPath;

    void Start()
    {
        // Get which base map the player selected
        baseMapName = PlayerPrefs.GetString("BaseMap", "Grasslands");

        // Path to your StreamingAssets/Maps folder
        mapsFolderPath = Path.Combine(Application.streamingAssetsPath, "Maps");

        // Load the default map automatically
        LoadDefaultMap();
    }

    void LoadDefaultMap()
    {
        string defaultPath = Path.Combine(mapsFolderPath, $"{baseMapName}Map.json");

        if (File.Exists(defaultPath))
        {
            string json = File.ReadAllText(defaultPath);
            ApplyMapData(json);
            statusText.text = $"Loaded default {baseMapName} map.";
        }
        else
        {
            statusText.text = $"Default map for {baseMapName} not found!";
            Debug.LogWarning("No default map found at: " + defaultPath);
        }
    }

    public void OnUploadButtonClick()
    {
#if UNITY_EDITOR
        string userPath = EditorUtility.OpenFilePanel("Select Map JSON", "", "json");
        if (!string.IsNullOrEmpty(userPath))
        {
            string json = File.ReadAllText(userPath);
            ApplyMapData(json);
            statusText.text = $"Loaded custom map: {Path.GetFileName(userPath)}";
            Debug.Log($"Custom map loaded: {userPath}");
        }
#else
        statusText.text = "File upload not supported outside of Editor yet.";
#endif
    }

    public void OnDownloadTemplateClick()
    {
        string templatePath = Path.Combine(mapsFolderPath, baseMapName + "Map.json");

        if (File.Exists(templatePath))
        {
#if UNITY_EDITOR
            string savePath = EditorUtility.SaveFilePanel("Save Map Template", "", baseMapName + "Map.json", "json");
            if (!string.IsNullOrEmpty(savePath))
            {
                File.Copy(templatePath, savePath, true);
                statusText.text = $"Template {baseMapName} downloaded!";
            }
#else
            string desktopPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop), baseMapName + "Map.json");
            File.Copy(templatePath, desktopPath, true);
            statusText.text = $"Template {baseMapName} downloaded to Desktop!";
#endif
        }
        else
        {
            statusText.text = "Template not found!";
            Debug.LogWarning("Template not found at: " + templatePath);
        }
    }

    void ApplyMapData(string json)
    {
        MapData map = JsonUtility.FromJson<MapData>(json);

        if (map == null || map.tiles == null || map.tiles.Length != map.width * map.height)
        {
            Debug.LogError("Failed to parse map JSON or tiles are empty!");
            statusText.text = "Failed to load map!";
            return;
        }

        // Clear previous tiles
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        float tileSize = 1f;

        for (int y = 0; y < map.height; y++)
        {
            for (int x = 0; x < map.width; x++)
            {
                string tileType = map.tiles[y * map.width + x];

                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.parent = transform;
                cube.transform.position = new Vector3(x * tileSize, 0, -y * tileSize);

                Renderer rend = cube.GetComponent<Renderer>();
                switch (tileType)
                {
                    case "Grass":
                        rend.material.color = Color.green;
                        break;
                    case "Swamp":
                        rend.material.color = Color.blue;
                        break;
                    case "Desert":
                        rend.material.color = Color.yellow;
                        break;
                    case "Tundra":
                        rend.material.color = Color.white;
                        break;
                    default:
                        rend.material.color = Color.gray;
                        break;
                }
            }
        }

        statusText.text = $"Loaded map: {map.width}x{map.height}";
    }
}
