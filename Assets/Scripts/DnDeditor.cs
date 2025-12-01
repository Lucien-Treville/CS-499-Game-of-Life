using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DnDeditor : MonoBehaviour
{
    public static DnDeditor Instance;
    public static bool startInEditMode = false;
    public GameObject startSimButton;

    void Awake()
    {
        // Basic Singleton Setup
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (startInEditMode)
        {
            // Logic for Drag and Drop Mode
            // Debug.Log("Mode: Editor. Simulation Paused.");
            Time.timeScale = 0f;
            // activate start simulation button
            startSimButton.SetActive(true);
        }
        else
        {
            startSimButton.SetActive(false);
            // Logic for Normal Run
            // Debug.Log("Mode: Simulation. Running normally.");
            Time.timeScale = 1f;
        }

        // Reset the flag for next time
        startInEditMode = false;
    }

    public void OnStartSimButton()
    {
        Time.timeScale = 1f;
        // disable Start sim button
        startSimButton.SetActive(false);
        // update population manager stats
        foreach (var specie in PopulationManager.Instance.populationData.Keys)
        {
            PopulationManager.Instance.populationData[specie].UpdateStats();
        }
    }

}
