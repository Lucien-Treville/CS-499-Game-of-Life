using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using TMPro;
using UnityEngine.UI;
using SFB;

// #if UNITY_EDITOR
// using UnityEditor;     // Only included when in Editor
// #endif

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

        string sourcePath = Path.Combine(Application.streamingAssetsPath, outputFileName);
        Debug.Log("Source path: " + sourcePath);

        string downloadsFolder = Path.Combine(
            System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile),
            "Downloads"
        );

        string destinationPath = Path.Combine(downloadsFolder, outputFileName);
        Debug.Log("Destination path: " + destinationPath);

        if (File.Exists(sourcePath))
        {
            try
            {
                File.Copy(sourcePath, destinationPath, true);
                File.SetCreationTime(destinationPath, System.DateTime.Now);
                File.SetLastWriteTime(destinationPath, System.DateTime.Now);
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

    void LoadJsonFromPath(string path)
    {
        // 3. Read the text content
        string fileContent = File.ReadAllText(path);
        
        Debug.Log($"Loaded: {path}");
        
        // 4. Pass to your Simulation Manager
        // SimulationManager.Instance.LoadConfig(fileContent);
    }
    public void UploadCustomTemplate()
    {
        WarningBox.gameObject.SetActive(false);

// #if UNITY_EDITOR
        // --- EDITOR MODE: Use OpenFilePanel normally ---
        // string path = EditorUtility.OpenFilePanel("Select Custom Template JSON", "", "json");
        // 1. Open the native file dialog
        // Arguments: Title, Directory, Extension Filter, Multiselect
        var extensions = new [] {
            new ExtensionFilter("JSON Files", "json"),
            new ExtensionFilter("All Files", "*" ),
        };

        // This returns a string array of paths (even if just 1 file selected)
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Open JSON Template", "", extensions, false);
        
        string path = paths[0];
        



        if (string.IsNullOrEmpty(path))
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

// #else
        // --- BUILD MODE: File picker is NOT allowed ---
        // Debug.LogWarning("Upload Custom Template only works inside the Unity Editor.");

        // WarningText.text = "⚠ Custom file upload only works in the Unity Editor.\n" +
        //                    "Build versions cannot open your file explorer.";
        // WarningBox.gameObject.SetActive(true);
// #endif
    }

    public void UseDnDeditor()
    {
        MapLoader.jsonFileName = "DnDeditor";
        DnDeditor.startInEditMode = true;
        SceneManager.LoadScene("Grasslands");
    }
}