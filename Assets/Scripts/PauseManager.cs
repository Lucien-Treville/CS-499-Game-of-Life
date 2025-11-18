using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PauseManager : MonoBehaviour
{
    private bool isPaused = false;
    public float timeScaleBeforePause;

    public Sprite pausedIcon;
    public Sprite unpausedIcon;
    public Image pauseButtonImage;
    public TextMeshProUGUI TimeValueText;
    public GameObject pauseOverlay;
    public GameObject cameraControlsUI;

    void Start()
    {
        // Initialize with a default value just in case
        timeScaleBeforePause = Time.timeScale;
    }

    public void TogglePause()
    {
        isPaused = !isPaused; // Flip the boolean

        if (isPaused)
        {
            // Store the current time scale *before* pausing
            timeScaleBeforePause = Time.timeScale;

            // Pause the simulation
            Time.timeScale = 0f;
            Debug.Log("Simulation Paused");
            // Set paused sprite
            if (pauseButtonImage != null && pausedIcon != null)
                pauseButtonImage.sprite = pausedIcon;

            // show pause overlay
            if (pauseOverlay != null)
                pauseOverlay.SetActive(true);

            if (cameraControlsUI != null)
                cameraControlsUI.SetActive(true);
        }
        else
        {
            // Restore the time scale to its previous value
            Time.timeScale = timeScaleBeforePause;
            Debug.Log("Simulation Resumed");
            // Set unpaused sprite
            if (pauseButtonImage != null && unpausedIcon != null)
                pauseButtonImage.sprite = unpausedIcon;

            // hide pause overlay
            if (pauseOverlay != null)
                pauseOverlay.SetActive(false);
            
            if (cameraControlsUI != null)
                cameraControlsUI.SetActive(false);
        }
    }

    void Update()
    {
        TimeValueText.text = Time.time.ToString("F1");
    }

    public bool IsPaused()
    {
        return isPaused;
    }
}