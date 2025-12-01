using UnityEngine;
using UnityEngine.UI;

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

        // Remove old toggles
        foreach (Transform child in content)
            Destroy(child.gameObject);

        // Create toggles only for species that exist
        foreach (string species in graph.AllowedSpecies)
        {
            if (PopulationManager.Instance.populationData.ContainsKey(species))
                CreateToggle(species);
        }
    }

    private void CreateToggle(string speciesName)
    {
        GameObject obj = Instantiate(togglePrefab, content);
        obj.name = speciesName + "_Toggle";

        Text label = obj.GetComponentInChildren<Text>();
        label.text = speciesName;

        Toggle toggle = obj.GetComponent<Toggle>();
        toggle.isOn = true;

        // Color the checkmark
        if (PopulationManager.SpeciesColors.ContainsKey(speciesName))
        {
            Image check = toggle.graphic.GetComponent<Image>();
            if (check != null)
                check.color = PopulationManager.SpeciesColors[speciesName];
        }

        toggle.onValueChanged.AddListener((isOn) =>
        {
            graph.ToggleSpecies(speciesName, isOn);
        });
    }
}
