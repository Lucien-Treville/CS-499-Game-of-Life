using UnityEngine;
using System.Collections.Generic;
using System.IO;  // Required for File writing
using System;     // Required for Environment paths
using Newtonsoft.Json; 

public class SceneStateExporter : MonoBehaviour
{
    public void ExportAndSave()
    {
        // 1. Generate the JSON String (The code we wrote before)
        string jsonContent = GenerateJSONString();

        // 2. Find the Downloads Folder Path
        // This trick gets the user's home folder (e.g., C:/Users/Lucien) and adds "Downloads"
        string downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
        
        // 3. Create a unique filename (so you don't overwrite previous saves)
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string fileName = $"SimulationLevel_{timestamp}.json"; // You can use .txt if you prefer
        string fullPath = Path.Combine(downloadsPath, fileName);
        // 4. Write the file to disk
        try
        {
            File.WriteAllText(fullPath, jsonContent);
            File.SetCreationTime(fullPath, System.DateTime.Now);
            File.SetLastWriteTime(fullPath, System.DateTime.Now);
            Debug.Log($"<color=green>SUCCESS!</color> Saved template to: {fullPath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save file: {e.Message}");
        }
    }

    // This is the logic we discussed previously, wrapped in a helper function
    private string GenerateJSONString()
    {
        var rootData = new Dictionary<string, Dictionary<string, List<object>>>
        {
            { "Predators", new Dictionary<string, List<object>>() },
            { "Grazers", new Dictionary<string, List<object>>() },
            { "Plants", new Dictionary<string, List<object>>() },
            { "Obstacles", new Dictionary<string, List<object>>() }
        };

        LivingEntity[] allLiving = FindObjectsOfType<LivingEntity>();

        foreach (LivingEntity creature in allLiving)
        {
            // if (creature.gameObject.scene.name == "DontDestroyOnLoad") continue;

            string category = "";
            string speciesName = "";

            if (creature is Plant plant)
            {
                category = "Plants";
                speciesName = plant.speciesGeneData.specieName;
            }
            else if (creature is Animal animal)
            {
                if (creature is Grazer grazer)
                {
                    category = "Grazers";
                    speciesName = grazer.speciesGeneData.specieName;
                }
                else if (creature is Predator predator)
                {
                    category = "Predators";
                    speciesName = predator.speciesGeneData.specieName;
                }
                
                
            }

            if (!string.IsNullOrEmpty(category))
            {
                if (!rootData[category].ContainsKey(speciesName))
                {
                    rootData[category][speciesName] = new List<object>();
                }

                var positionArray = new float[] { 
                    creature.transform.position.x, 
                    0, 
                    creature.transform.position.z 
                };

                positionArray = ConvertToTemplateCoordinates(positionArray);

                // The format you requested: [1, [x, 0, z]]
                rootData[category][speciesName].Add(new List<object> { 1, positionArray });
            }
        }
       // --- PART 2: OBSTACLES ---
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");

        foreach (GameObject obs in obstacles)
        {
            // if (obs.scene.name == "DontDestroyOnLoad") continue;

            string cleanName = obs.name.Replace("(Clone)", "").Trim();

            // FIX 1: Ensure the dictionary has a list for this specific obstacle name
            if (!rootData["Obstacles"].ContainsKey(cleanName))
            {
                rootData["Obstacles"][cleanName] = new List<object>();
            }

            // FIX 2 & 3: Use "Obstacles" string (not 'category') AND fix array syntax
            // We must create a new float array with curly braces
            float[] obsPos = new float[] { obs.transform.position.x, 0, obs.transform.position.z };
            
            // Convert and Add
            rootData["Obstacles"][cleanName].Add(new List<object> { 1, ConvertToTemplateCoordinates(obsPos) });
        }

        return JsonConvert.SerializeObject(rootData, Formatting.Indented, new FlatArrayConverter());
    }
    private float[] ConvertToTemplateCoordinates(float[] realPos)
    {
        // 1. DEFINE BOUNDS
        float realMinX = -22f;
        float realMaxX = 75f;
        float realMinZ = -83f;
        float realMaxZ = 14f;

        float templateMin = -100f;
        float templateMax = 100f;

        // 2. NORMALIZE
        float percentX = Mathf.InverseLerp(realMinX, realMaxX, realPos[0]);
        float percentZ = Mathf.InverseLerp(realMinZ, realMaxZ, realPos[2]);

        // 3. REMAP
        float jsonX = Mathf.Lerp(templateMin, templateMax, percentX);
        float jsonZ = Mathf.Lerp(templateMin, templateMax, percentZ);

        // 4. ROUND (The new step) ✂️
        // We cast to (float) because Math.Round returns a double
        jsonX = (float)System.Math.Round(jsonX, 2);
        jsonZ = (float)System.Math.Round(jsonZ, 2);

        return new float[] {jsonX, 0, jsonZ};
    }

}

public class FlatArrayConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        // Apply ONLY to the specific types you used for your data points
        // You used List<object> for the entry and float[] for the position
        return objectType == typeof(List<object>) || objectType == typeof(float[]);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        // Force Formatting.None (Compact) just for this specific list/array
        // This writes "[1, [10.5, 0, 5.2]]" on a single line
        writer.WriteRawValue(JsonConvert.SerializeObject(value, Formatting.None));
    }

    // We don't need to read for the exporter, so we leave this blank
    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}