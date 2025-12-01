using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
// using System.Diagnostics;
using UnityEngine.SceneManagement;
using UnityEngine.AI;


// MapLoader is responsible for loading a map from a JSON file and instantiating the appropriate game objects
// ReadJSON() will read the JSON file and populate the jsonData dictionary
// Spawner() will instantiate the game objects based on the data in jsonData

// To-do: Implement Spawner() to instantiate prefabs based on jsonData


public class MapLoader : MonoBehaviour
{
    private float jsonMin = -100f;
    private float jsonMax = 100f;

    // We will calculate these automatically from the NavMesh
    private float worldMinX, worldMaxX;
    private float worldMinZ, worldMaxZ;
    private float defaultY;
    public float clusterRadius = 2f; // Minimum distance from water's edge
    public LayerMask groundLayer; // Set this to "Terrain" or "Default" (Exclude Player/Creatures!)



    // dictionary to hold json data
    private static Dictionary<string, object> jsonData;

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
    public GameObject grassPrefab;

    public static string jsonFileName = "demo.json"; // or demo.json if user clicks on Demo button
    public static string jsonFilePath;

    void Awake()
    {
        CalculateNavMeshBounds();
    }

    void Start()
    {
        PopulationManager.Instance.simStartTime = Time.time;

        if (jsonFileName == "DnDeditor") return; 

        // initialize jsonData structure every new simulation
        jsonData = new Dictionary<string, object>
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
                    {"Flowers", new List<object> { } },
                    {"Grass", new List<object> { } }
                }
            },
            {"Obstacles", new Dictionary<string, object>
                {
                    {"Stump", new List<object> { } },
                    {"Boulder", new List<object> { } }
                }
            }
        };


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

    public static void ReadJSON(string jsonPath)
    {
        if (!File.Exists(jsonPath))
        {
            Debug.LogWarning("No map JSON found: " + jsonPath);
            return;
        }

        string json = File.ReadAllText(jsonPath);
        JObject root = JObject.Parse(json);

        var firstKeys = new[] { "Predators", "Grazers", "Plants", "Obstacles" };
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

                    if (position.y != 0)
                        throw new System.Exception("Y position must be 0 in JSON spawn data. Found y=" + position.y + " for " + name);

                    if (position.x < -100 || position.x > 100 || position.z < -100 || position.z > 100)
                        throw new System.Exception("Spawn position out of bounds in JSON spawn data. Found position " + position + " for " + name + " (must be within -100 to 100 for x and z)");

                    if (count < 0)
                        throw new System.Exception("Spawn count cannot be negative in JSON spawn data. Found count=" + count + " for " + name);
                    if (count > 15)
                        throw new System.Exception("High spawn count (" + count + " > 15) for " + name + ", may cause performance issues or spawning problems.");

                    if (SceneManager.GetActiveScene().name == "Grasslands")
                    {
                        // Debug.Log($"Parsed spawn: {count} x {name} at {position} in {creatureType}");
                        ((List<object>)((Dictionary<string, object>)jsonData[creatureType])[name]).Add(new object[] { count, position });
                    }
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
            // Debug.Log($"Category: {categoryPair.Key}");
            var creatureDict = categoryPair.Value as Dictionary<string, object>;
            foreach (var creaturePair in creatureDict)
            {
                // Debug.Log($"  Creature: {creaturePair.Key}");
                var spawnList = creaturePair.Value as List<object>;
                foreach (var spawnObj in spawnList)
                {
                    var spawnArr = spawnObj as object[];
                    int count = (int)spawnArr[0];
                    Vector3 pos = (Vector3)spawnArr[1];
                    // Debug.Log($"    Count: {count}, Position: {pos}");
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
                int totalCount = 0;
                foreach (var spawnObj in spawnList)
                {
                    var spawnArr = spawnObj as object[];
                    int count = (int)spawnArr[0];
                    totalCount += count;
                    /// Debug.Log($"Spawn {count} {creaturePair.Key} at {pos}");

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
                        case "Grass":
                            prefabToSpawn = grassPrefab;
                            break;
                        default:
                            Debug.LogWarning("No prefab found for " + creaturePair.Key);
                            break;
                    }

                    Vector3 pos = GetValidSpawnPoint(((Vector3)spawnArr[1]).x, ((Vector3)spawnArr[1]).z, prefabToSpawn, 1);
                    
                    if (prefabToSpawn != null)
                    {
                        Instantiate(prefabToSpawn, pos, Quaternion.identity); // spawn first one at pos, rest will spawn in circle around


                        for (int i = 0; i < count - 1; i++)
                        {
                            Vector3 newPos = pos;
                            // change pos slightly for next spawn to avoid overlap
                            // have pos change in a circle around original pos
                            float angle = i * (360f / (count - 1));
                            float x = Mathf.Cos(angle * Mathf.Deg2Rad) * 5f;
                            float z = Mathf.Sin(angle * Mathf.Deg2Rad) * 5f;
                            newPos += new Vector3(x, 0, z);
                            // Debug.Log($"Instantiated {creaturePair.Key} at newPos:{newPos}\tpos:{pos}");

                            Instantiate(prefabToSpawn, newPos, Quaternion.identity);
                        }
                    }
                }

                if (PopulationManager.Instance != null)
                {
                    // Pass the Name and Count to PopulationManager to initialize tracking
                    PopulationManager.Instance.InitializeSpecies(creaturePair.Key, totalCount);
                }


            }
        }
        foreach (var specie in PopulationManager.Instance.populationData.Keys)
        {
            PopulationManager.Instance.populationData[specie].UpdateStats();
        }
    }

    private void CalculateNavMeshBounds()
    {
        // 1. Get all vertices in the NavMesh
        NavMeshTriangulation triangulation = NavMesh.CalculateTriangulation();

        if (triangulation.vertices.Length == 0)
        {
            Debug.LogError("No NavMesh found! Did you bake it?");
            return;
        }

        // 2. Iterate to find the Min and Max extent of the world
        // Initialize with the first vertex
        Bounds bounds = new Bounds(triangulation.vertices[0], Vector3.zero);

        foreach (Vector3 vertex in triangulation.vertices)
        {
            bounds.Encapsulate(vertex);
        }

        worldMinX = bounds.min.x;
        worldMaxX = bounds.max.x;
        worldMinZ = bounds.min.z;
        worldMaxZ = bounds.max.z;
        defaultY = bounds.center.y; // Good starting height for raycasts

        Debug.Log($"World Bounds Auto-Detected: X[{worldMinX} to {worldMaxX}], Z[{worldMinZ} to {worldMaxZ}], Y={defaultY}");
    }


