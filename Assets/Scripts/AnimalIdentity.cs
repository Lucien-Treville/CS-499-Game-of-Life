using UnityEngine;

public class AnimalIdentity : MonoBehaviour
{
    public string speciesName; // Set on prefab

    public void ReportDeath()
    {
        if (PopulationManager.Instance != null)
            PopulationManager.Instance.ReportDeath(speciesName);
    }
}
