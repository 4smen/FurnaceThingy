using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    [SerializeField] float timeToDes;

    private void Start()
    {
        Destroy(gameObject, timeToDes);
    }
}