public Vector3 GetValidSpawnPoint(float jsonX, float jsonZ, GameObject creaturePrefab, int spaghettiID)
{
    // --- STEP 1: GET SAFE X/Z FROM NAVMESH ---
    // (This part works, so we keep it to handle Water avoidance)
    float percentX, percentZ, realX, realZ;
    Vector3 targetPos = new Vector3(0,0,0);
    switch (spaghettiID)    // I want this function for spawning children but need to skip the json section, please forgive this spaghetti
    {
        case 1:
            percentX = Mathf.InverseLerp(jsonMin, jsonMax, jsonX);
            percentZ = Mathf.InverseLerp(jsonMin, jsonMax, jsonZ);
            realX = Mathf.Lerp(worldMinX, worldMaxX, percentX);
            realZ = Mathf.Lerp(worldMinZ, worldMaxZ, percentZ);
            targetPos = new Vector3(realX, defaultY, realZ);
            break;
        case 2:
            targetPos = new Vector3(jsonX, 1, jsonZ);
            break;
        default:
            Debug.LogError($"MapLoader.GetValidSpawnPoint does not have spaghetti ID {spaghettiID}");
            break;
    }



    Vector3 finalPos = targetPos; // Default fallback

    NavMeshHit hit; // make sure we don't spawn in lake
    if (NavMesh.SamplePosition(targetPos, out hit, 10.0f, NavMesh.AllAreas)) // 30 felt big
    {
        Vector3 safePoint = hit.position;

        // Coastline push logic (Keep this!)
        NavMeshHit edgeHit;
        if (NavMesh.FindClosestEdge(safePoint, out edgeHit, NavMesh.AllAreas))
        {
            if (edgeHit.distance < clusterRadius)
            {
                float pushDistance = clusterRadius - edgeHit.distance + 0.5f; 
                safePoint += edgeHit.normal * pushDistance;
            }
        }
        
        // We now have the perfect X and Z. 
        // BUT, we ignore the NavMesh's Y. It causes sinking.
        finalPos = safePoint;
    }

    // --- STEP 2: THE "SKY DROP" (Fixing the Y-Axis) ---
    
    // Start high above the safe X/Z point
    Vector3 skyOrigin = new Vector3(finalPos.x, 10f, finalPos.z);
    RaycastHit groundHit;

    // Raycast specifically against the Ground Layer
    if (Physics.Raycast(skyOrigin, Vector3.down, out groundHit, 20f, groundLayer))
    {
        // This gives us the EXACT surface of the visual mesh/terrain
        finalPos.y = groundHit.point.y;
        
        // Visual Debug: Draw a green line where the ray hit
        Debug.DrawLine(skyOrigin, groundHit.point, Color.green, 10f);
    }
    else
    {
        // If we missed the ground (cave? hole?), fallback to NavMesh Y
        Debug.LogWarning($"Raycast missed ground at {finalPos.x}, {finalPos.z}");
    }

    // --- STEP 3: APPLY PIVOT CORRECTION ---
    // Now we lift the creature so its feet sit on that exact point
    // float legHeight = GetLegHeight(creaturePrefab);
    // finalPos.y += legHeight;

    return finalPos;
}

