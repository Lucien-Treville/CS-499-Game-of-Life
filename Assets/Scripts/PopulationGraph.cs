using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PopulationGraph : MonoBehaviour
{
    [Header("Graph Area")]
    public RectTransform graphArea;

    [Header("Prefabs")]
    public GameObject pointPrefab;
    public GameObject linePrefab;

    [Header("Axis Elements")]
    public RectTransform xAxis;
    public RectTransform yAxis;
    public GameObject xLabelPrefab;
    public GameObject yLabelPrefab;

    public int xLabelCount = 5;
    public int yLabelCount = 5;

    private Dictionary<string, List<GameObject>> activePoints =
        new Dictionary<string, List<GameObject>>();

    private Dictionary<string, List<GameObject>> activeLines =
        new Dictionary<string, List<GameObject>>();

    private float minX, maxX, minY, maxY;

    // Allowed species on this chart
    public readonly string[] AllowedSpecies =
    {
        "Rabbit",
        "Wolf",
        "Tiger",
        "Horse",
        "Snake",
        "Sheep",
        "Grass",
        "BerryBush",
        "AppleTree",
        "Flowers"
    };


    void Start()
    {
        RefreshGraph();
    }

    // =========================================================================
    // MAIN GRAPH RENDERING
    // =========================================================================
    public void RefreshGraph()
    {
        if (PopulationManager.Instance == null)
            return;

        if (!ValidateGraphReferences())
            return;

        CalculateBounds();
        DrawAxes();

        foreach (var entry in PopulationManager.Instance.populationData)
        {
            if (System.Array.Exists(AllowedSpecies, s => s == entry.Key))
            {
                DrawSpecies(entry.Key);
            }
        }
    }

    public void ToggleSpecies(string species, bool isOn)
    {
        if (isOn)
            DrawSpecies(species);
        else
            ClearSpecies(species);
    }

    private void DrawSpecies(string species)
    {
        ClearSpecies(species);

        if (!PopulationManager.Instance.populationData.ContainsKey(species))
            return;

        var stats = PopulationManager.Instance.populationData[species];

        if (stats.history.Count == 0)
            return;

        List<GameObject> points = new List<GameObject>();
        List<GameObject> lines = new List<GameObject>();

        // Graph color matches heatmap + species dictionary
        Color color = PopulationManager.SpeciesColors.ContainsKey(species)
            ? PopulationManager.SpeciesColors[species]
            : Color.white;

        Vector2? prev = null;

        foreach (var entry in stats.history)
        {
            Vector2 pos = TransformPoint(entry.time, entry.count);

            // Create point
            GameObject point = GameObject.Instantiate(pointPrefab, graphArea);
            RectTransform pRT = point.GetComponent<RectTransform>();
            pRT.anchoredPosition = pos;
            point.GetComponent<Image>().color = color;

            points.Add(point);

            // Create line segment
            if (prev.HasValue)
            {
                GameObject seg = GameObject.Instantiate(linePrefab, graphArea);
                RectTransform rt = seg.GetComponent<RectTransform>();

                seg.GetComponent<Image>().color = color;

                Vector2 dir = (pos - prev.Value).normalized;
                float dist = Vector2.Distance(pos, prev.Value);

                rt.sizeDelta = new Vector2(dist, 3f);
                rt.anchoredPosition = (prev.Value + pos) * 0.5f;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                rt.rotation = Quaternion.Euler(0, 0, angle);

                lines.Add(seg);
            }

            prev = pos;
        }

        activePoints[species] = points;
        activeLines[species] = lines;
    }

    // =========================================================================
    // CLEANUP
    // =========================================================================
    public void ClearSpecies(string species)
    {
        if (activePoints.ContainsKey(species))
        {
            foreach (var p in activePoints[species]) GameObject.Destroy(p);
        }
        if (activeLines.ContainsKey(species))
        {
            foreach (var l in activeLines[species]) GameObject.Destroy(l);
        }
    }

    // =========================================================================
    // AXIS LABELS
    // =========================================================================
    private void DrawAxes()
    {
        GenerateXLabels();
        GenerateYLabels();
    }

    private void GenerateXLabels()
    {
        foreach (Transform c in xAxis.transform)
            GameObject.Destroy(c.gameObject);

        float width = xAxis.rect.width;

        for (int i = 0; i < xLabelCount; i++)
        {
            float t = (float)i / (xLabelCount - 1);
            float timeVal = Mathf.Lerp(minX, maxX, t);

            GameObject label = GameObject.Instantiate(xLabelPrefab, xAxis.transform);
            label.GetComponent<Text>().text = timeVal.ToString("F1");

            RectTransform rt = label.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(t * width, 0f);
        }
    }

    private void GenerateYLabels()
    {
        foreach (Transform c in yAxis.transform)
            GameObject.Destroy(c.gameObject);

        float height = yAxis.rect.height;

        for (int i = 0; i < yLabelCount; i++)
        {
            float t = (float)i / (yLabelCount - 1);
            float val = Mathf.Lerp(minY, maxY, t);

            GameObject label = GameObject.Instantiate(yLabelPrefab, yAxis.transform);
            label.GetComponent<Text>().text = Mathf.RoundToInt(val).ToString();

            RectTransform rt = label.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(0f, t * height);
        }
    }

    // =========================================================================
    // BOUNDS + TRANSFORM
    // =========================================================================
    private void CalculateBounds()
    {
        minX = float.MaxValue;
        maxX = float.MinValue;
        minY = float.MaxValue;
        maxY = float.MinValue;

        foreach (var entry in PopulationManager.Instance.populationData)
        {
            if (!System.Array.Exists(AllowedSpecies, s => s == entry.Key))
                continue;

            foreach (var pt in entry.Value.history)
            {
                if (pt.time < minX) minX = pt.time;
                if (pt.time > maxX) maxX = pt.time;

                if (pt.count < minY) minY = pt.count;
                if (pt.count > maxY) maxY = pt.count;
            }
        }

        if (minX == maxX) maxX += 1f;
        if (minY == maxY) maxY += 1f;
    }

    private Vector2 TransformPoint(float time, int count)
    {
        float width = graphArea.rect.width;
        float height = graphArea.rect.height;

        float nx = Mathf.InverseLerp(minX, maxX, time);
        float ny = Mathf.InverseLerp(minY, maxY, count);

        return new Vector2(nx * width, ny * height);
    }

    private bool ValidateGraphReferences()
    {
        return graphArea != null &&
               xAxis != null &&
               yAxis != null &&
               xLabelPrefab != null &&
               yLabelPrefab != null;
    }
}
