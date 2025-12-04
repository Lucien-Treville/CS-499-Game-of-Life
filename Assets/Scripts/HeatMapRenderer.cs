using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeatMapRenderer : MonoBehaviour
{
    public RawImage heatmapImage;

    [Header("Legend UI")]
    public RectTransform legendContainer;
    public Font legendFont;

    [Header("Settings")]
    public int textureSize = 1024;

    private Texture2D tex;
    private float minX, maxX, minZ, maxZ;

    // --------- PLANTS TO IGNORE ----------
    private static readonly HashSet<string> plantSpecies =
        new HashSet<string>()
        {
            "BerryBush","Flowers","flowers2","flowers-tall",
            "AppleTree","tree","tree-pine","rock","rocks"
        };

    // --------- ANIMAL SPECIES COLORS ----------
    public static readonly Dictionary<string, Color> speciesColors =
        new Dictionary<string, Color>()
        {
            { "Rabbit", new Color(1f, 0.9f, 0.2f) },
            { "Wolf",   new Color(0.25f, 0.35f, 0.55f) },
            { "Tiger",  new Color(1f, 0.55f, 0.0f) },
            { "Horse",  new Color(0.2f, 0.4f, 1f) },
            { "Snake",  new Color(0.6f, 0.2f, 0.8f) },
            { "Sheep",  new Color(0.6f, 0.4f, 0.2f) }
        };

    private const int SPREAD_RADIUS = 3;

    private readonly HashSet<string> activeColoredSpecies =
        new HashSet<string>();


    // =====================================================
    // ENABLE
    // =====================================================
    void OnEnable()
    {
        Debug.Log("🔥 HeatMapRenderer ENABLED");
        GenerateHeatMap();
        GenerateLegendUI();
    }


    // =====================================================
    // HEATMAP GENERATION
    // =====================================================
    void GenerateHeatMap()
    {
        Debug.Log("📌 PATH COUNT = " + MovementTracker.paths.Count);
        activeColoredSpecies.Clear();

        if (MovementTracker.paths.Count == 0)
        {
            Debug.LogWarning("⚠ No movement data found. Heatmap skipped.");
            return;
        }

        ComputeBounds();

        // Background must be inside:
        // Assets/Resources/MapImages/GrasslandsMap.png
        Texture2D bg = Resources.Load<Texture2D>("MapImages/GrasslandsMap");

        if (bg == null)
        {
            Debug.LogError("❌ GrasslandsMap.png NOT FOUND in Resources/MapImages/");
            return;
        }

        tex = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false);

        Color[,] colorMap = new Color[textureSize, textureSize];
        float[,] weightMap = new float[textureSize, textureSize];

        // ---------- Build density ----------
        foreach (KeyValuePair<string, List<Vector3>> kvp in MovementTracker.paths)
        {
            string species = ParseSpeciesName(kvp.Key);

            if (plantSpecies.Contains(species))
                continue;

            Color speciesColor;
            bool hasColor = speciesColors.TryGetValue(species, out speciesColor);

            if (!hasColor)
            {
                Debug.LogWarning("⚠ No color for species: " + species);
                speciesColor = Color.white;
            }
            else
                activeColoredSpecies.Add(species);

            List<Vector3> path = kvp.Value;
            for (int i = 0; i < path.Count; i++)
            {
                Vector3 pos = path[i];
                Vector2Int center = WorldToPixel(pos);

                for (int y = center.y - SPREAD_RADIUS; y <= center.y + SPREAD_RADIUS; y++)
                {
                    for (int x = center.x - SPREAD_RADIUS; x <= center.x + SPREAD_RADIUS; x++)
                    {
                        if (!Inside(x, y))
                            continue;

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

        // ---------- Final blend ----------
        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                float u = (float)x / (float)textureSize;
                float v = (float)y / (float)textureSize;

                Color baseC = bg.GetPixelBilinear(u, v);

                if (weightMap[x, y] > 0)
                {
                    // Average the contributing species color
                    Color blended = colorMap[x, y] / weightMap[x, y];

                    // Darken proportionally to overlap count (more paths → darker)
                    // Tune 'darkenScale' to your dataset; higher value darkens faster.
                    float darkenScale = 0.08f; // e.g., 12 hits → ~50% toward black
                    float darkness = Mathf.Clamp01(weightMap[x, y] * darkenScale);
                    blended = Color.Lerp(blended, Color.black, darkness);

                    // Keep a consistent overlay strength
                    blended.a = 0.75f;

                    // Compose over background
                    Color finalC = Color.Lerp(baseC, blended, blended.a);
                    tex.SetPixel(x, y, finalC);
                }
                else
                {
                    tex.SetPixel(x, y, baseC);
                }
            }
        }

        tex.Apply();
        heatmapImage.texture = tex;

        Debug.Log("✅ Heatmap completed");
    }


    // =====================================================
    // LEGEND UI
    // =====================================================
    void GenerateLegendUI()
    {
        if (legendContainer == null)
        {
            Debug.LogWarning("⚠ No legend container");
            return;
        }

        foreach (Transform t in legendContainer)
            Destroy(t.gameObject);

        VerticalLayoutGroup vlg = legendContainer.GetComponent<VerticalLayoutGroup>();
        if (vlg == null)
            vlg = legendContainer.gameObject.AddComponent<VerticalLayoutGroup>();

        vlg.spacing = 2;
        vlg.childAlignment = TextAnchor.UpperLeft;

        List<string> list = new List<string>(activeColoredSpecies);
        list.Sort();

        for (int i = 0; i < list.Count; i++)
        {
            string species = list[i];
            Color c;

            if (speciesColors.TryGetValue(species, out c))
                CreateLegendEntry(species, c);
        }
    }


    void CreateLegendEntry(string species, Color color)
    {
        GameObject entry = new GameObject(species);
        entry.transform.SetParent(legendContainer, false);

        HorizontalLayoutGroup hlg = entry.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = 12;
        hlg.childAlignment = TextAnchor.MiddleLeft;

        // color box
        GameObject box = new GameObject("ColorBox");
        box.transform.SetParent(entry.transform, false);

        Image boxImg = box.AddComponent<Image>();
        boxImg.color = color;
        boxImg.rectTransform.sizeDelta = new Vector2(30, 30);

        // label
        GameObject labelObj = new GameObject("Label");
        labelObj.transform.SetParent(entry.transform, false);

        Text label = labelObj.AddComponent<Text>();
        label.text = species;
        label.font = legendFont;
        label.fontSize = 26;
        label.color = Color.black;
    }


    // =====================================================
    // UTILITIES
    // =====================================================
    string ParseSpeciesName(string id)
    {
        string baseName = id.Split('_')[0];
        baseName = baseName.Replace("(Clone)", "");
        baseName = baseName.Replace("Prefab", "");
        baseName = baseName.Replace("prefab", "");

        while (baseName.Length > 0 &&
               char.IsDigit(baseName[baseName.Length - 1]))
        {
            baseName = baseName.Substring(0, baseName.Length - 1);
        }

        return baseName.Trim();
    }

    Vector2Int WorldToPixel(Vector3 pos)
    {
        float u = Mathf.InverseLerp(minX, maxX, pos.x);
        float v = Mathf.InverseLerp(minZ, maxZ, pos.z);

        int px = Mathf.RoundToInt(u * (textureSize - 1));
        int py = Mathf.RoundToInt(v * (textureSize - 1));

        return new Vector2Int(px, py);
    }

    void ComputeBounds()
    {
        minX = float.PositiveInfinity;
        maxX = float.NegativeInfinity;

        minZ = float.PositiveInfinity;
        maxZ = float.NegativeInfinity;

        foreach (KeyValuePair<string, List<Vector3>> kvp in MovementTracker.paths)
        {
            List<Vector3> path = kvp.Value;

            for (int i = 0; i < path.Count; i++)
            {
                Vector3 p = path[i];

                if (p.x < minX) minX = p.x;
                if (p.x > maxX) maxX = p.x;

                if (p.z < minZ) minZ = p.z;
                if (p.z > maxZ) maxZ = p.z;
            }
        }

        if (Mathf.Approximately(minX, maxX))
        {
            minX -= 1;
            maxX += 1;
        }

        if (Mathf.Approximately(minZ, maxZ))
        {
            minZ -= 1;
            maxZ += 1;
        }

        Debug.Log("📏 Bounds X(" + minX + "," + maxX + ") Z(" + minZ + "," + maxZ + ")");
    }

    bool Inside(int x, int y)
    {
        return (x >= 0 && x < textureSize && y >= 0 && y < textureSize);
    }
}
