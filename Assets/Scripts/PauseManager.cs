using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    private bool isPaused = false;
    private float timeScaleBeforePause;

    public Sprite pausedIcon;
    public Sprite unpausedIcon;
    public Image pauseButtonImage;

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
        }
        else
        {
            // Restore the time scale to its previous value
            Time.timeScale = timeScaleBeforePause;
            Debug.Log("Simulation Resumed");
             // Set unpaused sprite
            if (pauseButtonImage != null && unpausedIcon != null)
                pauseButtonImage.sprite = unpausedIcon;
        }
    }

    void Update()
    {

    }
}