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
        SceneManager.LoadScene("MainMenu");
    }

    public void OnReportButton()
    {
        Debug.Log("See Report Button Clicked");
    }


}
