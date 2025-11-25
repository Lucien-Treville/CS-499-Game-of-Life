using UnityEngine;
using UnityEngine.UI;
using TMPro;   // <-- TMP support
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

    [Header("Axis Settings")]
    public int xLabelCount = 5;
    public int yLabelCount = 5;

    private Dictionary<string, List<GameObject>> activePoints = new Dictionary<string, List<GameObject>>();
    private Dictionary<string, List<GameObject>> activeLines = new Dictionary<string, List<GameObject>>();

    private float minX, maxX, minY, maxY;

    void Start()
    {
        RefreshGraph();
    }

    public void ToggleSpecies(string species, bool on)
    {
        if (on)
            DrawSpecies(species);
        else
            ClearSpecies(species);
    }

    public void RefreshGraph()
    {
        if (PopulationManager.Instance == null)
        {
            Debug.LogWarning("PopulationGraph: PopulationManager.Instance is NULL.");
            return;
        }

        if (!ValidateGraphReferences())
            return;

        CalculateBounds();
        DrawAxes();

        foreach (var sp in PopulationManager.Instance.populationData)
            DrawSpecies(sp.Key);
    }

    void DrawSpecies(string species)
    {
        ClearSpecies(species);

        if (!PopulationManager.Instance.populationData.ContainsKey(species))
            return;

        var stats = PopulationManager.Instance.populationData[species];
        if (stats.history.Count == 0)
            return;

        List<GameObject> points = new List<GameObject>();
        List<GameObject> lines = new List<GameObject>();

        Vector2? prev = null;

        foreach (var entry in stats.history)
        {
            Vector2 pos = TransformPoint(entry.time, entry.count);

            GameObject point = Instantiate(pointPrefab, graphArea);
            RectTransform ptRT = point.GetComponent<RectTransform>();
            ptRT.anchoredPosition = pos;
            point.transform.localScale = Vector3.one;

            Color color = Random.ColorHSV(0f, 1f, 0.6f, 1f);
            point.GetComponent<Image>().color = color;

            points.Add(point);

            if (prev.HasValue)
            {
                GameObject seg = Instantiate(linePrefab, graphArea);
                RectTransform rt = seg.GetComponent<RectTransform>();

                seg.GetComponent<Image>().color = color;

                Vector2 dir = (pos - prev.Value).normalized;
                float dist = Vector2.Distance(pos, prev.Value);

                rt.sizeDelta = new Vector2(dist, 3f);
                rt.anchoredPosition = (prev.Value + pos) * 0.5f;
                rt.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);

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
            foreach (var p in activePoints[species]) Destroy(p);
            foreach (var l in activeLines[species]) Destroy(l);
        }
    }

    Vector2 TransformPoint(float time, int count)
    {
        // Safety
        if (graphArea == null)
            return Vector2.zero;

        float x0 = 0f;
        float y0 = 0f;

        float width = graphArea.rect.width;
        float height = graphArea.rect.height;

        float normX = Mathf.InverseLerp(minX, maxX, time);
        float normY = Mathf.InverseLerp(minY, maxY, count);

        float x = x0 + (normX * width);
        float y = y0 + (normY * height);

        return new Vector2(x, y);
    }

    void CalculateBounds()
    {
        minX = minY = float.MaxValue;
        maxX = maxY = float.MinValue;

        foreach (var sp in PopulationManager.Instance.populationData)
        {
            foreach (var pt in sp.Value.history)
            {
                if (pt.time < minX) minX = pt.time;
                if (pt.time > maxX) maxX = pt.time;
                if (pt.count < minY) minY = pt.count;
                if (pt.count > minY) maxY = pt.count;
            }
        }

        if (minX == maxX) maxX += 1f;
        if (minY == maxY) maxY += 1f;
    }

    void DrawAxes()
    {
        if (!ValidateGraphReferences()) return;

        GenerateXLabels();
        GenerateYLabels();
    }

    // =======================================================
    // TMP-SAFE LABEL CREATION
    // =======================================================
    void SetLabelText(GameObject labelObj, string value)
    {
        TMP_Text tmp = labelObj.GetComponent<TMP_Text>();
        if (tmp != null)
        {
            tmp.text = value;
            return;
        }

        Text legacy = labelObj.GetComponent<Text>();
        if (legacy != null)
        {
            legacy.text = value;
            return;
        }

        Debug.LogWarning("Label prefab has no Text or TextMeshProUGUI!", labelObj);
    }

    void GenerateXLabels()
    {
        foreach (Transform c in xAxis.transform)
            Destroy(c.gameObject);

        float width = xAxis.rect.width;

        for (int i = 0; i < xLabelCount; i++)
        {
            float t = (float)i / (xLabelCount - 1);
            float timeVal = Mathf.Lerp(minX, maxX, t);

            GameObject label = Instantiate(xLabelPrefab, xAxis.transform);
            label.GetComponent<Text>().text = timeVal.ToString("F1");

            RectTransform rt = label.GetComponent<RectTransform>();

            rt.anchorMin = new Vector2(0, 0);
            rt.anchorMax = new Vector2(0, 0);
            rt.pivot = new Vector2(0.5f, 0);

            // Proper spacing across the entire bottom width
            rt.anchoredPosition = new Vector2(t * width, 0f);
        }
    }

    void GenerateYLabels()
    {
        foreach (Transform c in yAxis.transform)
            Destroy(c.gameObject);

        float height = yAxis.rect.height;

        for (int i = 0; i < yLabelCount; i++)
        {
            float t = (float)i / (yLabelCount - 1);
            float popVal = Mathf.Lerp(minY, maxY, t);

            GameObject label = Instantiate(yLabelPrefab, yAxis.transform);
            label.GetComponent<Text>().text = Mathf.RoundToInt(popVal).ToString();

            RectTransform rt = label.GetComponent<RectTransform>();

            rt.anchorMin = new Vector2(1, 0);
            rt.anchorMax = new Vector2(1, 0);
            rt.pivot = new Vector2(0, 0.5f);

            // Proper spacing vertically
            rt.anchoredPosition = new Vector2(0f, t * height);
        }
    }


    bool ValidateGraphReferences()
    {
        if (graphArea == null) { Debug.LogWarning("graphArea missing"); return false; }
        if (xAxis == null) { Debug.LogWarning("xAxis missing"); return false; }
        if (yAxis == null) { Debug.LogWarning("yAxis missing"); return false; }
        if (xLabelPrefab == null) { Debug.LogWarning("xLabelPrefab missing"); return false; }
        if (yLabelPrefab == null) { Debug.LogWarning("yLabelPrefab missing"); return false; }

        return true;
    }
}
