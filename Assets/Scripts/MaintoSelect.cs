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
