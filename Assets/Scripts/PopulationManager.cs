using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class PopulationManager : MonoBehaviour
{
    public static PopulationManager Instance;

    public TextMeshProUGUI TerminationText;
    public Canvas TerminationCanvas;

    public Dictionary<string, SpeciesStats> populationData =
        new Dictionary<string, SpeciesStats>();

    public float simStartTime;
    private float timer = 0f;
    public float logInterval = 1f;

    // ============================================================
    //  UNIFIED COLOR DICTIONARY (Graph + Heatmap)
    // ============================================================
    public static readonly Dictionary<string, Color> SpeciesColors =
        new Dictionary<string, Color>()
    {
        { "Rabbit",     new Color(1f, 0.9f, 0.2f) },   // Yellow
        { "Wolf",       new Color(0.8f, 0.0f, 0.0f) }, // Dark red
        { "Tiger",      new Color(1f, 0.5f, 0.1f) },   // Orange
        { "Horse",      new Color(0.2f, 0.4f, 1f) },   // Blue
        { "Snake",      new Color(0.6f, 0.2f, 0.8f) }, // Purple
        { "Sheep",      new Color(0.6f, 0.4f, 0.2f) }, // Brown

        // Plants — names FIXED to match toggles + spawner + UI
        { "Grass",      new Color(0.7f, 1f, 0.7f) },
        { "Berry Bush", new Color(0.4f, 0.75f, 0.4f) },
        { "Apple Tree", new Color(0.2f, 0.6f, 0.2f) },
        { "Flowers",    new Color(1f, 0.4f, 1f) }
    };

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
        simStartTime = Time.time;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= logInterval)
        {
            timer = 0f;

            // Log a backup snapshot each second
            foreach (var sp in populationData.Values)
                sp.AddToHistory(Time.time - simStartTime);

            CheckTermination();
        }
    }

    // ============================================================
    // TERMINATION RULES
    // ============================================================
    private void CheckTermination()
    {
        if (GetPlantCount() <= 0)
        {
            Terminate("All plants have been eliminated! Grazers will starve next.");
        }

        if (GetGrazerCount() <= 0)
        {
            Terminate("All grazers have been eliminated! Predators will starve next.");
        }
    }

    private void Terminate(string message)
    {
        TerminationText.text = message;
        TerminationCanvas.gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    // ============================================================
    // GROUPED COUNT HELPERS
    // ============================================================
    public int GetPredatorCount()
    {
        string[] preds = { "Wolf", "Tiger", "Snake" };
        int total = 0;

        foreach (string s in preds)
            if (populationData.ContainsKey(s))
                total += populationData[s].currentCount;

        return total;
    }

    public int GetGrazerCount()
    {
        string[] grazers = { "Sheep", "Rabbit", "Horse" };
        int total = 0;

        foreach (string s in grazers)
            if (populationData.ContainsKey(s))
                total += populationData[s].currentCount;

        return total;
    }

    public int GetPlantCount()
    {
        string[] plants = { "Berry Bush", "Apple Tree", "Flowers", "Grass" };
        int total = 0;

        foreach (string s in plants)
            if (populationData.ContainsKey(s))
                total += populationData[s].currentCount;

        return total;
    }

    // ============================================================
    // SPECIES REGISTRATION
    // ============================================================
    public void InitializeSpecies(string speciesName, int count)
    {
        if (populationData.ContainsKey(speciesName))
        {
            populationData[speciesName].currentCount += count;
            populationData[speciesName].UpdateStats();
        }
        else
        {
            SpeciesStats newStats = new SpeciesStats(speciesName, count);
            populationData.Add(speciesName, newStats);
        }

        float t = Time.time - simStartTime;
        populationData[speciesName].AddToHistory(t);
    }

    // ============================================================
    // COUNT UPDATES (Births)
    // ============================================================
    public void UpdateCount(string speciesName, int amount)
    {
        if (!populationData.ContainsKey(speciesName))
            return;

        SpeciesStats stats = populationData[speciesName];
        stats.currentCount += amount;
        stats.UpdateStats();

        float t = Time.time - simStartTime;
        stats.AddToHistory(t);
    }

    // ============================================================
    // DEATH REPORTING
    // ============================================================
    public void ReportDeath(string speciesName)
    {
        if (!populationData.ContainsKey(speciesName))
            return;

        SpeciesStats stats = populationData[speciesName];
        stats.currentCount--;
        stats.UpdateStats();

        float t = Time.time - simStartTime;
        stats.AddToHistory(t);
    }
}

// ============================================================
// SPECIES DATA + HISTORY
// ============================================================
[System.Serializable]
public class SpeciesStats
{
    public string name;
    public int currentCount;
    public int maxRecorded;
    public int minRecorded;

    public List<PopulationHistoryPoint> history =
        new List<PopulationHistoryPoint>();

    public SpeciesStats(string n, int start)
    {
        name = n;
        currentCount = start;
        maxRecorded = start;
        minRecorded = start;
    }

    public void UpdateStats()
    {
        if (currentCount > maxRecorded) maxRecorded = currentCount;
        if (currentCount < minRecorded) minRecorded = currentCount;
    }

    public void AddToHistory(float time)
    {
        if (history.Count > 0)
        {
            PopulationHistoryPoint last = history[history.Count - 1];

            if (last.time == time && last.count == currentCount)
                return;
        }

        history.Add(new PopulationHistoryPoint()
        {
            time = time,
            count = currentCount
        });
    }
}

[System.Serializable]
public struct PopulationHistoryPoint
{
    public float time;
    public int count;
}
