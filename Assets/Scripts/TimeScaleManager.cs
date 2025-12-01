using UnityEngine;
using UnityEngine.UI; // Required for Slider
using TMPro;          // Required for TextMeshPro

public class TimeScaleManager : MonoBehaviour
{
    // Drag your UI elements here in the Inspector
    public Slider speedSlider;
    public TextMeshProUGUI speedText;
    public PauseManager pauseManager;

    void Start()
    {
        Time.timeScale = 1.0f;
        
        // Set the slider's initial value to match the game's time scale
        if (speedSlider != null)
        {
            speedSlider.value = Time.timeScale;
            UpdateSpeedText(Time.timeScale);
        }

        // Add a listener to the slider
        // This calls 'OnSpeedSliderChanged' whenever the value changes
        if (speedSlider != null)
        {
            speedSlider.onValueChanged.AddListener(OnSpeedSliderChanged);
        }
    }

    // This method is called by the slider's listener
    public void OnSpeedSliderChanged(float newSpeed)
    {
        // Round to nearest 0.05
        float roundedSpeed = Mathf.Round(newSpeed / 0.05f) * 0.05f;

        // Make slider "sticky" to 1.0
        if (Mathf.Abs(roundedSpeed - 1.0f) < 0.15f)
        {
            roundedSpeed = 1.0f;
        }
    
        // Update the slider value to the rounded value
        if (speedSlider != null)
            speedSlider.value = roundedSpeed;

        // Only update time scale if not paused
        if (pauseManager == null || !pauseManager.IsPaused())
        {
            Time.timeScale = roundedSpeed;
        }
        else
        {
            // If paused, ensure time scale remains 0
            pauseManager.timeScaleBeforePause = roundedSpeed;
        }

        UpdateSpeedText(roundedSpeed);
    }

    // A helper function to format the text
    private void UpdateSpeedText(float speed)
    {
        if (speedText != null)
        {
            if (speed == 10f) speedText.text = speed.ToString("F1") + 'x';
            else speedText.text = speed.ToString("F2") + "x";
        }
    }
}