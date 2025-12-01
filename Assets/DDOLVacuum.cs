using UnityEngine;
using UnityEngine.SceneManagement;


// the heatmap approach causes a lot of creatures to be DontDestroyOnLoad, meaning they never go away.
// this clutters memory, and makes them stay on later simulation runs, which is bad
// this script moves all DDOL objects from the DDOL area to the Report scene, so once the Report scene is exited, they all get cleaned up.
public class DDOLVacuum : MonoBehaviour
{
    void Start()
    {
        MoveAllDDOLObjectsHere();
    }

    void MoveAllDDOLObjectsHere()
    {
        // 1. THE TRICK: Accessing the secret DDOL Scene
        // We create a temp object and send it to DDOL so we can grab the scene handle.
        GameObject temp = new GameObject("TempDDOLReference");
        DontDestroyOnLoad(temp);
        
        Scene ddolScene = temp.scene;
        
        // Destroy the temp object immediately, we don't need it anymore
        DestroyImmediate(temp);

        // 2. Get every root object in that scene
        GameObject[] allDDOLObjects = ddolScene.GetRootGameObjects();
        Scene currentScene = SceneManager.GetActiveScene();

        Debug.Log($"Vacuuming {allDDOLObjects.Length} objects from DDOL to {currentScene.name}...");

        foreach (GameObject obj in allDDOLObjects)
        {
            // Move them to the current scene.
            // Now they are no longer immortal. They will die when this scene ends.
            SceneManager.MoveGameObjectToScene(obj, currentScene);
        }
    }
}