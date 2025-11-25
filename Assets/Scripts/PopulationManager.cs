using UnityEngine;
using System.Collections.Generic; // Required for Dictionaries

public class PopulationManager : MonoBehaviour
{
    public static PopulationManager Instance;

    // The Key is the species name (string), the Value is the stats object
    public Dictionary<string, SpeciesStats> populationData = new Dictionary<string, SpeciesStats>();
    public float simStartTime;

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
            // Debug.Log($"Updated species: {speciesName} with count: {populationData[speciesName].currentCount}");
        }
        else
        {
            // Create a new entry for this species
            SpeciesStats newStats = new SpeciesStats(speciesName, count);
            populationData.Add(speciesName, newStats);
            // Debug.Log($"Initialized species: {speciesName} with count: {count}");
            populationData[speciesName].AddToHistory(Time.time);
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
            // Debug.Log($"Species: {speciesName} | Updated Count: {populationData[speciesName].currentCount}");
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
        AddToHistory(Time.time);
    }

    public void AddToHistory(float time)
    {
        time -= PopulationManager.Instance.simStartTime;
        if (history.Count > 0)
        {
            int lastIndex = history.Count - 1;

            // We must copy the struct out to read/modify it
            PopulationHistoryPoint lastPoint = history[lastIndex];

            if (time - lastPoint.time <= 0.1f) // if deaths within 0.1 seconds, edit last entry
            {
                lastPoint.count = currentCount;
                history[lastIndex] = lastPoint;
                return; 
            }
        }
        // if array is empty or time difference is more than 0.1s, add new entry
        history.Add(new PopulationHistoryPoint { time = time, count = currentCount });
    }
}

[System.Serializable]
public struct PopulationHistoryPoint
{
    public float time;  // The X axis
    public int count;   // The Y axis
}