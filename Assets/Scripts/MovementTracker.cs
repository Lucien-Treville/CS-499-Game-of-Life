using System.Collections.Generic;
using UnityEngine;

public class MovementTracker : MonoBehaviour
{
    public static Dictionary<string, List<Vector3>> paths =
        new Dictionary<string, List<Vector3>>();

    private static int globalCounter = 0;
    private string id;

    float logInterval = 0.25f;
    float timer = 0f;

    void Awake()
    {
        // IMPORTANT: Make this object survive scene changes
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // Make a UNIQUE id per creature instance
        id = gameObject.name + "_" + globalCounter++;
        paths[id] = new List<Vector3>();

        // First position
        paths[id].Add(transform.position);
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= logInterval)
        {
            timer = 0f;
            paths[id].Add(transform.position);
        }
    }
}