// Helper: Calculates distance from Pivot (0,0,0) to Feet (Min Y)
// Accounts for the Prefab's scale to prevent sinking.
private float GetLegHeight(GameObject prefab)
{
    // Debug.Log("Actually entered GetLegHeight for prefab: " + prefab.name);
    if (prefab == null) return 0f;

    // 1. Get the root scale of the prefab
    // If your prefab is scaled to 2.0 on the Y axis, we need to double the offset.
    float rootScaleY = prefab.transform.localScale.y;

    // 2. Check Skinned Mesh (Most common for animated creatures)
    SkinnedMeshRenderer skinnedMesh = prefab.GetComponentInChildren<SkinnedMeshRenderer>();
    if (skinnedMesh != null && skinnedMesh.sharedMesh != null)
    {
        // bounds.min.y is the lowest point of the mesh in local space.
        // We take the absolute value (e.g., -0.9 becomes 0.9)
        // Then multiply by scale to get the "World" height relative to pivot.
        return Mathf.Abs(skinnedMesh.sharedMesh.bounds.min.y) * rootScaleY;
    }

    // 3. Check Standard Mesh (Static objects/props)
    MeshFilter meshFilter = prefab.GetComponentInChildren<MeshFilter>();
    if (meshFilter != null && meshFilter.sharedMesh != null)
    {
        return Mathf.Abs(meshFilter.sharedMesh.bounds.min.y) * rootScaleY;
    }

    // 4. Fallback to Collider (Capsules/Boxes)
    // Useful if the mesh is missing or complex, but a collider exists.
    Collider col = prefab.GetComponentInChildren<Collider>();
    if (col != null)
    {
        // For standard Unity capsules, the pivot is in the center.
        // So the feet are exactly half the height (extents.y) away from the center.
        return col.bounds.extents.y * rootScaleY;
    }

    Debug.LogWarning("No valid renderer or collider found on prefab: " + prefab.name);
    return 0f; // No valid renderer/collider found
}

}


