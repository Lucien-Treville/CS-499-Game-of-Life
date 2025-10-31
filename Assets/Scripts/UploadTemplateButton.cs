using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class UploadTemplateButton : MonoBehaviour
{
    public Button uploadButton;
    private string sourcePath;

    void Start()
    {
        // Path to the user's Downloads folder
        string downloadsFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile) + "\\Downloads";
        sourcePath = Path.Combine(downloadsFolder, "grasslands_template.json");

        uploadButton.onClick.AddListener(UploadTemplate);
    }

    public void UploadTemplate()
    {
        if (File.Exists(sourcePath))
        {
            // Copy the edited file into the persistentDataPath so the Grasslands scene loader can find it
            string destinationPath = Path.Combine(Application.persistentDataPath, "grasslands_template.json");
            File.Copy(sourcePath, destinationPath, true);
            Debug.Log($"✅ Custom template uploaded to: {destinationPath}");

            // Load the Grasslands scene — MapLoader script will automatically rebuild from this JSON
            SceneManager.LoadScene("Grasslands");
        }
        else
        {
            Debug.LogWarning("⚠ No custom template found in Downloads folder: " + sourcePath);
        }
    }
}

