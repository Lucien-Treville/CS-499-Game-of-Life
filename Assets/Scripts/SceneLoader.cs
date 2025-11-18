using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class SceneLoader : MonoBehaviour
{
    public void LoadGrasslandsDemo()
    {
        SceneManager.LoadScene("Grasslands");
    }

    public void DownloadTemplate()
    {
        string outputFileName = "grasslands_template.json";

        // Path to Resources JSON
        string sourcePath = Path.Combine(Application.dataPath, "Resources", outputFileName);

        // Path to Downloads folder
        string downloadsFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile) + "\\Downloads";
        string destinationPath = Path.Combine(downloadsFolder, outputFileName);

        if (File.Exists(sourcePath))
        {
            File.Copy(sourcePath, destinationPath, true);
            Debug.Log($"✅ Template copied to {destinationPath}");
        }
        else
        {
            Debug.LogWarning("⚠ Template file not found at " + sourcePath);
        }
    }
}
    