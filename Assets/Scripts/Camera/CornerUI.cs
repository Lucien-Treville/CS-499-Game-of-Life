using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CornerUI : MonoBehaviour
{
    void Start()
    {
        transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0));
    }
}
