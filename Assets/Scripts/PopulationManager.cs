using UnityEngine;
using System.Collections.Generic; // Required for Dictionaries

public class PopulationManager : MonoBehaviour
{
    public static PopulationManager Instance;

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
        }
        // -------------------------------
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
