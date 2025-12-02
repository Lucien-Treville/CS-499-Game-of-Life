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

        // Color
        // Color the checkmark, background, and the Toggle ColorBlock
        if (PopulationManager.SpeciesColors.TryGetValue(speciesName, out Color c))
        {
            // declare up front so it's definitely assigned for later use
            Image checkImg = null;

            // assign if toggle.graphic is an Image
            if (toggle.graphic is Image img)
                checkImg = img;

            // 1) Color the checkmark (if present)
            if (checkImg != null)
                checkImg.color = c;

            // 2) Try to color a separate background image (common child name "Background")
            Transform bgT = obj.transform.Find("Background") ?? obj.transform.Find("BackgroundImage");
            if (bgT != null)
            {
                Image bgImg = bgT.GetComponent<Image>();
                if (bgImg != null) bgImg.color = c * 0.9f;
            }
            else
            {
                // fallback: color the first Image child that is not the checkmark
                Image[] imgs = obj.GetComponentsInChildren<Image>();
                foreach (var imgChild in imgs)
                {
                    if (imgChild == checkImg) continue;
                    imgChild.color = c * 0.9f;
                    break;
                }
            }

            // 3) Update the ColorBlock so transitions use the color
            ColorBlock cb = toggle.colors;
            cb.normalColor = c;
            cb.highlightedColor = c * 1.1f;
            cb.pressedColor = c * 0.9f;
            cb.disabledColor = Color.gray;
            cb.colorMultiplier = 1f;
            cb.fadeDuration = 0.1f;
            toggle.colors = cb;
        }

        toggle.onValueChanged.AddListener((isOn) =>
        {
            graph.ToggleSpecies(speciesName, isOn);
        });
    }
}
