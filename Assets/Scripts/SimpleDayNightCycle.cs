using UnityEngine;

public class SimpleDayNightCycle : MonoBehaviour
{
    [Header("References")]
    public Light sunLight; // Drag your directional light here

    [Header("Cycle Settings")]
    public float dayDuration = 120f; // Seconds for a full 24h cycle
    
    [Header("Visuals")]
    public Gradient lightColor; // Controls color over time (Sunrise -> Day -> Sunset -> Night)
    public AnimationCurve lightIntensity; // Controls brightness over time

    [Header("Debug")]
    [Range(0, 1)] 
    public float timeOfDay = 0f; // 0.0 = Start, 0.5 = Noon, 1.0 = End

    private float timer;

    void Start()
    {
        if (sunLight == null) sunLight = GetComponent<Light>();
        timer = dayDuration * 0.5f; // just so we start at noon
    }

    void Update()
    {
        // 1. Advance Time
        timer += Time.deltaTime;
        
        // Calculate normalized time (0.0 to 1.0) based on duration
        // The modulo (%) operator ensures it loops back to 0 when it hits the limit
        timeOfDay = (timer % dayDuration) / dayDuration;

        // 2. Apply Color (Evaluate the gradient at the current % time)
        if (sunLight != null)
        {
            sunLight.color = lightColor.Evaluate(timeOfDay);
            sunLight.intensity = lightIntensity.Evaluate(timeOfDay);
        }
    }
}