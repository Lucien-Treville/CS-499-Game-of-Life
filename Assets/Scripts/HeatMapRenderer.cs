using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HeatMapRenderer : MonoBehaviour
{
    public RawImage heatmapImage;
    public int textureSize = 1024;

    private Texture2D tex;

    // Dynamic world bounds (auto-detected)
    float dynamicMinX = float.MaxValue;
    float dynamicMaxX = float.MinValue;
    float dynamicMinZ = float.MaxValue;
    float dynamicMaxZ = float.MinValue;

    void OnEnable()
    {
        Debug.Log("HeatMapRenderer OnEnable() triggered");
        GenerateHeatMap();
    }

    void GenerateHeatMap()
    {
        Debug.Log("Heatmap paths count = " + MovementTracker.paths.Count);

        // ------------------------------------------
        // STEP 1 — Detect actual world bounds
        // ------------------------------------------
        foreach (var kvp in MovementTracker.paths)
        {
            foreach (var p in kvp.Value)
            {
                if (p.x < dynamicMinX) dynamicMinX = p.x;
                if (p.x > dynamicMaxX) dynamicMaxX = p.x;

                if (p.z < dynamicMinZ) dynamicMinZ = p.z;
                if (p.z > dynamicMaxZ) dynamicMaxZ = p.z;
            }
        }

        Debug.Log($"Dynamic bounds: X({dynamicMinX}, {dynamicMaxX})  Z({dynamicMinZ}, {dynamicMaxZ})");

        // ------------------------------------------
        // STEP 2 — Create blank texture
        // ------------------------------------------
        tex = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false);

        Color clear = new Color(0, 0, 0, 0);
        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                tex.SetPixel(x, y, clear);
            }
        }

        // ------------------------------------------
        // STEP 3 — Draw each creature's path
        // ------------------------------------------
        foreach (var kvp in MovementTracker.paths)
        {
            DrawPath(kvp.Value, Color.black);
        }

        // ------------------------------------------
        // STEP 4 — Apply texture to UI
        // ------------------------------------------
        tex.Apply();
        heatmapImage.texture = tex;
    }

    // Convert world → texture coordinate
    Vector2 WorldToTex(Vector3 pos)
    {
        float u = Mathf.InverseLerp(dynamicMinX, dynamicMaxX, pos.x);
        float v = Mathf.InverseLerp(dynamicMinZ, dynamicMaxZ, pos.z);

        return new Vector2(u * textureSize, v * textureSize);
    }

    // Draw all segments of a path
    void DrawPath(List<Vector3> points, Color color)
    {
        if (points.Count < 2)
            return;

        for (int i = 0; i < points.Count - 1; i++)
        {
            Vector2 p1 = WorldToTex(points[i]);
            Vector2 p2 = WorldToTex(points[i + 1]);

            DrawLine((int)p1.x, (int)p1.y, (int)p2.x, (int)p2.y, color);
        }
    }

    // Draw a single pixel line (Bresenham)
    void DrawLine(int x0, int y0, int x1, int y1, Color c)
    {
        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            // Safe check (avoid drawing outside texture)
            if (x0 >= 0 && x0 < textureSize && y0 >= 0 && y0 < textureSize)
                tex.SetPixel(x0, y0, c);

            if (x0 == x1 && y0 == y1) break;

            int e2 = 2 * err;
            if (e2 > -dy) { err -= dy; x0 += sx; }
            if (e2 < dx) { err += dx; y0 += sy; }
        }
    }
}
