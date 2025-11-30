using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class ReportManager : MonoBehaviour
{
   [Header("UI References")]
    public Transform populationCardContainer;

    // Start is called before the first frame update
    void Start()
    {
        if (PopulationManager.Instance == null) Debug.Log("PopulationManager instance not found. Did you start the simulation?");
        if (populationCardContainer == null) Debug.LogError("You forgot to assign the Population Card Container in the Inspector!");


        // testing
        Debug.Log($"Population manager shows {PopulationManager.Instance.GetPredatorCount()} predators currently.");
        Debug.Log($"Population manager shows {PopulationManager.Instance.GetGrazerCount()} grazers currently.");
        Debug.Log($"Population manager shows {PopulationManager.Instance.GetPlantCount()} plants currently.");

        // PrintSpeciesCounts();
        
        PopulateReport();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // for testing, prints in Debug.Log all species counts and histories
    public void PrintSpeciesCounts()
    {
        if (PopulationManager.Instance != null)
        {
            foreach (var entry in PopulationManager.Instance.populationData)
            {
                Debug.Log($"Species: {entry.Key} | Current Count: {entry.Value.currentCount} | Max Recorded: {entry.Value.maxRecorded} | Min Recorded: {entry.Value.minRecorded}");
                foreach (var point in entry.Value.history)
                {
                    Debug.Log($"Species: {entry.Key} | Time: {point.time} | Count: {point.count}");
                }
            }
        }
        else
        {
            Debug.Log("PopulationManager instance not found.");
        }
    }
    void PopulateReport()
    {
        // 1. Get the data and Sort it immediately
        // We convert the Dictionary Values to a List, then Order By MaxRecorded (Descending)
        // also need to drop "Boulder" and "Stump" from list
        List<SpeciesStats> sortedStats = PopulationManager.Instance.populationData.Values
            .Where(stat => stat.name != "Boulder" && stat.name != "Stump")  // Exclude "Boulder" and "Stump"
            .OrderByDescending(stat => stat.maxRecorded)
            .ToList();
        
        // 2. Iterate through the UI rows
        int statsIndex = 0;

        // 0th child is the header, so we start from 1
        for (int i = 1; i < populationCardContainer.childCount; i++)
        {
            Transform entryRow = populationCardContainer.GetChild(i);

            // Check if we still have species data to show
            if (statsIndex < sortedStats.Count)     // think this is ai bs, dont need statsIndex?
            {
                SpeciesStats data = sortedStats[statsIndex]; // -1 because we started from 1 in the loop, instead of statsIndex                

                // 3. Assign Data to the 3 Children (Creature, Min, Max)
                // Assuming order: Child 0 = Name, Child 1 = Min, Child 2 = Max
                TextMeshProUGUI nameText = entryRow.GetChild(0).GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI minText = entryRow.GetChild(1).GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI maxText = entryRow.GetChild(2).GetComponent<TextMeshProUGUI>();

                // Set the text
                if (nameText != null) nameText.text = data.name;
                if (minText != null)  minText.text = data.minRecorded.ToString();
                if (maxText != null)  maxText.text = data.maxRecorded.ToString();

                statsIndex++;
            }
            else
            {
                // We have more UI slots than species. Turn off the empty rows.
                entryRow.gameObject.SetActive(false);
            }
        }
    }
}
