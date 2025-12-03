using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MaintoSelect : MonoBehaviour
{

    public void OnPlayButton()
    {
        // call select scene
        SceneManager.LoadScene("MapMenuScreen");
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }

    public void OnMainMenuButton()
    {
        if (PopulationManager.Instance != null)
        {
            Destroy(PopulationManager.Instance.gameObject);
        }
        

        // 1. THE TRICK: Accessing the secret DDOL Scene
        // We create a temp object and send it to DDOL so we can grab the scene handle.
        GameObject temp = new GameObject("TempDDOLReference");
        DontDestroyOnLoad(temp);
        
        Scene ddolScene = temp.scene;
        
        // Destroy the temp object immediately, we don't need it anymore
        DestroyImmediate(temp);

        // 2. Get every root object in that scene
        GameObject[] allDDOLObjects = ddolScene.GetRootGameObjects();
        Scene currentScene = SceneManager.GetActiveScene();

        Debug.Log($"Vacuuming {allDDOLObjects.Length} objects from DDOL to {currentScene.name}...");

        foreach (GameObject obj in allDDOLObjects)
        {
            // Move them to the current scene.
            // Now they are no longer immortal. They will die when this scene ends.
            SceneManager.MoveGameObjectToScene(obj, currentScene);
        }

        SceneManager.LoadScene("TitleScreen");
    }

    public void OnReportButton()
    {
        Debug.Log("See Report Button Clicked");
    }


    public void OnEndButton()
    {
        SceneManager.LoadScene("Report");
    }

}
