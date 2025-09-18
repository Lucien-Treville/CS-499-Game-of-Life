#if UNITY_EDITOR
using UnityEditor; // For file picker in Editor
#endif
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class MapUploadManager : MonoBehaviour
{
    // Upload button methods
    public void UploadGrasslands() => UploadMap("Level 1"); // Grasslands -> Level1 scene
    public void UploadTundra() => UploadMap("Level 2");     // Tundra -> Level2 scene
    public void UploadSwamp() => UploadMap("Level 3");      // Swamp -> Level3 scene
    public void UploadDesert() => UploadMap("Level 4");     // Desert -> Level4 scene

    private void UploadMap(string sceneName)
    {
#if UNITY_EDITOR
        // Open file picker for JSON files
        string path = EditorUtility.OpenFilePanel($"Select JSON for {sceneName}", "", "json");

        if (!string.IsNullOrEmpty(path))
        {
            // Read JSON contents
            string json = File.ReadAllText(path);
            Debug.Log($"JSON uploaded for {sceneName}: {json}");

            // Store JSON so the next scene can access it
            MapDataHolder.UploadedJson = json;

            // Load the scene
            SceneManager.LoadScene(sceneName);
        }
#else
        Debug.LogWarning("File upload only works inside the Unity Editor right now!");
#endif
    }
}
