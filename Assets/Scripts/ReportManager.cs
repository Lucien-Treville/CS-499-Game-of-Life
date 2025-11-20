using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReportManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (PopulationManager.Instance != null)
        {

            foreach (var entry in PopulationManager.Instance.populationData)
            {
                Debug.Log($"Species: {entry.Key} | Current Count: {entry.Value.currentCount} | Max Recorded: {entry.Value.maxRecorded} | Min Recorded: {entry.Value.minRecorded}");
                foreach (var point in entry.Value.history)
                {
                    Debug.Log($"Species: {entry.Key} | Time: {point.time} | Count: {point.count}");
                }
            }
        }
        else
        {
            Debug.Log("PopulationManager instance not found.");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
