using UnityEngine;
using System.IO;

public class MapInitializer : MonoBehaviour
{
    public GameObject defaultMapRoot;
    public MapLoader mapLoader;

    private string uploadedJsonPath;

    void Start()
    {
        uploadedJsonPath = Path.Combine(Application.persistentDataPath, "uploaded_template.json");

        if (File.Exists(uploadedJsonPath))
        {
            defaultMapRoot.SetActive(false); // hide default objects
            mapLoader.LoadMap(uploadedJsonPath);
        }
        else
        {
            defaultMapRoot.SetActive(true);  // show default objects
        }
    }
}
