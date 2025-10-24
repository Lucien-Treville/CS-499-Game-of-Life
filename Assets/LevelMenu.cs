using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelMenu : MonoBehaviour
{
    // ===== Normal Levels =====
    public void OpenGrasslands() => SceneManager.LoadScene("Grasslands");
    public void OpenTundra() => SceneManager.LoadScene("Tundra");
    public void OpenSwamp() => SceneManager.LoadScene("Swamp");
    public void OpenDesert() => SceneManager.LoadScene("Desert");

    // ===== Build Your Own Levels =====
    public void BuildMyOwn(string baseMap)
    {
        // Save which base map the player selected
        PlayerPrefs.SetString("BaseMap", baseMap);

        // Load the blank builder scene
        SceneManager.LoadScene("Blank");
    }
}
