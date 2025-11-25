using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    void Start()
    {
        if (PopulationManager.Instance != null)
            PopulationManager.Instance.simStartTime = Time.time;
    }
}
