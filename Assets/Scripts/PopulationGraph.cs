using UnityEngine;
using UnityEngine.UI;
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

    // EXACT species names
    public readonly string[] AllowedSpecies =
    {
        "Rabbit",
        "Wolf",
        "Tiger",
        "Horse",
        "Snake",
        "Sheep",
        "Grass",
        "Berry Bush",
        "Apple Tree",
        "Flowers"
    };

    void Start()
    {
        RefreshGraph();
    }

    public void RefreshGraph()
    {
        if (!ValidateGraphReferences())
            return;

        if (PopulationManager.Instance == null)
            return;

        CalculateBounds();
        DrawAxes();

        foreach (var entry in PopulationManager.Instance.populationData)
        {
            if (System.Array.Exists(AllowedSpecies, s => s == entry.Key))
                DrawSpecies(entry.Key);
        }
    }

    public void ToggleSpecies(string species, bool on)
    {
        if (on) DrawSpecies(species);
        else ClearSpecies(species);
    }

    private void DrawSpecies(string species)
    {
        ClearSpecies(species);

        if (!PopulationManager.Instance.populationData.ContainsKey(species))
            return;

        SpeciesStats stats = PopulationManager.Instance.populationData[species];
        if (stats.history.Count == 0)
            return;

        List<GameObject> points = new List<GameObject>();
        List<GameObject> lines = new List<GameObject>();

        Color color = PopulationManager.SpeciesColors.ContainsKey(species)
            ? PopulationManager.SpeciesColors[species]
            : Color.white;

        Vector2? prev = null;

        foreach (PopulationHistoryPoint entry in stats.history)
        {
            Vector2 pos = TransformPoint(entry.time, entry.count);

            GameObject p = Instantiate(pointPrefab, graphArea);
            p.GetComponent<Image>().color = color;
            p.GetComponent<RectTransform>().anchoredPosition = pos;
            points.Add(p);

            if (prev.HasValue)
            {
                GameObject seg = Instantiate(linePrefab, graphArea);
                Image img = seg.GetComponent<Image>();
                img.color = color;

                RectTransform rt = seg.GetComponent<RectTransform>();
                Vector2 dir = (pos - prev.Value).normalized;
                float dist = Vector2.Distance(prev.Value, pos);

                rt.sizeDelta = new Vector2(dist, 3f);
                rt.anchoredPosition = (prev.Value + pos) / 2f;
                rt.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);

                lines.Add(seg);
            }

            prev = pos;
        }

        activePoints[species] = points;
        activeLines[species] = lines;
    }

    public void ClearSpecies(string species)
    {
        if (activePoints.ContainsKey(species))
        {
            foreach (GameObject g in activePoints[species])
                Destroy(g);
        }

        if (activeLines.ContainsKey(species))
        {
            foreach (GameObject g in activeLines[species])
                Destroy(g);
        }
    }

    private void DrawAxes()
    {
        GenerateXLabels();
        GenerateYLabels();
    }

    private void GenerateXLabels()
    {
        foreach (Transform c in xAxis.transform)
            Destroy(c.gameObject);

        float width = xAxis.rect.width;

        for (int i = 0; i < xLabelCount; i++)
        {
            float t = (float)i / (xLabelCount - 1);
            float val = Mathf.Lerp(minX, maxX, t);

            GameObject label = Instantiate(xLabelPrefab, xAxis.transform);
            label.GetComponent<Text>().text = val.ToString("F1");

            RectTransform rt = label.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(t * width, 0f);
        }
    }

    private void GenerateYLabels()
    {
        foreach (Transform c in yAxis.transform)
            Destroy(c.gameObject);

        float height = yAxis.rect.height;

        for (int i = 0; i < yLabelCount; i++)
        {
            float t = (float)i / (yLabelCount - 1);
            float val = Mathf.Lerp(minY, maxY, t);

            GameObject label = Instantiate(yLabelPrefab, yAxis.transform);
            label.GetComponent<Text>().text = Mathf.RoundToInt(val).ToString();

            RectTransform rt = label.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(0f, t * height);
        }
    }

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

            foreach (PopulationHistoryPoint pt in entry.Value.history)
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
        float w = graphArea.rect.width;
        float h = graphArea.rect.height;

        float nx = Mathf.InverseLerp(minX, maxX, time);
        float ny = Mathf.InverseLerp(minY, maxY, count);

        return new Vector2(nx * w, ny * h);
    }

    private bool ValidateGraphReferences()
    {
        return graphArea && xAxis && yAxis && xLabelPrefab && yLabelPrefab;
    }
}
