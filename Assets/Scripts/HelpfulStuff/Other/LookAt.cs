using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{

    void Update()
    {
        gameObject.transform.LookAt(Camera.main.transform);
        gameObject.transform.Rotate(0, 180, 0);
    }
}
