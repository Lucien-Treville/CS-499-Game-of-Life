using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using TMPro;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{

    public TextMeshProUGUI WarningText;
    public Image WarningBox;

    void Start()
    {
        if (PopulationManager.Instance != null)
        {
            PopulationManager.Instance = null;
        }
        WarningBox.gameObject.SetActive(false);
    }


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
        WarningBox.gameObject.SetActive(false);
        // open users file explorer to select a JSON file
        
        string path = UnityEditor.EditorUtility.OpenFilePanel("Select Custom Template JSON", "", "json");
        if (path.Length == 0)
        {
            Debug.Log("No file selected.");
            return;
        }
        MapLoader.jsonFilePath = path;
        MapLoader.jsonFileName = "userFile";
        try
        {
            MapLoader.ReadJSON(path);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error reading custom JSON file: " + ex.Message);
            WarningText.text = "Error reading JSON file: " + ex.Message;
            WarningBox.gameObject.SetActive(true);
            return;
        }
        Debug.Log("Selected file: " + path);
        SceneManager.LoadScene("Grasslands");

    }

    public void UseDnDeditor()
    {
        MapLoader.jsonFileName = "DnDeditor";
        DnDeditor.startInEditMode = true;
        SceneManager.LoadScene("Grasslands");
    }


}
