using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class DownloadTemplateButton : MonoBehaviour
{
    public Button downloadButton;
    private string sourcePath;
    private string destinationPath;

    void Start()
    {
        // Source = exported JSON in your project
        sourcePath = Path.Combine(Application.dataPath, "Resources", "grasslands_template.json");

        // Destination = user's Downloads folder
        string downloadsFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile) + "\\Downloads";
        destinationPath = Path.Combine(downloadsFolder, "grasslands_template.json");

        downloadButton.onClick.AddListener(DownloadTemplate);
    }


    public void DownloadTemplate()
    {
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
