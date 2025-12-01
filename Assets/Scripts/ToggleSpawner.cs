using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ToggleSpawner : MonoBehaviour
{
    public GameObject togglePrefab; 
    public Transform content;      
    public PopulationGraph graph;

    void Start()
    {
        if (PopulationManager.Instance == null || graph == null)
        {
            Debug.LogWarning("PopulationManager or PopulationGraph missing!");
            return;
        }

        // --- FIX: Clear existing toggles before creating new ones ---
        // This ensures that if Start() is called again (e.g., scene reload),
        // we don't end up with duplicate toggles.
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
        // -----------------------------------------------------------

        // Loop ONLY through allowed species
        foreach (string species in graph.AllowedSpecies)
        {
            // Only create toggles for species that actually exist in populationData
            if (PopulationManager.Instance.populationData.ContainsKey(species))
            {
                CreateToggle(species);
            }
        }
    }

    void CreateToggle(string speciesName)
    {
        GameObject obj = Instantiate(togglePrefab, content);
        obj.name = speciesName + "_Toggle";

        // Change label text
        Text label = obj.GetComponentInChildren<Text>();
        label.text = speciesName;
        
        // --- FIX: DECLARE AND INITIALIZE 'toggle' FIRST ---
        // Get the Toggle component now so we can use it to access the graphic later.
        Toggle toggle = obj.GetComponent<Toggle>();
        if (toggle == null)
        {
            Debug.LogError($"Toggle component not found on prefab for {speciesName}!");
            return;
        }

        // Get the species color for the circle on the toggle.
        Color speciesColor = PopulationManager.SpeciesColors.ContainsKey(speciesName)
            ? PopulationManager.SpeciesColors[speciesName]
            : Color.white;

        // Optional: Find the background Image component (if available) to color the toggle
        Image bgImage = obj.GetComponent<Image>();
        if (bgImage != null)
        {
            // You might want to use the color for the graphic instead of the background
        }

        // Find the graphic element (like a checkmark or circle) of the toggle
        // This is safe now because 'toggle' is defined above.
        Image checkmarkImage = toggle.graphic.GetComponent<Image>();
        if (checkmarkImage != null)
        {
            checkmarkImage.color = speciesColor;
        }

        // Hook toggle event
        toggle.isOn = true;

        toggle.onValueChanged.AddListener((isOn) =>
        {
            graph.ToggleSpecies(speciesName, isOn);
        });
    }
}