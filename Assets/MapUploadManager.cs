#if UNITY_EDITOR
using UnityEditor; // For file picker in Editor
#endif
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class MapUploadManager : MonoBehaviour
{
    // Upload button methods (now point to your actual scene names)
    public void UploadGrasslands() => UploadMap("Grasslands");
    public void UploadTundra() => UploadMap("Tundra");
    public void UploadSwamp() => UploadMap("Swamp");
    public void UploadDesert() => UploadMap("Desert");

    public void UploadCustom(string baseMap)
    {
        PlayerPrefs.SetString("BaseMap", baseMap);
        UploadMap("Blank");
    }

    private void UploadMap(string sceneName)
    {
#if UNITY_EDITOR
        string path = EditorUtility.OpenFilePanel($"Select JSON for {sceneName}", "", "json");

        if (!string.IsNullOrEmpty(path))
        {
            string json = File.ReadAllText(path);
            Debug.Log($"JSON uploaded for {sceneName}:\n{json}");

            // Store uploaded JSON so the next scene can access it
            MapDataHolder.UploadedJson = json;

            // Load the scene
            SceneManager.LoadScene(sceneName);
        }
#else
        Debug.LogWarning("File upload only works inside the Unity Editor right now!");
#endif
    }
}
