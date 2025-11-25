using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class SceneLoader : MonoBehaviour
{
    public void LoadGrasslandsDemo()
    {
        MapLoader.jsonFileName = "demo.json";
        SceneManager.LoadScene("Grasslands");
    }

    public void DownloadTemplate()
    {

        string outputFileName = "SpawnSettings.JSON";

        // Path to JSON template in StreamingAssets (StreamingAssets is accessible at runtime, unlike Resources)
        string sourcePath = Path.Combine(Application.streamingAssetsPath, outputFileName);
        Debug.Log("Source path: " + sourcePath);

        // Path to Downloads folder
        string downloadsFolder = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile), "Downloads");
        string destinationPath = Path.Combine(downloadsFolder, outputFileName);
        Debug.Log("Destination path: " + destinationPath);

        if (File.Exists(sourcePath))
        {
            try
            {
                // copy the file to Downloads folder
                File.Copy(sourcePath, destinationPath, true);
                // file copies to original creation and modification timestamps, so update them to now
                File.SetCreationTime(destinationPath, System.DateTime.Now);
                File.SetLastWriteTime(destinationPath, System.DateTime.Now);

                // Debug.Log($":) Template copied to {destinationPath}");
                // Debug.Log("File exists at destination: " + File.Exists(destinationPath));
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Error copying file: " + ex.Message);
            }
        }
        else
        {
            Debug.LogWarning("⚠ Template file not found at " + sourcePath);
        }

    }

    public void UploadCustomTemplate()
    {
        // open users file explorer to select a JSON file
        string path = UnityEditor.EditorUtility.OpenFilePanel("Select Custom Template JSON", "", "json");
        MapLoader.jsonFilePath = path;
        MapLoader.jsonFileName = "userFile";
        Debug.Log("Selected file: " + path);
        SceneManager.LoadScene("Grasslands");

    }


}
