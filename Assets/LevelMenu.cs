using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelMenu : MonoBehaviour
{
    // Called when a level button is clicked
    public void OpenLevel(int levelId)
    {
        string sceneName = GetSceneName(levelId);
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Invalid level ID selected: " + levelId);
        }
    }

    // Maps level IDs to their correct scene names
    private string GetSceneName(int levelId)
    {
        switch (levelId)
        {
            case 1:
                return "Grasslands";
            case 2:
                return "Tundra";
            case 3:
                return "Swamp";
            case 4:
                return "Desert";

            // "Build My Own Map" options — all go to Blank
            case 5:
            case 6:
            case 7:
            case 8:
                return "Blank";

            default:
                return null;
        }
    }
}
