using UnityEngine;

public class ChartSelector : MonoBehaviour
{
    public GameObject populationChart;
    public GameObject heatMap;

    void Start()
    {
        ShowPopulation();
    }

    public void ShowPopulation()
    {
        populationChart.SetActive(true);
        heatMap.SetActive(false);
    }

    public void ShowHeatMap()
    {
        populationChart.SetActive(false);
        heatMap.SetActive(true);
    }
}
