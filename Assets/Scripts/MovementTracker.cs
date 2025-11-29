using System.Collections.Generic;
using UnityEngine;

public class MovementTracker : MonoBehaviour
{
    public static Dictionary<string, List<Vector3>> paths =
        new Dictionary<string, List<Vector3>>();

    private string id;
    private float logInterval = 0.25f;
    private float timer = 0f;

    void Start()
    {
        id = GetInstanceID().ToString();

        if (!paths.ContainsKey(id))
            paths[id] = new List<Vector3>();

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
