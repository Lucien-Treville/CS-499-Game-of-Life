using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeatMapRenderer : MonoBehaviour
{
    public RawImage heatmapImage;

    [Header("Legend UI")]
    public RectTransform legendContainer; // Panel for legend entries
    public Font legendFont;               // Your UI font

    [Header("Settings")]
    public int textureSize = 1024;

    private Texture2D tex;
    private float minX, maxX, minZ, maxZ;

    // --------- PLANTS TO IGNORE ----------
    private static readonly HashSet<string> plantSpecies = new HashSet<string>
    {
        "BerryBush","Flowers","flowers2","flowers-tall",
        "AppleTree","tree","tree-pine","rock","rocks"
    };

    // --------- ANIMAL SPECIES COLORS ----------
    // NOTE: keys MUST match the parsed species names (see ParseSpeciesName).
    public static readonly Dictionary<string, Color> speciesColors = new Dictionary<string, Color>
{
    { "Rabbit",    new Color(1f, 0.9f, 0.2f) },     // Yellow
    { "Wolf",      new Color(0.25f, 0.35f, 0.55f) }, // Dark Gray-Blue
    { "Tiger",     new Color(1f, 0.55f, 0.0f) },     // Orange
    { "Horse",     new Color(0.2f, 0.4f, 1f) },      // Blue
    { "Snake",     new Color(0.6f, 0.2f, 0.8f) },    // Purple

    // NEW CREATURES
    { "Sheep",       new Color(0.6f, 0.4f, 0.2f) },     // Brown
};



    private const int SPREAD_RADIUS = 3;

    // species that actually appeared in the data *and* had a defined color
    private readonly HashSet<string> activeColoredSpecies = new HashSet<string>();


    void OnEnable()
    {
        Debug.Log("🔥 HeatMapRenderer ENABLED");
        GenerateHeatMap();
        GenerateLegendUI();
    }

    // ==========================================================
    // 🔥 HEATMAP GENERATION
    // ==========================================================
    void GenerateHeatMap()
    {
        Debug.Log("📌 PATH COUNT = " + MovementTracker.paths.Count);

        activeColoredSpecies.Clear();

        if (MovementTracker.paths.Count == 0)
        {
            Debug.LogWarning("⚠️ No movement data found!");
            return;
        }

        ComputeBounds();

        Texture2D bg = Resources.Load<Texture2D>("MapImages/GrasslandsMap");
        if (bg == null)
        {
            Debug.LogError("❌ GrasslandsMap.png NOT FOUND in Resources/MapImages/");
            return;
        }

        tex = new Texture2D(textureSize, textureSize);
        Color[,] colorMap = new Color[textureSize, textureSize];
        float[,] weightMap = new float[textureSize, textureSize];

        // ---------------- Build colored density ----------------
        foreach (var kvp in MovementTracker.paths)
        {
            string species = ParseSpeciesName(kvp.Key);

            // skip plants entirely
            if (plantSpecies.Contains(species))
                continue;

            bool hasColor = speciesColors.TryGetValue(species, out Color speciesColor);
            if (!hasColor)
            {
                // This is where your rabbits were likely going white:
                Debug.LogWarning($"⚠️ No color defined for species '{species}'. Using white on map and skipping from legend.");
                speciesColor = Color.white;
            }
            else
            {
                // only species with a defined color go into legend
                activeColoredSpecies.Add(species);
            }

            foreach (Vector3 pos in kvp.Value)
            {
                Vector2Int center = WorldToPixel(pos);

                for (int y = center.y - SPREAD_RADIUS; y <= center.y + SPREAD_RADIUS; y++)
                {
                    for (int x = center.x - SPREAD_RADIUS; x <= center.x + SPREAD_RADIUS; x++)
                    {
                        if (!Inside(x, y)) continue;

                        float dx = x - center.x;
                        float dy = y - center.y;
                        float distSq = dx * dx + dy * dy;

                        if (distSq <= SPREAD_RADIUS * SPREAD_RADIUS)
                        {
                            colorMap[x, y] += speciesColor;
                            weightMap[x, y] += 1f;
                        }
                    }
                }
            }
        }

        // ---------------- Draw final blended texture ----------------
        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                float u = (float)x / textureSize;
                float v = (float)y / textureSize;

                Color baseC = bg.GetPixelBilinear(u, v);

                if (weightMap[x, y] > 0)
                {
                    Color blended = colorMap[x, y] / weightMap[x, y];
                    blended.a = 0.75f;

                    Color final = Color.Lerp(baseC, blended, blended.a);
                    tex.SetPixel(x, y, final);
                }
                else
                {
                    tex.SetPixel(x, y, baseC);
                }
            }
        }

        tex.Apply();
        heatmapImage.texture = tex;

        Debug.Log("✅ Heatmap applied!");
    }

    // ==========================================================
    // 🏷️ LEGEND UI CREATION
    // ==========================================================
    void GenerateLegendUI()
    {
        if (legendContainer == null)
        {
            Debug.LogWarning("⚠️ No legendContainer assigned — skipping legend.");
            return;
        }

        // Clear old entries
        foreach (Transform child in legendContainer)
            Destroy(child.gameObject);

        // Ensure parent has VerticalLayoutGroup
        VerticalLayoutGroup vlg = legendContainer.GetComponent<VerticalLayoutGroup>();
        if (vlg == null)
            vlg = legendContainer.gameObject.AddComponent<VerticalLayoutGroup>();

        vlg.spacing = 10;
        vlg.childAlignment = TextAnchor.UpperLeft;

        // Sort species for consistent order
        List<string> speciesList = new List<string>(activeColoredSpecies);
        speciesList.Sort();

        foreach (string species in speciesList)
        {
            if (speciesColors.TryGetValue(species, out Color c))
                CreateLegendEntry(species, c);
        }
    }

    void CreateLegendEntry(string species, Color color)
    {
        GameObject entry = new GameObject(species);
        RectTransform rt = entry.AddComponent<RectTransform>();
        entry.transform.SetParent(legendContainer, false);

        HorizontalLayoutGroup hlg = entry.AddComponent<HorizontalLayoutGroup>();
        hlg.childAlignment = TextAnchor.MiddleLeft;
        hlg.spacing = 12;
        hlg.childControlWidth = false;
        hlg.childControlHeight = false;

        // Color box
        GameObject colorBox = new GameObject("ColorBox");
        colorBox.transform.SetParent(entry.transform, false);

        Image boxImg = colorBox.AddComponent<Image>();
        boxImg.color = color;

        RectTransform boxRT = boxImg.GetComponent<RectTransform>();
        boxRT.sizeDelta = new Vector2(30, 30);

        // Label
        GameObject labelObj = new GameObject("Label");
        labelObj.transform.SetParent(entry.transform, false);

        Text label = labelObj.AddComponent<Text>();
        label.text = species;
        label.font = legendFont;
        label.color = Color.black;
        label.fontSize = 26;
    }

    // ==========================================================
    // UTILITIES
    // ==========================================================

    // Parse the base species name from the MovementTracker key.
    // Example IDs:
    //   "Rabbit_0"                -> "Rabbit"
    //   "Dog_001(Clone)_3"        -> "Dog"
    //   "RabbitPrefab(Clone)_12"  -> "RabbitPrefab"
    string ParseSpeciesName(string id)
    {
        // 1. Split by underscore: "Rabbit_3" → "Rabbit"
        string baseName = id.Split('_')[0];

        // 2. Remove "(Clone)" → "Rabbit(Clone)" → "Rabbit"
        baseName = baseName.Replace("(Clone)", "");

        // 3. Remove "Prefab" or "prefab"
        baseName = baseName.Replace("Prefab", "");
        baseName = baseName.Replace("prefab", "");

        // 4. Remove numbers at end: "Rabbit001" → "Rabbit"
        while (baseName.Length > 0 && char.IsDigit(baseName[baseName.Length - 1]))
            baseName = baseName.Substring(0, baseName.Length - 1);

        // 5. Trim whitespace just in case
        baseName = baseName.Trim();

        return baseName;
    }


    Vector2Int WorldToPixel(Vector3 pos)
    {
        float u = Mathf.InverseLerp(minX, maxX, pos.x);
        float v = Mathf.InverseLerp(minZ, maxZ, pos.z);

        return new Vector2Int(
            Mathf.RoundToInt(u * (textureSize - 1)),
            Mathf.RoundToInt(v * (textureSize - 1))
        );
    }

    void ComputeBounds()
    {
        minX = minZ = float.PositiveInfinity;
        maxX = maxZ = float.NegativeInfinity;

        foreach (var kvp in MovementTracker.paths)
        {
            foreach (Vector3 p in kvp.Value)
            {
                if (p.x < minX) minX = p.x;
                if (p.x > maxX) maxX = p.x;

                if (p.z < minZ) minZ = p.z;
                if (p.z > maxZ) maxZ = p.z;
            }
        }

        if (Mathf.Approximately(minX, maxX)) { minX -= 1; maxX += 1; }
        if (Mathf.Approximately(minZ, maxZ)) { minZ -= 1; maxZ += 1; }

        Debug.Log($"📏 Bounds: X({minX}, {maxX}) Z({minZ}, {maxZ})");
    }

    bool Inside(int x, int y)
    {
        return (x >= 0 && x < textureSize && y >= 0 && y < textureSize);
    }
}
