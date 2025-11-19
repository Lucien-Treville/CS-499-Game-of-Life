using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
// using System.Diagnostics;


// MapLoader is responsible for loading a map from a JSON file and instantiating the appropriate game objects
// ReadJSON() will read the JSON file and populate the jsonData dictionary
// Spawner() will instantiate the game objects based on the data in jsonData

// To-do: Implement Spawner() to instantiate prefabs based on jsonData


public class MapLoader : MonoBehaviour
{
    
    // dictionary to hold json data
    private Dictionary<string, object> jsonData = new Dictionary<string, object>
    {
        {"Predators", new Dictionary<string, object>
            {
                {"Wolf", new List<object> { } },
                {"Tiger", new List<object> { } },
                {"Snake", new List<object> { } }
            }
        },
        {"Grazers", new Dictionary<string, object>
            {
                {"Rabbit", new List<object> { } },
                {"Sheep", new List<object> { } },
                {"Horse", new List<object> { } }
            }
        },
        {"Plants", new Dictionary<string, object>
            {
                {"Berry Bush", new List<object> { } },
                {"Apple Tree", new List<object> { } },
                {"Flowers", new List<object> { } }
            }
        },
        {"Obstacles", new Dictionary<string, object>
            {
                {"Stump", new List<object> { } },
                {"Boulder", new List<object> { } }
            }
        }
    };


    // Prefabs for different map objects
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

    public static string jsonFileName = "SpawnSettings.JSON"; // or demo.json if user clicks on Demo button
    public static string jsonFilePath;

    void Start()
    {
        switch (jsonFileName)
        {
            case "SpawnSettings.JSON":
                Debug.Log("Loading map from SpawnSettings.JSON");
                jsonFilePath = Path.Combine(Application.persistentDataPath, jsonFileName);
                ReadJSON(jsonFilePath);
                break;
            case "demo.json":
                Debug.Log("Loading map from demo.json");
                jsonFilePath = Path.Combine(Application.streamingAssetsPath, jsonFileName);
                ReadJSON(jsonFilePath);
                break;
            case "userFile":
                Debug.Log("Loading map from user selected file: " + jsonFilePath);
                ReadJSON(jsonFilePath);
                break;
            default:
                Debug.Log(jsonFileName + " is not a recognized map file.");
                break;
        }
        
        Spawner();
    }

    public void ReadJSON(string jsonPath)
    {
        if (!File.Exists(jsonPath))
        {
            Debug.LogWarning("No map JSON found: " + jsonPath);
            return;
        }

        string json = File.ReadAllText(jsonPath);
        JObject root = JObject.Parse(json);

        var firstKeys = new[] { "Predators", "Grazers", "Plants", "Obstacles" };
        // Example: Extract all creatures from "Predators"
        var predators = root["Predators"];
        foreach (var creatureType in firstKeys)
        {
            var creatures = root[creatureType];
            foreach (var creature in creatures)
            {
                string name = ((JProperty)creature).Name;
                JArray spawns = (JArray)((JProperty)creature).Value;

                foreach (JArray spawn in spawns)
                {
                    int count = spawn[0].ToObject<int>();
                    JArray posArr = (JArray)spawn[1];
                    Vector3 position = new Vector3(
                        posArr[0].ToObject<float>(),
                        posArr[1].ToObject<float>(),
                        posArr[2].ToObject<float>()
                    );
                    ((List<object>)((Dictionary<string, object>)jsonData[creatureType])[name]).Add(new object[] { count, position });
                    // instantiate prefabs based on name and position here
                }
            }
        }

        // PrintJsonData();
        Debug.Log("Map loaded from JSON: " + jsonPath);
    }

    private void PrintJsonData()
    {
        foreach (var categoryPair in jsonData)
        {
            Debug.Log($"Category: {categoryPair.Key}");
            var creatureDict = categoryPair.Value as Dictionary<string, object>;
            foreach (var creaturePair in creatureDict)
            {
                Debug.Log($"  Creature: {creaturePair.Key}");
                var spawnList = creaturePair.Value as List<object>;
                foreach (var spawnObj in spawnList)
                {
                    var spawnArr = spawnObj as object[];
                    int count = (int)spawnArr[0];
                    Vector3 pos = (Vector3)spawnArr[1];
                    Debug.Log($"    Count: {count}, Position: {pos}");
                }
            }
        }
    }


    private void Spawner()
    {



        foreach (var categoryPair in jsonData)
        {
            var creatureDict = categoryPair.Value as Dictionary<string, object>;
            foreach (var creaturePair in creatureDict)
            {
                var spawnList = creaturePair.Value as List<object>;
                foreach (var spawnObj in spawnList)
                {
                    var spawnArr = spawnObj as object[];
                    int count = (int)spawnArr[0];
                    Vector3 pos = (Vector3)spawnArr[1] + new Vector3(-115, 1.1f, -190);  // offset to center in scene
                    // Debug.Log($"Spawn {count} {creaturePair.Key} at {pos}");

                    // find prefab to use based on creaturePair.Key
                    GameObject prefabToSpawn = null;
                    switch (creaturePair.Key)
                    {
                        case "Wolf":
                            prefabToSpawn = wolfPrefab;
                            break;
                        case "Tiger":
                            prefabToSpawn = tigerPrefab;
                            break;
                        case "Snake":
                            prefabToSpawn = snakePrefab;
                            break;
                        case "Rabbit":
                            prefabToSpawn = rabbitPrefab;
                            break;
                        case "Sheep":
                            prefabToSpawn = sheepPrefab;
                            break;
                        case "Horse":
                            prefabToSpawn = horsePrefab;
                            break;
                        case "Berry Bush":
                            prefabToSpawn = bushPrefab;
                            break;
                        case "Apple Tree":
                            prefabToSpawn = treePrefab;
                            break;
                        case "Flowers":
                            prefabToSpawn = flowerPrefab;
                            break;
                        case "Stump":
                            prefabToSpawn = stumpPrefab;
                            break;
                        case "Boulder":
                            prefabToSpawn = boulderPrefab;
                            break;
                        default:
                            Debug.LogWarning("No prefab found for " + creaturePair.Key);
                            break;
                    }

                    if (prefabToSpawn != null)
                    {
                        Instantiate(prefabToSpawn, pos, Quaternion.identity); // spawn first one at pos, rest will spawn in circle around
                        if (PopulationManager.Instance != null)
                            {
                                // Pass the Name and Count to PopulationManager to initialize tracking
                                PopulationManager.Instance.InitializeSpecies(creaturePair.Key, count);
                            }

                        for (int i = 0; i < count-1; i++)
                        {
                            Vector3 newPos = pos;
                            // change pos slightly for next spawn to avoid overlap
                            // have pos change in a circle around original pos
                            float angle = i * (360f / (count-1));
                            float x = Mathf.Cos(angle * Mathf.Deg2Rad) * 5f;
                            float z = Mathf.Sin(angle * Mathf.Deg2Rad) * 5f;
                            newPos += new Vector3(x, 0, z);
                            // Debug.Log($"Instantiated {creaturePair.Key} at newPos:{newPos}\tpos:{pos}");
                            
                            Instantiate(prefabToSpawn, newPos, Quaternion.identity);
                        }
                    }
                }
            }
        }
    }



}


