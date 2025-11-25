using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ToggleSpawner : MonoBehaviour
{
    public GameObject togglePrefab;      // SpeciesToggle prefab
    public Transform content;            // The Content object inside ScrollView
    public PopulationGraph graph;

    void Start()
    {
        if (PopulationManager.Instance == null)
        {
            Debug.LogWarning("PopulationManager missing!");
            return;
        }

        // Loop through each species and create a toggle
        foreach (var entry in PopulationManager.Instance.populationData)
        {
            string speciesName = entry.Key;
            CreateToggle(speciesName);
        }
    }

    void CreateToggle(string speciesName)
    {
        GameObject obj = Instantiate(togglePrefab, content);
        obj.name = speciesName + "_Toggle";

        // Change the label text
        Text label = obj.GetComponentInChildren<Text>();
        label.text = speciesName;

        // Hook toggle event
        Toggle toggle = obj.GetComponent<Toggle>();
        toggle.isOn = true;  // default ON

        toggle.onValueChanged.AddListener((isOn) =>
        {
            graph.ToggleSpecies(speciesName, isOn);
        });

    }
}
