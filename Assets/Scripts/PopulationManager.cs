using UnityEngine;
using System.Collections.Generic; 
using TMPro;
using UnityEngine.UI;

public class PopulationManager : MonoBehaviour
{
    public static PopulationManager Instance;
    public TextMeshProUGUI TerminationText;
    public Canvas TerminationCanvas;

    // The Key is the species name (string), the Value is the stats object
    public Dictionary<string, SpeciesStats> populationData = new Dictionary<string, SpeciesStats>();
    public float simStartTime;

    // -------------------------------
    // ADDED: Timer for logging history
    public float logInterval = 1f;
    private float timer = 0f;
    // -------------------------------

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    private void Start()
    {
        // -------------------------------
        // ADDED: Mark simulation start
        simStartTime = Time.time;
        // -------------------------------
    }

    private void Update()
    {
        // -------------------------------
        // ADDED: Log species counts every X seconds
        timer += Time.deltaTime;
        if (timer >= logInterval)
        {
            timer = 0f;

            // Log each species once per interval
            foreach (var sp in populationData.Values)
            {
                sp.AddToHistory(Time.time - simStartTime);
            }
            // check termination conditions
            if (GetPlantCount() <= 0)
            {
                Debug.Log("All plants have been eliminated! Simulation ending.");
                TerminationText.text = "All plants have been eliminated! Grazers are soon to starve, causing predators to starve as well. Simulation ending.";
                TerminationCanvas.gameObject.SetActive(true);
                Time.timeScale = 0f;
                // Implement additional termination logic here (e.g., stop simulation, notify user)
            }
            if (GetGrazerCount() <= 0)
            {
                Debug.Log("All grazers have been eliminated! Simulation ending.");
                TerminationText.text = "All grazers have been eliminated! Predators are soon to starve. Simulation ending.";
                TerminationCanvas.gameObject.SetActive(true);
                Time.timeScale = 0f;
                // Implement additional termination logic here (e.g., stop simulation, notify user)
            }
        }
        // -------------------------------
    }


    // make getter methods for predator, grazer, and plant currentCount for termination conditions
    public int GetPredatorCount()
    {
        List<string> predatorSpecies = new List<string> { "Wolf", "Tiger", "Snake" };
        int totalPredators = 0;
        foreach (string species in predatorSpecies)
        {
            if (populationData.ContainsKey(species))
            {
                totalPredators += populationData[species].currentCount;
            }
        }
        return totalPredators;
    }

    public int GetGrazerCount()
    {
        List<string> grazerSpecies = new List<string> { "Sheep", "Rabbit", "Horse" };
        int totalGrazers = 0;
        foreach (string species in grazerSpecies)
        {
            if (populationData.ContainsKey(species))
            {
                totalGrazers += populationData[species].currentCount;
            }
        }
        return totalGrazers;
    }

    public int GetPlantCount()
    {
        List<string> plantSpecies = new List<string> { "Berry Bush", "Apple Tree", "Flowers", "Grass" };
        int totalPlants = 0;
        foreach (string species in plantSpecies)
        {
            if (populationData.ContainsKey(species))
            {
                totalPlants += populationData[species].currentCount;
            }
        }
        return totalPlants;
    }

    // 1. Initialize a specific species (Called by Spawner/PopulationCounter)
    public void InitializeSpecies(string speciesName, int count)
    {
        if (populationData.ContainsKey(speciesName))
        {
            // If species exists, update count
            populationData[speciesName].currentCount += count;
            populationData[speciesName].UpdateStats();
            Debug.Log($"Updated species: {speciesName} with count: {populationData[speciesName].currentCount}");
        }
        else
        {
            // Create brand new species entry
            SpeciesStats newStats = new SpeciesStats(speciesName, count);
            populationData.Add(speciesName, newStats);
            Debug.Log($"Initialized species: {speciesName} with count: {count}");

            // First history entry
            newStats.AddToHistory(Time.time - simStartTime);
        }
    }

    // 2. Handle changes (Called by Creatures when born/removed)
    public void UpdateCount(string speciesName, int amount)
    {
        if (populationData.ContainsKey(speciesName))
        {
            populationData[speciesName].currentCount += amount;
            populationData[speciesName].UpdateStats();

            Debug.Log($"Species: {speciesName} | Updated Count: {populationData[speciesName].currentCount}");

            
        }
    }
}

[System.Serializable]
public class SpeciesStats
{
    public string name;
    public int currentCount;
    public int maxRecorded;
    public int minRecorded;
    public List<PopulationHistoryPoint> history = new List<PopulationHistoryPoint>();

    public SpeciesStats(string speciesName, int startingCount)
    {
        name = speciesName;
        currentCount = startingCount;
        maxRecorded = startingCount;
        minRecorded = startingCount;
    }

    public void UpdateStats()
    {
        if (currentCount > maxRecorded) maxRecorded = currentCount;
        if (currentCount < minRecorded && currentCount >= 0) minRecorded = currentCount;

        // Log changes immediately
        AddToHistory(Time.time - PopulationManager.Instance.simStartTime);
    }

    public void AddToHistory(float time)
    {
        if (history.Count > 0)
        {
            int lastIndex = history.Count - 1;

            // Copy the struct properly
            PopulationHistoryPoint lastPoint = history[lastIndex];

            // If the event happens within 0.1 seconds, overwrite instead of adding
            if (time - lastPoint.time <= 0.1f)
            {
                lastPoint.count = currentCount;
                history[lastIndex] = lastPoint;
                return;
            }
        }

        // Add new timepoint
        history.Add(new PopulationHistoryPoint
        {
            time = time,
            count = currentCount
        });
    }
}

[System.Serializable]
public struct PopulationHistoryPoint
{
    public float time;  // The X axis
    public int count;   // The Y axis
}
