using UnityEngine;
using System.Collections.Generic; // Required for Dictionaries

public class PopulationManager : MonoBehaviour
{
    public static PopulationManager Instance;

    // The Key is the species name (string), the Value is the stats object
    public Dictionary<string, SpeciesStats> populationData = new Dictionary<string, SpeciesStats>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    // 1. Initialize a specific species (Called by Spawner)
    public void InitializeSpecies(string speciesName, int count)
    {
        if (populationData.ContainsKey(speciesName))
        {
            // If we already have this species, just add to the count
            populationData[speciesName].currentCount += count;
            populationData[speciesName].UpdateStats();
            Debug.Log($"Updated species: {speciesName} with count: {populationData[speciesName].currentCount}");
        }
        else
        {
            // Create a new entry for this species
            SpeciesStats newStats = new SpeciesStats(speciesName, count);
            populationData.Add(speciesName, newStats);
            Debug.Log($"Initialized species: {speciesName} with count: {count}");
        }
    }

    // 2. Handle changes (Called by Creatures)
    public void UpdateCount(string speciesName, int amount)
    {
        if (populationData.ContainsKey(speciesName))
        {
            populationData[speciesName].currentCount += amount;
            populationData[speciesName].UpdateStats();

            // Debug log to verify it's working
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

    // Constructor to set up the starting values
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
    }
}